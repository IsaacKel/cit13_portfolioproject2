using DataLayer;

namespace WebApi.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next; 
        private readonly IDataService _dataService;

        public AuthMiddleware(RequestDelegate next, IDataService dataService)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            await _next(context);
        }
    }
}
