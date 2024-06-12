using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCourses.Models
{
    public class HomeModel
    {
        public ICollection<Course> TopCourses { get; set; }
        public ICollection<User> TopUsers { get; set; }
    }
}
