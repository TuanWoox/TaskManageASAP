﻿using System;

namespace WebApplication1.DTOs.TaskDTO
{

    public record CreateTaskDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AssignedTo { get; set; }

        public DateTime DateStarted { get; set; }
        public DateTime DeadlineDate { get; set; }
    }

}
