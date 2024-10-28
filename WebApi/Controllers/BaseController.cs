using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Controllers
{
  [ApiController]
  public class BaseController : ControllerBase
  {
    private readonly LinkGenerator _linkGenerator;
    protected const int DefaultPageSize = 10;
    private const int MaxPageSize = 25;

    public BaseController(LinkGenerator linkGenerator)
    {
      _linkGenerator = linkGenerator;
    }

    protected string? GetUrl(string linkName, object args)
    {
      var uri = _linkGenerator.GetUriByName(HttpContext, linkName, args);
      Console.WriteLine($"Generating URL for linkName: {linkName}, args: {args}, result: {uri}");
      return uri;
    }
    protected string? GetLink(string linkName, int userId, int page, int pageSize)
    {
      return GetUrl(linkName, new { userId, page, pageSize });
    }

    protected object CreatePaging<T>(string linkName, int userId, int page, int pageSize, int total, IEnumerable<T?> items)
    {
      const int MaxPageSize = 25;
      pageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;
      var numberOfPages = (int)Math.Ceiling(total / (double)pageSize);

      var curPage = GetLink(linkName, userId, page, pageSize);
      var nextPage = page < numberOfPages - 1 ? GetLink(linkName, userId, page + 1, pageSize) : null;
      var prevPage = page > 0 ? GetLink(linkName, userId, page - 1, pageSize) : null;
      Console.WriteLine($"Previous Page Link for {linkName}: {prevPage}");
      Console.WriteLine($"Next Page Link for {linkName}: {nextPage}");
      Console.WriteLine($"Current Page Link for {linkName}: {curPage}");

      var result = new
      {
        CurPage = curPage,
        NextPage = nextPage,
        PrevPage = prevPage,
        NumberOfItems = total,
        NumberPages = numberOfPages,
        Items = items
      };

      return result;
    }

    protected string? GenerateSelfLink(string actionName, object routeValues)
    {
      return Url.Action(actionName, routeValues);
    }
  }
}