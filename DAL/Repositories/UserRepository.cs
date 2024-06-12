using Core.Entity;
using DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository : IBaseRepository<User>
    {
        private AppDbContext _dbContext { get; set; }

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(User entity)
        {
            await _dbContext.Users.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            _dbContext.Users.Remove(await Get(id));
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<User> Get(int id)
        {
            return await _dbContext.Users.Include(u => u.Profile).Include(u => u.Profile.Courses).Include(u => u.Profile.Skills).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ICollection<User>> GetAll()
        {
            return await _dbContext.Users.Include(u => u.Profile).Include(u => u.Profile.Courses).Include(u => u.Profile.Skills).ToListAsync();
        }

        public async Task<bool> Update(User entity)
        {
            var user = await Get(entity.Id);

            user.Login = entity.Login;
            user.Password = entity.Password;
            user.Profile.FirstName = entity.Profile.FirstName;
            user.Profile.SecondName = entity.Profile.SecondName;
            user.Profile.AboutMe = entity.Profile.AboutMe;
            user.Profile.Age = entity.Profile.Age;
            user.Profile.AvatarName = entity.Profile.AvatarName;

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
