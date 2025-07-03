using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        //This is navigation property, it will be convinient when we go to
        public List<User> Users { get; set; } // Navigation property
    }
}
