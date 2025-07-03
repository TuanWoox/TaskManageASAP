using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("tasks")]
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        // Foreign Key Properties
        public int? AssignedTo { get; set; }

        [Column("createdby")]
        public int CreatedBy { get; set; }

        [Column("updatedBy")]
        public int UpdatedBy { get; set; }

        [Column("datestarted")]
        public DateTime DateStarted { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("deadlinedate")]
        public DateTime DeadlineDate { get; set; }

        [Column("isdeleted")]
        public bool IsDeleted { get; set; }

        // Navigation Properties - These connect to User entity

        //Use the property AssignedTo as the foreign key for the navigation property AssignedUser
        [ForeignKey("AssignedTo")]
        public User AssignedUser { get; set; }

        //Use the property CreatedBy as the foreign key for the navigation property CreatedByUser
        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; }

        //Use the property UpdatedBy as the foreign key for the navigation property UpdatedBy
        [ForeignKey("UpdatedBy")]
        public User UpdatedByUser { get; set; }
    }
}