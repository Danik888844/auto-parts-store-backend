using Ganss.Xss;

namespace AutoParts.Core.GeneralHelpers;

public interface IXssRepository
{
    string Execute(string text);
}

public class XssRepository : IXssRepository
{
    public string Execute(string text)
    {
        var sanitizer = new HtmlSanitizer();
        var sanitized = sanitizer.Sanitize(text);
        return sanitized;
    }
}