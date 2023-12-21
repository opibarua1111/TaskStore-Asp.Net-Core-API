using TaskStore.Model;

namespace TaskStore.Services
{
    public interface IUserService
    {
        Task<ApiResponse> Register(AddUserRequest addUserRequest);
        Task<ApiResponse> Login(AddUserRequest addUserRequest);
    }
}
