using System.Text.RegularExpressions;

namespace GatitoChat.Core.Security;
public static partial class TextUtils
{
    [GeneratedRegex("^[\\w\\.-]+@[a-zA-Z\\d\\.-]+\\.[a-zA-Z]{2,6}$", RegexOptions.Compiled)]
    public static partial Regex EmailRegex();
}
