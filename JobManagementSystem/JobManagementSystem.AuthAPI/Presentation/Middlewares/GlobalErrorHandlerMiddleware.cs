using JobManagementSystem.AuthAPI.Domain;

namespace JobManagementSystem.AuthAPI.Presentation
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }catch(BaseResponse baseEx)
            {
                context.Response.StatusCode = baseEx.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new 
                {
                    StatusCode = baseEx.StatusCode, 
                    Message = baseEx.Message
                });
            }catch(Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode =500,
                    Message = ex.Message
                });
            }
        }
    }
}
