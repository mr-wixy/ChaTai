namespace ChaTai.Blazor.Data
{
    public class AppConst
    {
        public const string TopicSummary = "使用四到五个字直接返回这句话的简要主题，不要解释、不要标点、不要语气词、不要多余文本，如果没有主题，请直接返回“闲聊”";

        public static string DefaultSystemPrompt
        {
            get
            {
                return $"You are ChatGPT, a large language model trained by OpenAI. Answer as concisely as possible.Knowledge cutoff: 2021-09-01 Current date: {DateTime.Now:yyyy-MM-dd}";
            }
        }
    }
}
