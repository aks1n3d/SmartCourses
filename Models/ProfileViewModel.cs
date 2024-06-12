using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCourses.Models
{
    public class ProfileViewModel
    {
        public User Data { get; set; }
        public ICollection<Course> AuthorCourses { get; set; }
    }
}
