using Blazored.LocalStorage;
using Ganss.Xss;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using ChaTai.Core;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using OpenAI.GPT3.Extensions;

namespace ChaTai.Blazor
{
    public static class ChaTaiSetup
    {
        public static void AddChaTaiSetup(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddBlazoredLocalStorage(config =>
            {
                config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                config.JsonSerializerOptions.WriteIndented = false;
                config.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
            });
            services.AddOpenAIService(setting =>
            {
                setting.ApiKey = configuration["OpenAI:ApiKey"];
                if (!string.IsNullOrEmpty(configuration["OpenAI:ProxyUrl"]))
                {
                    setting.BaseDomain = configuration["OpenAI:ProxyUrl"];
                }
            });
            services.AddScoped<IHtmlSanitizer, HtmlSanitizer>(x =>
            {
                // Configure sanitizer rules as needed here.
                // For now, just use default rules + allow class attributes
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedAttributes.Add("class");
                return sanitizer;
            });

            GlobalFunction.LoadSecrets(configuration["Secrets"]);

            if (GlobalVariable.Platform == PlatformType.Web)
            {
                var promptFile = File.ReadAllText("prompt.json");
                GlobalFunction.LoadPromptList(promptFile);
            }
            else
            {
                var a = typeof(ChaTaiSetup).GetTypeInfo().Assembly;
                using var stream = a.GetManifestResourceStream("ChaTai.Blazor.prompt.json");
                using var reader = new StreamReader(stream);
                var promptFile = reader.ReadToEnd();
                GlobalFunction.LoadPromptList(promptFile);
            }
        }
    }
}
