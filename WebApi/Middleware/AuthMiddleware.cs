namespace WebApi.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        
        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
             await _next(context);
        }
    }
}
