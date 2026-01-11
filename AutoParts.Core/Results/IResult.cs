using System.Net;

namespace AutoParts.Core.Results;

public interface IResult
{
    public HttpStatusCode StatusCode { get; set; }
    public bool Result { get; }
    public string Message { get; }
    public List<string> ErrorMessages { get; set; }
}