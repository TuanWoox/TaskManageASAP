﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebApplication1.DTOs.TaskDTO;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
    
        private readonly TaskService taskService;

        public TaskController(TaskService taskService)
        {
            this.taskService = taskService;
        }

        //GET api/tasks => to get tasks, right now is all but future possible have pagination
        [HttpGet]
        
        public ActionResult<IEnumerable<TaskDTO>> GetTasks()
        {
            try
            {
                List<TaskDTO> tasks = taskService.FindTasks();
                return Ok(tasks);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        //GET api/tasks/1 => to get one task 
        [HttpGet("{id}", Name = "GetTaskById")]
        public ActionResult<TaskDTO> GetOneTask(int id)
        {
            try
            {
                TaskDTO task = taskService.FindOneTask(id);
                if (task == null) return NotFound();
                return Ok(task);
            } catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        //POST api/tasks => to create a new task
        [HttpPost]
        public ActionResult CreateNewTask([FromBody] CreateTaskDTO createTaskDTO)
        {
            try
            {
                TaskDTO task = taskService.CreateNewTask(createTaskDTO);
                return CreatedAtRoute(
                   routeName: "GetTaskById",
                   routeValues: new { id = task.Id },
                   value: task
               );

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }

        }

        //PUT api/tasks/id => to adjust one task, return a completely new, just need to return true
        [HttpPut("{id}")]
        [RequireAdminForDoneStatus]
        public ActionResult UpdateOneTask([FromBody] UpdateTaskDTO updateTaskDTO, int id)
        {
            try
            {
                bool updatedSuccessfully = taskService.UpdateNewTask(updateTaskDTO, id);
                if (updatedSuccessfully)
                {
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        //DELETE api/tasks/1 => to delete one task, just need to return true
        [HttpDelete("{id}")]
        public ActionResult DeleteTask(int id)
        {
            try
            {
                bool deletedSuccesfully = taskService.DeleteTask(id);
                if (deletedSuccesfully)
                {
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
