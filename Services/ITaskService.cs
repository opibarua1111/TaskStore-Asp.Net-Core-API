using TaskStore.Model;
using TaskStore.Model.ResponseModels;

namespace TaskStore.Services
{
    public interface ITaskService
    {
        Task<ApiResponse> GetTaskList(int skip, int length, string searchValue);
        Task<ApiResponse> saveTask(TaskResponse task);
        Task<ApiResponse> updateTask(TaskResponse task);
        Task<ApiResponse> deleteTask(Guid Id);
        Task<ApiResponse> getTaskById(Guid Id);

    }
}
