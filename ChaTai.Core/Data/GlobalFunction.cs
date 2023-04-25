using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ChaTai.Core
{ 
    public class GlobalFunction
    {
        public static void LoadSecrets(string? secretStr)
        {
            if (string.IsNullOrEmpty(secretStr))
                GlobalVariable.Secrets = new List<string>();

            GlobalVariable.Secrets = secretStr.Split(',').ToList();
        }

        public static void LoadPromptList(string promptJson)
        {
            var promptData = JsonSerializer.Deserialize<DefaultPromptData>(promptJson);
            GlobalVariable.Prompts = new List<Prompt>();

            foreach (var item in promptData.CN)
            {
                if (item != null && item.Length == 2)
                    GlobalVariable.Prompts.Add(new Prompt(item[0], item[1]));
            }

            foreach (var item in promptData.EN)
            {
                if (item != null && item.Length == 2)
                    GlobalVariable.Prompts.Add(new Prompt(item[0], item[1]));
            }
        }
    }
}
