using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCourses.Models
{
    public class AccountsModel
    {
        public ICollection<User> Data { get; set; }
        public string Search { get; set; }
    }
}
