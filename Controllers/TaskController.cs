using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskStore.Model;
using TaskStore.Model.ResponseModels;
using TaskStore.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskStore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [HttpGet("GetTaskList")]
        public async Task<IActionResult> GetTaskList(int pageNo, int length, string searching_value)
        {
            ApiResponse response = new();
            try
            {
                int skip = pageNo > 0 ? (pageNo - 1) * length : 0;
                string searchValue = (searching_value != null) ? searching_value.ToLower() : searching_value;
                response = await _taskService.GetTaskList(skip, length, searchValue);
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.InnerException?.Message ?? ex.Message;
            }
            return Ok(response);
        }

        // GET api/<TaskController>/5
        [HttpGet("getTaskById")]
        public async Task<IActionResult> getTaskById(Guid Id)
        {
            ApiResponse response = new();
            try
            {
                response = await _taskService.getTaskById(Id);
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.InnerException?.Message ?? ex.Message;
            }
            return Ok(response);
        }

        [HttpPost("saveTask")]
        public async Task<IActionResult> saveTask(TaskResponse task)
        {
            ApiResponse response = new();
            try
            {
                response = await _taskService.saveTask(task);
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.InnerException?.Message ?? ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("updateTask")]
        public async Task<IActionResult> updateTask(TaskResponse task)
        {
            ApiResponse response = new();
            try
            {
                response = await _taskService.updateTask(task);
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.InnerException?.Message ?? ex.Message;
            }
            return Ok(response);
        }

        [HttpDelete("deleteTask")]
        public async Task<IActionResult> deleteTask(Guid Id)
        {
            ApiResponse response = new();
            try
            {
                response = await _taskService.deleteTask(Id);
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.InnerException?.Message ?? ex.Message;
            }
            return Ok(response);
        }
    }
}
