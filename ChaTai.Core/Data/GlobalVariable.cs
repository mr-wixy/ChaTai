using System.Collections.Generic;

namespace ChaTai.Core
{
    public class GlobalVariable
    {
        public static List<string> Secrets {  get; set; }

        public static List<Prompt> Prompts {  get; set; }

        public static PlatformType Platform { get; set; }
    }
}
