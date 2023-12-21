using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskStore.Data;
using TaskStore.Model;
using TaskStore.Model.ResponseModels;

namespace TaskStore.Services
{
    public class TaskService : ITaskService
    {

        public readonly ApplicationDBContext _context;
        public TaskService(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse> GetTaskList(int skip, int length, string searchValue)
        {
            ApiResponse response = new() { statusCode = 200 };

            try
            {
                var taskList = await _context.Tasks
                    .Where(x => x.IsDeleted != true)
                    .Select(task => new TaskResponse()
                    {
                       Id = task.Id,
                       Title = task.Title,
                       Description = task.Description,
                       DueDate = task.DueDate
                    }).Skip(skip).Take(length).ToListAsync();

                var totalrows = _context.Tasks.Where(x => x.IsDeleted == false).Count();

                response.data = JsonSerializer.Serialize(new { taskList = taskList, totalrows = totalrows });

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResponse> saveTask(TaskResponse data)
        {
            ApiResponse response = new() { statusCode = 200 };

            try
            {
                Model.Task task = new Model.Task();
                task.Id = Guid.NewGuid();
                task.CreatedDate = DateTime.Now;
                task.Title = data.Title;
                task.Description = data.Description;
                task.Status = data.Status;
                task.DueDate = data.DueDate;
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();
                response.message = "Task Save Sucessfully";
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> updateTask(TaskResponse data)
        {
            ApiResponse response = new() { statusCode = 200 };

            try
            {
                if (data.Id != null)
                {
                    var task = await _context.Tasks.Where(x => x.Id == data.Id && x.IsDeleted != true).FirstOrDefaultAsync();
                    if (task == null)
                    {
                        throw new Exception("This id task couldn't found");
                    }
                    task.ModifiedDate = DateTime.Now;
                    task.Title = data.Title;
                    task.Description = data.Description;
                    task.DueDate = data.DueDate;
                    _context.Entry(task).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    response.message = "Task Update Sucessfully";
                }
                else
                {
                    throw new Exception("Provide Id");
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse> deleteTask(Guid Id)
        {
            ApiResponse response = new() { statusCode = 200 };

            try
            {
                var task = await _context.Tasks.Where(x => x.Id == Id && x.IsDeleted != true).FirstOrDefaultAsync();
                if (task == null)
                {
                    throw new Exception("This id task couldn't found");
                }
                task.IsDeleted = true;
                task.DeletedDate = DateTime.Now;
                _context.Entry(task).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                response.message = "Task Deleted Sucessfully";

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
