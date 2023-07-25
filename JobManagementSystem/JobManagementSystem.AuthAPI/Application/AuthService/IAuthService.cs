namespace JobManagementSystem.AuthAPI.Application
{
    public interface IAuthService
    {
        Task<int> RegisterNewUser(NewUserRequest newUser);
    }
}
