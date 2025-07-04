﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.DTOs.TaskDTO;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class TaskRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TaskRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public List<Task> FindAll()
        {
            return _dbContext.Tasks
                .Where(task => task.IsDeleted != true)
                .ToList();
        }


        public Task FindOneTask(int id)
        {
            return _dbContext.Tasks.FirstOrDefault(t => t.Id == id);
        }
        public Task CreateNewTask(CreateTaskDTO createTaskDTO)
        {
            Task newTask = ConvertCreateDTOTOne(createTaskDTO);

            _dbContext.Tasks.Add(newTask);

            //When entity save => it will also fill the id
            _dbContext.SaveChanges();

            return newTask;
        }
        public bool UpdateTask(UpdateTaskDTO updateTaskDTO, int id)
        {
            Task existingTask = _dbContext.Tasks.FirstOrDefault(t => t.Id == id && !t.IsDeleted);

            if (existingTask == null)
            {
                return false;
            }

            // Map fields
            existingTask.Name = updateTaskDTO.Name;
            existingTask.Description = updateTaskDTO.Description;
            existingTask.AssignedTo = updateTaskDTO.AssignedTo;
            existingTask.UpdatedBy = TakeUserIdOutOfToken();
            existingTask.DateStarted = updateTaskDTO.DateStarted;
            existingTask.DeadlineDate = updateTaskDTO.DeadlineDate;

            _dbContext.SaveChanges();

            return true;
        }

        public bool DeleteTask(int id)
        {
            Task task = _dbContext.Tasks.FirstOrDefault(t => t.Id == id && !t.IsDeleted);

            if (task != null)
            {
                task.IsDeleted = true;
                _dbContext.SaveChanges();   
                return true;
            }
            return false;
        }
        private Task ConvertCreateDTOTOne(CreateTaskDTO createTaskDTO)
        {
            

            return new Task
            {
                Name = createTaskDTO.Name,
                Description = createTaskDTO.Description,
                AssignedTo = createTaskDTO.AssignedTo,
                CreatedBy = TakeUserIdOutOfToken(),
                UpdatedBy = TakeUserIdOutOfToken(),
                DateStarted = createTaskDTO.DateStarted,
                DeadlineDate = createTaskDTO.DeadlineDate,
                IsDeleted = false,
                Status = "waiting"
            };

        }

        private int TakeUserIdOutOfToken()
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            return int.Parse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
