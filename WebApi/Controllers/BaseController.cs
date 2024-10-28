using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
  private readonly LinkGenerator _linkGenerator;

  public BaseController(LinkGenerator linkGenerator)
  {
    _linkGenerator = linkGenerator;
  }

  protected string? GetUrl(string linkName, object parameters)
  {
    string? url = null;

    if (linkName == "GetSearchHistory")
    {
      var paramDict = parameters.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(parameters, null));
      url = $"http://localhost:5002/api/SearchHistory?page={paramDict["page"]}&pageSize={paramDict["pageSize"]}";
    }
    else if (linkName == "GetSearchHistoryById")
    {
      var paramDict = parameters.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(parameters, null));
      url = $"http://localhost:5002/api/SearchHistory/{paramDict["userId"]}/{paramDict["SearchQuery"]}/{paramDict["CreatedAt"]:MM/dd/yyyy HH:mm:ss}";
    }

    return url;
  }

  protected string? GetLink(string linkName, int page, int pageSize)
  {
    return GetUrl(linkName, new { page, pageSize });
  }

  protected object CreatePaging<T>(string linkName, int page, int pageSize, int total, IEnumerable<T?> items)
  {
    const int MaxPageSize = 25;
    pageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;

    var numberOfPages = (int)Math.Ceiling(total / (double)pageSize);

    var curPage = GetLink(linkName, page, pageSize);
    var nextPage = page < numberOfPages - 1 ? GetLink(linkName, page + 1, pageSize) : null;
    var prevPage = page > 0 ? GetLink(linkName, page - 1, pageSize) : null;

    return new
    {
      CurPage = curPage,
      NextPage = nextPage,
      PrevPage = prevPage,
      NumberOfItems = total,
      NumberPages = numberOfPages,
      Items = items
    };
  }
}