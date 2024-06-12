using Core.Response.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Servises.Abstractions
{
    public interface IService<T>
    {
        Task<IBaseResponse<ICollection<T>>> GetAll();
        Task<IBaseResponse<T>> Get(int id);
        Task<IBaseResponse<bool>> Add(T entity);
        Task<IBaseResponse<bool>> Delete(int id);
        Task<IBaseResponse<bool>> Update(T entity);
    }
}
