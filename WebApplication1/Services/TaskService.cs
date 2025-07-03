using System.Collections.Generic;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.DTOs.TaskDTO;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class TaskService
    {
        private readonly TaskRepository taskRepository;

        public TaskService(TaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
        }
        
        public List<TaskDTO> FindTasks()
        {
            List<Task> tasks = taskRepository.FindAll();
            
            return ConvertsListToDTO(tasks);
        }
        public TaskDTO FindOneTask(int id)
        {
            Task task = taskRepository.FindOneTask(id);
               
            return ConvertsOneToDTO(task);
        }
        public TaskDTO CreateNewTask(CreateTaskDTO createTaskDTO)
        {
            return ConvertsOneToDTO(taskRepository.CreateNewTask(createTaskDTO));
        }
        public bool UpdateNewTask(UpdateTaskDTO updateTaskDTO,int id)
        {
            return taskRepository.UpdateTask(updateTaskDTO, id);
        }
        public bool DeleteTask(int id)
        {
            return taskRepository.DeleteTask(id);
        }


        //This function is used to convert List<Task> to List<TaskDTO> after fetch so it is private
        private List<TaskDTO> ConvertsListToDTO(List<Task> tasks)
        {
            return tasks.Select(task => new TaskDTO
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                AssignedTo = task.AssignedTo,
                CreatedBy = task.CreatedBy,
                UpdatedBy = task.UpdatedBy,
                DateStarted = task.DateStarted,
                DeadlineDate = task.DeadlineDate,
                Status = task.Status,
            }).ToList();
        }

        //This function is used to convert Task to TaskDTO after fetch so it is private
        private TaskDTO ConvertsOneToDTO(Task task)
        {
            if(task == null) return null;   
            return new TaskDTO
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                AssignedTo = task.AssignedTo,
                CreatedBy = task.CreatedBy,
                UpdatedBy = task.UpdatedBy,
                DateStarted = task.DateStarted,
                DeadlineDate = task.DeadlineDate,
                Status = task.Status,
            };
        }
        

    }
}
