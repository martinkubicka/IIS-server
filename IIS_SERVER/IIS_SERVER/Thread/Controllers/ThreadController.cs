﻿using IIS_SERVER.Services;
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

        [HttpGet("GetAllThreads")]
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

        [HttpGet("GetThreads")]
        public async Task<IActionResult> GetThreadsFromSpecificGroup(string Handle,  int currentPage, int itemsPerPage, string? filterName, string? filterFromDate, string? filterToDate)
        {
            try
            {
                List<ThreadModel>? thread = await MySqlService.GetThreadsFromSpecificGroup(Handle, currentPage, itemsPerPage, filterName, filterFromDate, filterToDate);
                if (thread != null)
                {
                    return StatusCode(200, thread);
                }
                else
                {
                    return StatusCode(404, "Error: Thread not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("GetThreadsCount")]
        public async Task<IActionResult> GetThreadsCount(string Handle, string? filterName, string? filterFromDate, string? filterToDate)
        {
            try
            {
                int? count = await MySqlService.GetThreadsCount(Handle, filterName, filterFromDate, filterToDate);
                if (count != null)
                {
                    return StatusCode(200, count);
                }
                else
                {
                    return StatusCode(404, "Error: Group not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        
        [HttpGet("get/{threadId}")]
        public async Task<IActionResult> GetThread(Guid threadId)
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
        public async Task<IActionResult> UpdateThread(Guid threadId, ThreadModel updatedThread)
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
        public async Task<IActionResult> DeleteThread(Guid threadId)
        {
            try 
            {
                Tuple<bool, string?> result = await MySqlService.DeleteThread(threadId);

                if (result.Item1)
                {
                    return StatusCode(204, "Thread successfully deleted.");
                }
                else
                {

                    return StatusCode(404, "Error: Thread not found.");
                }
            }
            catch
            {
                return StatusCode(500, "Error: DB error occurred.");
            }

        }
        [HttpGet("GetAllThreadsUserIsIn/{handle}")]
        public async Task<IActionResult> GetAllThreadsUserIsIn(string handle)
        {
            List<ThreadModel>? thread = await MySqlService.GetAllThreadsUserIsIn(handle);
            if (thread != null)
            {
                return StatusCode(200, thread);
            }
            else
            {
                return StatusCode(404, "Error: Thread not found.");
            }
        }
    }
}
