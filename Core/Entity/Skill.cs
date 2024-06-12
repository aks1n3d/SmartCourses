using Core.Entity.Abstractions;
using Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entity
{
    public class Skill : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public SkillLevelEnum Level { get; set; }
        public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
