using IIS_SERVER.Services;
using IIS_SERVER.Thread.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Thread.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThreadController : ControllerBase, IThreadController
    {
        private readonly IMySQLService MySqlService;

        public ThreadController(IMySQLService mySqlService)
        {
            MySqlService = mySqlService ?? throw new ArgumentNullException(nameof(mySqlService));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateThread(ThreadModel thread)
        {
            Tuple<bool, string?> result = await MySqlService.CreateThread(thread);
            if (result.Item1)
            {
                return StatusCode(201, "Thread successfully added to DB.");
            }
            else
            {
                return StatusCode(500, result.Item2);       
            }
        }

        [HttpGet("getThreads")]
        public async Task<IActionResult> GetAllThreads()
        {
            List<ThreadModel>? thread = await MySqlService.GetAllThreads();
            if (thread != null)
            {
                return StatusCode(200, thread);
            }
            else
            {
                return StatusCode(404, "Error: Thread not found.");
            }
        }

        [HttpGet("get/{threadId}")]
        public async Task<IActionResult> GetThread(string threadId)
        {
            ThreadModel? thread = await MySqlService.GetThread(threadId);
            if (thread != null)
            {
                return StatusCode(200, thread);
            }
            else
            {
                return StatusCode(404, "Error: Thread not found.");
            }
        }

        [HttpPut("update/{threadId}")]
        public async Task<IActionResult> UpdateThread(string threadId, ThreadModel updatedThread)
        {
            bool result = await MySqlService.UpdateThread(threadId, updatedThread);
            if (result)
            {
                return StatusCode(200, "Thread successfully updated.");
            }
            else
            {
                return StatusCode(404, "Error: Thread not found or DB error occurred.");
            }
        }

        [HttpDelete("delete/{threadId}")]
        public async Task<IActionResult> DeleteThread(string threadId)
        {
            try
            {
                Tuple<bool, string?> result = await MySqlService.DeleteThread(threadId);

                if (result.Item1)
                {
                    return NoContent();
                }
                else
                {
                    if (result.Item2.Contains("admin"))
                    {
                        if (result.Item2.Contains("group"))
                        {
                            return StatusCode(403, "Error: Thread cannot be deleted because it is an admin in one or more groups.");
                        }
                        else
                        {
                            return StatusCode(403, "Error: Thread cannot be deleted because it is a system admin.");
                        }
                    }
                    else
                    {
                        return StatusCode(500, "Error: An internal server error occurred.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: An internal server error occurred.");
            }
        }
    }
}
