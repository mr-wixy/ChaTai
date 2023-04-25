using System.Text.Json.Serialization;

namespace ChaTai.Core
{
    public class DefaultPromptData
    {
        [JsonPropertyName("cn")]
        public string[][] CN { get; set; }

        [JsonPropertyName("en")]
        public string[][] EN { get; set; }

        public DefaultPromptData()
        {
        }
    }
}
