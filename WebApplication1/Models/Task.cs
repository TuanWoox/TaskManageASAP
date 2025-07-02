using System;
namespace WebApplication1.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AssignedTo { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DeadlineDate { get; set; }
        public bool IsDeleted { get; set; }
    }

}
