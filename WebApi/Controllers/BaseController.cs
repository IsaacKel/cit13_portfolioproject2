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

    protected string? GetLinkNConst(string linkName, string nconst, int page, int pageSize)
    {
      return GetUrl(linkName, new { nconst, page, pageSize });
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

    protected object CreatePagingNConst<T>(string linkName, string nconst, int page, int pageSize, int total, IEnumerable<T?> items)
    {
      pageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;
      var numberOfPages = (int)Math.Ceiling(total / (double)pageSize);

      // Current page link
      var curPage = GetLinkNConst(linkName, nconst, page, pageSize);

      // Next page link (only if there is a next page)
      var nextPage = page < numberOfPages ? GetLinkNConst(linkName, nconst, page + 1, pageSize) : null;

      // Previous page link (only if there is a previous page)
      var prevPage = page > 1 ? GetLinkNConst(linkName, nconst, page - 1, pageSize) : null;

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