using System.ComponentModel;

namespace ChaTai.Blazor.Data
{
    public enum ShortcutKey
    {
        [Description("Enter")]
        Enter,

        [Description("Ctrl + Enter")]
        CtrlEnter,

        [Description("Shift + Enter")]
        ShiftEnter
    }
}
