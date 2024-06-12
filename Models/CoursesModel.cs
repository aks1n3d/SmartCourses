using Core.Entity;
using System.Collections.Generic;

namespace SmartCourses.Models
{
    public class CoursesModel
    {
        public ICollection<Course> Data { get; set; }
        public int Category { get; set; }
        public string Search { get; set; }
    }
}
