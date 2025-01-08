using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFCoreVarianceDemo
{
    // Base Entity Class
    public abstract class Entity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Specific Entity Types
    public class User : Entity
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class Admin : User
    {
        public bool IsSuperAdmin { get; set; }
    }

    // Covariant Repository Interface
    public interface IReadRepository<out T> where T : Entity
    {
        T GetById(int id);
        IQueryable<T> GetAll();
    }

    // Contravariant Repository Interface
    public interface IWriteRepository<in T> where T : Entity
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }

    // Combined Repository Interface using Variance
    public interface IRepository<T> : 
        IReadRepository<T>, 
        IWriteRepository<T> 
        where T : Entity
    {
    }

    // Concrete Repository Implementation (Covariant)
    public class EntityReadRepository<T> : IReadRepository<T> 
        where T : Entity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public EntityReadRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }
    }

    // Concrete Repository Implementation (Contravariant)
    public class EntityWriteRepository<T> : IWriteRepository<T> 
        where T : Entity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public EntityWriteRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }

    // Full Repository Implementation
    public class EntityRepository<T> : IRepository<T> 
        where T : Entity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public EntityRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Covariant Read Methods
        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        // Contravariant Write Methods
        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }

    // DbContext for the example
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use in-memory database for demonstration
            optionsBuilder.UseInMemoryDatabase("DemoDatabase");
        }
    }

    // Demonstration Class
    public class Variancedemonstration
    {
        public static void Demonstrate()
        {
            // Setup DbContext
            using var context = new ApplicationDbContext();

            // Covariant Read Repositories
            IReadRepository<User> userReadRepo = new EntityReadRepository<User>(context);
            IReadRepository<Admin> adminReadRepo = new EntityReadRepository<Admin>(context);

            // Covariance in action - can treat Admin repo as User repo
            IReadRepository<Entity> entityReadRepo = adminReadRepo;

            // Contravariant Write Repositories
            IWriteRepository<Entity> entityWriteRepo = new EntityWriteRepository<Entity>(context);
            IWriteRepository<User> userWriteRepo = entityWriteRepo;

            // Full Repository
            var userRepo = new EntityRepository<User>(context);
            var adminRepo = new EntityRepository<Admin>(context);

            // Demonstrate usage
            var newUser = new User 
            { 
                Username = "johndoe", 
                Email = "john@example.com",
                CreatedAt = DateTime.UtcNow
            };

            var newAdmin = new Admin 
            { 
                Username = "admin", 
                Email = "admin@example.com",
                IsSuperAdmin = true,
                CreatedAt = DateTime.UtcNow
            };

            // Add entities
            userRepo.Add(newUser);
            adminRepo.Add(newAdmin);

            // Retrieve entities
            var retrievedUser = userRepo.GetById(newUser.Id);
            var retrievedAdmin = adminRepo.GetById(newAdmin.Id);

            Console.WriteLine($"User: {retrievedUser.Username}");
            Console.WriteLine($"Admin: {retrievedAdmin.Username}");
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            Variancedemonstration.Demonstrate();
        }
    }
}
