using Microsoft.AspNetCore.Components;

namespace ChaTai.Blazor.Pages.Components
{
    public class ToastOption
    {
        public string? Content { get; set; }

        public ToastOption(string content) {
            Content = content;
        }

        public RenderFragment? RenderFragment { get; set; }
    }
}
