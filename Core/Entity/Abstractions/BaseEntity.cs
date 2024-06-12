
using System.ComponentModel.DataAnnotations;

namespace Core.Entity.Abstractions
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        //public override bool Equals(object obj)
        //{
        //    BaseEntity o1 = obj as BaseEntity;

        //    return Id == o1.Id;
        //}
    }
}
