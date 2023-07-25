namespace JobManagementSystem.AuthAPI.Domain
{
    public class BaseResponse : Exception
    {
        public int StatusCode { get; set; }
        public BaseResponse(int statusCode,string message) : base(message) 
        {
            StatusCode = statusCode;
        }
    }
}
