using Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            //Database.EnsureDeleted(); // peresozdanie bd
            //Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<VideoMaterial> Videos { get; set; }
        public DbSet<ArticleMaterial> Articles { get; set; }
        public DbSet<BookMaterial> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<Profile>(p => p.UserId);

            modelBuilder
                .Entity<Course>()
                .HasOne(cour => cour.Category)
                .WithMany(cat => cat.Courses);

            modelBuilder.Entity<Course>()
                 .HasMany(c => c.Profiles)
                 .WithMany(p => p.Courses)
                 .UsingEntity(j => j.ToTable("CoursesProfiles"));

            modelBuilder.Entity<Course>()
                 .HasMany(c => c.Skills)
                 .WithMany(s => s.Courses)
                 .UsingEntity(j => j.ToTable("CoursesSkills"));

            modelBuilder.Entity<Course>()
                 .HasMany(c => c.Videos)
                 .WithMany(v => v.Courses)
                 .UsingEntity(j => j.ToTable("CoursesVideos"));

            modelBuilder.Entity<Course>()
                 .HasMany(c => c.Books)
                 .WithMany(b => b.Courses)
                 .UsingEntity(j => j.ToTable("CoursesBooks"));

            modelBuilder.Entity<Course>()
                 .HasMany(c => c.Articles)
                 .WithMany(a => a.Courses)
                 .UsingEntity(j => j.ToTable("CoursesArticles"));

            modelBuilder.Entity<Profile>()
                 .HasMany(c => c.Skills)
                 .WithMany(s => s.Profiles)
                 .UsingEntity(j => j.ToTable("ProfilesSkills"));
        }
    }
}
