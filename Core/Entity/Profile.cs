using Core.Entity.Abstractions;
using Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity
{
    public class Profile : BaseEntity
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public int Age { get; set; }
        public string AboutMe { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string AvatarName { get; set; }
        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public DateTime RegistrationDate { get; set; } = DateTime.Now.Date;
        public RoleEnum Role { get; set; } = RoleEnum.DefaultUser;
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
