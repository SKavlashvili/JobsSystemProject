namespace JobManagementSystem.AuthAPI.Domain
{
    public class UserAlreadyExists : BaseResponse
    {
        public UserAlreadyExists() : base(400,"User with this email already exists")
        {

        }
    }
}
