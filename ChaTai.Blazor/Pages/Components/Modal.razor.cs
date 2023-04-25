using Microsoft.AspNetCore.Components;

namespace ChaTai.Blazor.Pages.Components
{
    public partial class Modal
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public bool ShowFooter { get; set; } = false;

        [Parameter]
        public bool ShowConfirmBtn { get; set; } = false;

        [Parameter]
        public bool ShowCancelBtn { get; set; } = false;

        [Parameter]
        public string ConfirmBtnText { get; set; }

        [Parameter]
        public string CancelBtnText { get; set; }

        [Parameter]
        public string ContentCss { get; set; }

        [Parameter]
        public EventCallback OnConfirmBtnClick { get; set;}

        [Parameter]
        public EventCallback OnCancelBtnClick { get; set;}
    }
}
