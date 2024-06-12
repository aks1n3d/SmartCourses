using Core.Entity;
using DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class CourseRepository : IBaseRepository<Course>
    {
        private AppDbContext _dbContext { get; set; }

        public CourseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(Course entity)
        {
            if (!entity.IsPersonal)
            {
                var courses = await GetAll();

                if (courses.Any(c => c.Name == entity.Name))
                {
                    return false;
                }
            }

            await _dbContext.Courses.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private void DeleteCash(Course courseToDelete)
        {
            if (courseToDelete.Videos.Count != 0)
            {
                foreach (var video in courseToDelete.Videos)
                {
                    _dbContext.Videos.Remove(video);
                }
            }

            if (courseToDelete.Books.Count != 0)
            {
                foreach (var book in courseToDelete.Books)
                {
                    _dbContext.Books.Remove(book);
                }
            }

            if (courseToDelete.Articles.Count != 0)
            {
                foreach (var article in courseToDelete.Articles)
                {
                    _dbContext.Articles.Remove(article);
                }
            }

            if (courseToDelete.Skills.Count != 0)
            {
                foreach (var skill in courseToDelete.Skills)
                {
                    _dbContext.Skills.Remove(skill);
                }
            }
        }

        public async Task<bool> Delete(int id)
        {
            var courseToDelete = await Get(id);
            _dbContext.Courses.Remove(courseToDelete);

            DeleteCash(courseToDelete);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Course> Get(int id)
        {
            return await _dbContext.Courses.Include(c => c.Category).Include(c => c.Articles)
                .Include(c => c.Books).Include(c => c.Videos).Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ICollection<Course>> GetAll()
        {
            var courses = await _dbContext.Courses.Include(c => c.Category).Include(c => c.Articles)
                .Include(c => c.Books).Include(c => c.Videos).Include(c => c.Skills).Where(c => c.IsPersonal == false).ToListAsync();

            return courses;
        }

        public async Task<bool> Update(Course entity)
        {
            if (!entity.IsPersonal)
            {
                var courses = await GetAll();

                var coursesWithout = courses.Where(c => c.Id != entity.Id).ToList();

                if (coursesWithout.Any(c => c.Name == entity.Name))
                {
                    return false;
                }
            }

            var course = await Get(entity.Id);

            //DeleteCash(course);

            course.Name = entity.Name;
            course.Description = entity.Description;
            course.AuthorId = entity.AuthorId;
            course.CategoryId = entity.CategoryId;
            course.Videos = entity.Videos;
            course.Books = entity.Books;
            course.Articles = entity.Articles;
            course.Skills = entity.Skills;
            course.AvatarName = entity.AvatarName;
            course.IsCompleted = entity.IsCompleted;
            course.Progress = entity.Progress;

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
