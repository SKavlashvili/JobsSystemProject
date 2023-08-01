namespace JobManagementSystem.API.Infrastructure
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalErrorHandler>();
        }
    }
}
