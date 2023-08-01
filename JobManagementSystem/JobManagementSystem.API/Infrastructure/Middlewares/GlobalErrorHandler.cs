namespace JobManagementSystem.API.Infrastructure
{
    public class GlobalErrorHandler 
    {
        private readonly RequestDelegate _next;
        public GlobalErrorHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { Message = ex.Message, StatusCode = 500});
            }
        }
    }
}
