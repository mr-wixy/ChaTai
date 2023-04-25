using ChaTai.Blazor.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using Blazored.LocalStorage;
using ChaTai.Blazor.Shared;
using ChaTai.Blazor.Services;
using System.Diagnostics.CodeAnalysis;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using Microsoft.Extensions.Configuration;
using ChaTai.Core;

namespace ChaTai.Blazor.Pages
{
    [Route("/")]
    [Layout(typeof(NoneLayout))]
    public partial class Index : IAsyncDisposable
    {
        #region Privite Member

        private DotNetObjectReference<Index> _objectReference;
        private int _windowWidth { get; set; } = 1980;
        private IOpenAIService? _proOpenAIService { get; set; }

        private List<Func<Task>> _afterRenderAsyncJobs = new();

        #endregion

        #region Injection

        [Inject]
        [NotNull]
        public HttpClient? HttpClient { get; set; }

        [Inject]
        [NotNull]
        public IJSRuntime? JSRuntime { get; set; }

        [Inject]
        [NotNull]
        public ILocalStorageService? LocalStorage { get; set; }

        [Inject]
        [NotNull]
        public IOpenAIService? OpenAIService { get; set; }

        [Inject]
        [NotNull]
        public IConfiguration? Configuration { get; set; }

        [Inject]
        [NotNull]
        private NavigationManager? NavigationManager { get; set; }

        #endregion

        #region Properties

        List<Session> ChatList { get; set; }

        Session CurrentSession { get; set; }

        List<Message> CurrentMessageList { get; set; }

        AppConfig AppConfig { get; set; }

        bool IsMobile
        {
            get
            {
                return _windowWidth <= 600;
            }
        }

        bool isInited { get; set; } = false;

        #endregion

        public bool showSideBar = true;

        public bool openSettings = false;

        public string? UserId = string.Empty;

        public string? Title = string.Empty;

        public string? Description = string.Empty;

        #region Impl

        protected override async Task OnInitializedAsync()
        {
            _objectReference = DotNetObjectReference.Create(this);

            Title = Configuration["AppName"];
            Description = Configuration["AppDescription"];

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    var config = await LocalStorage.GetItemAsync<AppConfig>("config");
                    if (config != null)
                        AppConfig = config;
                    else
                        AppConfig = new()
                        {
                            TightBorder = GlobalVariable.Platform == PlatformType.Web ? false : true,
                        };

                    var chatList = await LocalStorage.GetItemAsync<List<Session>>("sessions");
                    if (chatList != null)
                    {
                        ChatList = chatList;
                    }
                    else
                    {
                        ChatList = new List<Session>
                    {
                        new Session
                        {
                            Id=Guid.NewGuid(),
                            Title="新的对话",
                            LastUpdateTime = DateTime.Now,
                            Messages = new List<Message>
                            {
                                new Message
                                {
                                    Content="有什么可以帮你的吗",
                                    Role = "assistant",
                                    IsError = true,
                                    Time = DateTime.Now
                                }
                            }
                        }
                    };
                    }
                    await SelectChat(ChatList.First());
                    isInited = true;
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
                await InitWindowWidthListener();
            }
            try
            {
                while (_afterRenderAsyncJobs.Any())
                {
                    var job = _afterRenderAsyncJobs.First();
                    _afterRenderAsyncJobs.Remove(job);
                    await job.Invoke();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("RemoveWindowWidthListener", _objectReference);
            }
            catch (Exception ex)
            {

            }
            _objectReference?.Dispose();
        }

        #endregion

        #region Method

        public async Task AddChat()
        {
            CurrentSession = new Session
            {
                Id = Guid.NewGuid(),
                Title = "新的对话",
                Messages = new List<Message>
            {
                new Message
                {
                    Content="有什么可以帮你的吗",
                    Role="assistant",
                    Time = DateTime.Now,
                }
            },
                LastUpdateTime = DateTime.Now
            };
            ChatList.Add(CurrentSession);
            await LocalStorage.SetItemAsync("sessions", ChatList);
        }

        public async Task SelectChat(Session chat)
        {
            CurrentSession = chat;
            _afterRenderAsyncJobs.Add(ScrolToBottom);
            _afterRenderAsyncJobs.Add(HighLightCode);
        }

        public async Task DeleteChat(Session chat)
        {
            ChatList.Remove(chat);

            if (ChatList.Count == 0)
            {
                await AddChat();
            }

            await SelectChat(ChatList.FirstOrDefault()!);
            await LocalStorage.SetItemAsync("sessions", ChatList);
            StateHasChanged();
        }

        public void GoToGithub()
        {
            NavigationManager.NavigateTo($"https://github.com/mr-wixy/ChaTai", true);
        }

        public void ChangeSidebarVisible()
        {
            showSideBar = !showSideBar;
        }

        #endregion

        #region JS Function

        [JSInvokable]
        public void UpdateWindowWidth(int windowWidth)
        {
            _windowWidth = windowWidth;
            StateHasChanged();
        }

        private async Task InitWindowWidthListener()
        {
            await JSRuntime.InvokeVoidAsync("AddWindowWidthListener", _objectReference);
        }

        public async Task RemoveChatScrollListener()
        {
            await JSRuntime.InvokeVoidAsync("RemoveWindowWidthListener", _objectReference);
        }

        public async Task ScrolToBottom()
        {
            await JSRuntime.InvokeVoidAsync("ScrollToBottom");
        }

