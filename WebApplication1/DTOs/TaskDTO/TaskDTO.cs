﻿using System;

namespace WebApplication1.DTOs.TaskDTO
{
    public record TaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AssignedTo { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string Status { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DeadlineDate { get; set; }
    }
}
