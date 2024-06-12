using Core.Entity.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Core.Entity
{
    public class Course : BaseEntity
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        [Required]
        [StringLength(2000)]
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public string AvatarName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now.Date;
        public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
        public ICollection<VideoMaterial> Videos { get; set; } = new List<VideoMaterial>();
        public ICollection<BookMaterial> Books { get; set; } = new List<BookMaterial>();
        public ICollection<ArticleMaterial> Articles { get; set; } = new List<ArticleMaterial>();
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public bool IsPersonal { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        public int Progress { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

        public int GetProgres()
        {
            var materialsCount = Articles.Count + Books.Count + Videos.Count;
            if (materialsCount > 0)
            {
                int progress = (Articles.Where(a => a.IsCompleted).Count() +
                    Books.Where(b => b.IsCompleted).Count() +
                    Videos.Where(v => v.IsCompleted).Count()) *
                    100 / materialsCount;

                if (progress == 100)
                {
                    IsCompleted = true;
                }

                return progress;
            }

            IsCompleted = true;
            Progress = 100;

            return 100;
        }
    }
}