        public async Task HighLightCode()
        {
            await JSRuntime.InvokeVoidAsync("highlightAll");
        }
        #endregion

        public async Task SendMessage(string currentMessage)
        {
            var request = new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>(),
                Model = Models.ChatGpt3_5Turbo,
                MaxTokens = 1000//optional
            };

            var hisMsg = CurrentSession.Messages.Where(p => !p.IsError).TakeLast(AppConfig.HistoryMessageCount);
            request.Messages.Add(ChatMessage.FromSystem(AppConst.DefaultSystemPrompt));
            foreach (var item in hisMsg)
            {
                if (item.Role == "user")
                {
                    request.Messages.Add(ChatMessage.FromUser(item.Content));
                }
                else
                {
                    request.Messages.Add(ChatMessage.FromAssistant(item.Content));
                }
            }

            var userSendMessage = new Message
            {
                Content = currentMessage,
                Role = "user",
                Time = DateTime.Now,
                Id = Guid.NewGuid()
            };

            if (string.IsNullOrEmpty(AppConfig.Secret))
            {
                CurrentSession.Messages.Add(userSendMessage);
                var resultMessage = new Message
                {
                    Content = "未设置Secret",
                    IsError = true,
                    QuestionID = userSendMessage.Id,
                    Role = "assistant",
                    Time = DateTime.Now
                };
                CurrentSession.Messages.Add(resultMessage);
                StateHasChanged();
                _afterRenderAsyncJobs.Add(ScrolToBottom);
                return;
            }

            if(!GlobalVariable.Secrets.Contains(AppConfig.Secret))
            {
                CurrentSession.Messages.Add(userSendMessage);
                var resultMessage = new Message
                {
                    Content = "Secret不正确",
                    IsError = true,
                    QuestionID = userSendMessage.Id,
                    Role = "assistant",
                    Time = DateTime.Now
                };
                CurrentSession.Messages.Add(resultMessage);
                StateHasChanged();
                _afterRenderAsyncJobs.Add(ScrolToBottom);
                return;
            }
            else
            {
                var lastAssistantChat = request.Messages.LastOrDefault(p => p.Role == "assistant" && p.Content.Length > 50);
                if (lastAssistantChat != null && CurrentSession.Title == "新的对话")
                {
                    var topicRequest = new ChatCompletionCreateRequest
                    {
                        Messages = new List<ChatMessage>(),
                        Model = Models.ChatGpt3_5Turbo,
                        MaxTokens = 50
                    };
                    topicRequest.Messages.Add(ChatMessage.FromAssistant(lastAssistantChat.Content));
                    _ = Task.Run(async () =>
                    {
                        var sessionId = CurrentSession.Id;
                        topicRequest.Messages.Add(ChatMessage.FromUser(AppConst.TopicSummary));
                        var topicCompletionResult = await OpenAIService.ChatCompletion.CreateCompletion(topicRequest);
                        if (topicCompletionResult.Successful)
                        {
                            var topic = topicCompletionResult.Choices.FirstOrDefault().Message.Content;
                            topic = topic.Replace("。", "").Replace("，", "").Replace("！", "").Replace("？", "");
                            topic = topic.Replace(",", "").Replace(".", "").Replace("?", "").Replace("!", "");
                            ChatList.FirstOrDefault(p => p.Id == sessionId).Title = topic;
                            await LocalStorage.SetItemAsync("sessions", ChatList);
                            _ = InvokeAsync(() => StateHasChanged());
                        }
                    });
                }

                CurrentSession.Messages.Add(userSendMessage);
                request.Messages.Add(ChatMessage.FromUser(currentMessage));
                currentMessage = string.Empty;

                var resultMessage = new Message
                {
                    Content = "",
                    Role = "assistant",
                    Time = DateTime.Now,
                    QuestionID = userSendMessage.Id,
                    IsStream = true
                };
                CurrentSession.Messages.Add(resultMessage);
                StateHasChanged();

                _afterRenderAsyncJobs.Add(ScrolToBottom);

                var questionChat = CurrentSession.Messages.FirstOrDefault(p => p.Id == resultMessage.QuestionID);
                int useCount = 1;
                try
                {
                    var completionResult = OpenAIService.ChatCompletion.CreateCompletionAsStream(request);
                    await foreach (var completion in completionResult)
                    {
                        if (completion.Successful)
                        {
                            resultMessage.Content += completion.Choices.FirstOrDefault()?.Message.Content;
                            _ = InvokeAsync(() => StateHasChanged());
                        }
                        else
                        {
                            useCount = 0;
                            if (completion.Error == null)
                            {
                                throw new Exception("Unknown Error");
                            }
                            Console.WriteLine($"{completion.Error.Code}: {completion.Error.Message}");
                            resultMessage.Content = "系统发生错误！";
                            resultMessage.IsError = true;
                            _ = InvokeAsync(StateHasChanged);
                        }
                    }

                    if (resultMessage.IsError)
                    {
                        questionChat.IsError = true;
                    }

                    resultMessage.IsStream = false;

                    await LocalStorage.SetItemAsync("sessions", ChatList);
                    _ = InvokeAsync(StateHasChanged);

                    _afterRenderAsyncJobs.Add(ScrolToBottom);
                    _afterRenderAsyncJobs.Add(HighLightCode);
                }
                catch (Exception ex)
                {

                }
            }

        }
    }
}
