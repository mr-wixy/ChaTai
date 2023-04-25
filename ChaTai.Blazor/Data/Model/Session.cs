namespace ChaTai.Blazor.Data
{
    public class Session
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string MemoryPrompt { get; set; }

        public string LastSummarizeIndex { get; set; }

        public List<Message> Messages { get; set; }

        public DateTime LastUpdateTime { get; set; }

    }
}
