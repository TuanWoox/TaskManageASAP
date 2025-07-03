using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Column("username")]
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Column("email")]
        [Required]
        [MaxLength(200)]
        public string Email { get; set; }

        [Column("passwordhashed")]
        [Required]
        public string PasswordHash { get; set; }

        [Column("roleid")]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }


        //Navigation back to task

        //This is used to navigate from user to user's list of assigned tasks
        [InverseProperty("AssignedUser")]
        public ICollection<Task> AssignedTasks { get; set; }

        //This is used to navigate from user to user's list of created tasks
        [InverseProperty("CreatedByUser")]
        public ICollection<Task> CreatedTasks { get; set; }


        //This is used to navigate from user to user's list of update task
        [InverseProperty("UpdatedByUser")]
        public ICollection<Task> UpdatedTasks { get; set; }

        
    }
}
