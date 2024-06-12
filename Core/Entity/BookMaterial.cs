using Core.Entity.Abstractions;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity
{
    public class BookMaterial : BaseMaterial
    {
        public string Author { get; set; }
        public int PagesAmount { get; set; }
        public string Format { get; set; }
        public int PublishingDate { get; set; }
        [NotMapped]
        public IFormFile BookFile { get; set; }
    }
}
