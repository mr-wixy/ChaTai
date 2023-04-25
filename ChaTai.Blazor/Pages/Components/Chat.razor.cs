using AngleSharp.Css.Values;
using AngleSharp.Html.Dom.Events;
using ChaTai.Blazor.Data;
using ChaTai.Blazor.Pages.Components.Toast;
using ChaTai.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace ChaTai.Blazor.Pages.Components
{
    public partial class Chat:IAsyncDisposable
    {
        private DotNetObjectReference<Chat> _objectReference;

        [Parameter]
        public Session CurrentSession { get; set; }

        [Parameter]
        public bool IsMobile { get; set; }

        public bool ShowPromptModal { get; set; } = false;

        [Parameter]
        public AppConfig AppConfig { get; set; }

        [Parameter]
        public EventCallback<AppConfig> AppConfigChanged { get; set; }

        [Parameter]
        public string UserMessage { get; set; }

        [Parameter]
        public EventCallback<string> UserMessageChanged { get; set; }

        [Parameter]
        public EventCallback<MouseEvent> ChangeSideVisiable { get; set; }

        [Parameter]
        public EventCallback<string> SendMessage { get; set; }

        public bool ChatIsScollBottom { get; set; } = true;

        [Inject]
        [NotNull]
        public IJSRuntime? JSRuntime { get; set; }

        public List<Prompt>? PromptList { get; set; }

        protected override Task OnInitializedAsync()
        {
            _objectReference = DotNetObjectReference.Create(this);
            return base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                await AddScrollListener();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override Task OnParametersSetAsync()
        {
            return base.OnParametersSetAsync();
        }

        public async Task ChangeSidebarVisible()
        {
            if (ChangeSideVisiable.HasDelegate)
                await ChangeSideVisiable.InvokeAsync();
        }

        public async Task ChangePromptModalVisible()
        {
            ShowPromptModal = !ShowPromptModal;
        }

        public async Task CopyText(string text)
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", text);
            var toastOp = new ToastOption("复制成功");
            ToastContainer.Instance?.Add(toastOp);
            await Task.Delay(1000);
            ToastContainer.Instance?.Remove(toastOp);
        }

        private async Task KeySubmit(KeyboardEventArgs args)
        {
            SearchPrompt();
            await Task.Delay(100);
            if (AppConfig.ShortcutKey == ShortcutKey.Enter)
            {
                if (args.Key == "Enter")
                {
                    await Send();
                }
            }
            else if (AppConfig.ShortcutKey == ShortcutKey.CtrlEnter)
            {
                if (args is { CtrlKey: true, Key: "Enter" })
                {
                    await Send();
                }
            }
            else if (AppConfig.ShortcutKey == ShortcutKey.ShiftEnter)
            {
                if (args is { ShiftKey: true, Key: "Enter" })
                {
                    await Send();
                }
            }
        }

        public async Task Send()
        {
            if (SendMessage.HasDelegate)
            {
                if(AppConfig.ShortcutKey == ShortcutKey.Enter)
                {
                    if (UserMessage == "\n")
                        return;
                    if (UserMessage.EndsWith("\n"))
                        UserMessage = UserMessage.Substring(0, UserMessage.Length - 1);
                }
                await SendMessage.InvokeAsync(UserMessage);
                UserMessage = string.Empty;
            }
        }

        public async Task GoToBottom()
        {
            await JSRuntime.InvokeVoidAsync("ScrollToBottom");
        }

        public async Task ChangeTightBorder()
        {
            AppConfig.TightBorder = !AppConfig.TightBorder;
            if(AppConfigChanged.HasDelegate)
                await AppConfigChanged.InvokeAsync(AppConfig);
        }

        public async Task ChangeTheme(Theme theme)
        {
            AppConfig.Theme = theme;
            if (AppConfigChanged.HasDelegate)
                await AppConfigChanged.InvokeAsync(AppConfig);

            await JSRuntime.InvokeVoidAsync("ChangeTheme", theme.GetDescription());
        }

        public void SelectPrompt(Prompt prompt)
        {
            UserMessage = prompt.Content;
            PromptList = null;
        }

        public void SearchPrompt()
        {
            if(UserMessage != null && UserMessage.StartsWith("/"))
            {
                if(UserMessage == "/")
                {
                    PromptList = GlobalVariable.Prompts;
                }
                else
                {
                    var searchText = UserMessage.Replace("/", "");
                    PromptList = GlobalVariable.Prompts.Where(p => p.Title.Contains(searchText)).ToList();
                }
            }
            else
            {
                PromptList = null;
            }
        }

        public async Task AddScrollListener()
        {
            await JSRuntime.InvokeVoidAsync("AddScrollListener", _objectReference);
        }

        public async Task RemoveScrollListener()
        {
            await JSRuntime.InvokeVoidAsync("RemoveScrollListener", _objectReference);
        }

        [JSInvokable]
        public void UpdateScroollPosition(bool isBottom)
        {
            ChatIsScollBottom = isBottom;
            StateHasChanged();
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await RemoveScrollListener();
            }
            catch(Exception ex)
            { 
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
