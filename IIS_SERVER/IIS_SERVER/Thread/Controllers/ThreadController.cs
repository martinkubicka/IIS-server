﻿using IIS_SERVER.Services;
using IIS_SERVER.Thread.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
            bool result = await MySqlService.CreateThread(thread);
            if (result)
            {
                return StatusCode(201, "Thread successfully created.");
            }
            else
            {
                return StatusCode(500, "Error: Failed to create the thread.");
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
            bool result = await MySqlService.DeleteThread(threadId);
            if (result)
            {
                return StatusCode(200, "Thread successfully deleted.");
            }
            else
            {
                return StatusCode(404, "Error: Thread not found or DB error occurred.");
            }
        }
    }
}
