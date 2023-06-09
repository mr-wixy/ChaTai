﻿using Ganss.Xss;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace ChaTai.Blazor.Pages.Components
{
    public class Markdown : ComponentBase
    {
        private string _content;

        [Inject]
        public IHtmlSanitizer HtmlSanitizer { get; set; }

        [Parameter]
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                HtmlContent = ConvertStringToMarkupString(_content);
            }
        }

        public MarkupString HtmlContent { get; private set; }

        private MarkupString ConvertStringToMarkupString(string value)
        {
            if (!string.IsNullOrWhiteSpace(_content))
            {
                // Convert markdown string to HTML
                var html = Markdig.Markdown.ToHtml(value, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

                // Sanitize HTML before rendering
                var sanitizedHtml = HtmlSanitizer.Sanitize(html);

                // Return sanitized HTML as a MarkupString that Blazor can render
                return new MarkupString(sanitizedHtml);
            }

            return new MarkupString();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(1,HtmlContent);
        }
    }
}
