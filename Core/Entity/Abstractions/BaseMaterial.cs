using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entity.Abstractions
{
    public abstract class BaseMaterial : BaseEntity
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        [Required]
        public string Source { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now.Date;
        public ICollection<Course> Courses { get; set; }
    }
}
