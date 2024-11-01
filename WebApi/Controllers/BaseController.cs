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
      return uri;
    }
    protected string? GetLinkUser(string linkName, int userId, int page, int pageSize)
    {
      return GetUrl(linkName, new { userId, page, pageSize });
    }

    protected string? GetLinkNConst(string linkName, int nConst, int page, int pageSize)
    {
      return GetUrl(linkName, new { nConst, page, pageSize });
    }

    protected object CreatePagingUser<T>(string linkName, int userId, int page, int pageSize, int total, IEnumerable<T?> items)
    {
      const int MaxPageSize = 25;
      pageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;
      var numberOfPages = (int)Math.Ceiling(total / (double)pageSize);

      var curPage = GetLinkUser(linkName, userId, page, pageSize);
      var nextPage = page < numberOfPages - 1 ? GetLinkUser(linkName, userId, page + 1, pageSize) : null;
      var prevPage = page > 1 ? GetLinkUser(linkName, userId, page - 1, pageSize) : null;

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

    protected object CreatePaging<T>(string linkName, int page, int pageSize, int total, IEnumerable<T> items)
    {
      pageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;
      var numberOfPages = (int)Math.Ceiling(total / (double)pageSize);

      Console.WriteLine($"Link Name: {linkName}");
      Console.WriteLine($"Page: {page}, Page Size: {pageSize}, Total Items: {total}, Number of Pages: {numberOfPages}");

      // Generate links with nconst as a required route parameter
      var curPage = GetUrl(linkName, new { nconst = "<YOUR_NCONST>", page, pageSize });
      Console.WriteLine($"CurPage URL: {curPage}");

      var nextPage = page < numberOfPages ? GetUrl(linkName, new { nconst = "<YOUR_NCONST>", page = page + 1, pageSize }) : null;
      Console.WriteLine($"NextPage URL: {nextPage}");

      var prevPage = page > 1 ? GetUrl(linkName, new { nconst = "<YOUR_NCONST>", page = page - 1, pageSize }) : null;
      Console.WriteLine($"PrevPage URL: {prevPage}");

      // Return result with pagination details
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