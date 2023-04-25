namespace ChaTai.Blazor.Data
{
    public class Message
    {
        public string Content { get; set; }

        public string MessageType { get; set; } = "msg";

        public bool IsError { get; set; }

        public Guid Id { get; set; }

        public Guid QuestionID { get; set; }

        public string Role { get; set; }

        public DateTime Time { get; set; }

        public bool IsStream { get; set; } 
    }
}
