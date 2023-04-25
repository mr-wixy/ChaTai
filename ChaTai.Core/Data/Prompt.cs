namespace ChaTai.Core
{
    public class Prompt
    {
        public string? Title { get; set; }

        public string? Content { get; set; }

        public Prompt()
        {
        }

        public Prompt(string title, string content) {
            (Title, Content) = (title, content);
        }
    }
}
