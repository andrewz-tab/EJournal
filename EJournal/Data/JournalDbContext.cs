using EJournal.Models;
using Microsoft.EntityFrameworkCore;

namespace EJournal.Data
{
    public class JournalDbContext : DbContext
    {
        public JournalDbContext(DbContextOptions<JournalDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<Discipline> Disciplines { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<Mark> Marks { get; set; } = null!;
        public DbSet<PersonalData> PersonalDatas { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<TypeUser> TypeUsers { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasIndex(a => a.EMail).IsUnique();
            modelBuilder.Entity<Class>().HasIndex(c => new { c.Number, c.Liter }).IsUnique();
            modelBuilder.Entity<Discipline>().HasIndex(d => new { d.SubjectKey, d.EmployeeKey, d.ClassKey }).IsUnique();
            modelBuilder.Entity<Mark>().ToTable(m => m.HasCheckConstraint("Value", "Value >= -1 AND Value <= 5"));
            modelBuilder.Entity<PersonalData>().HasIndex(pd => pd.SNILS).IsUnique();
            modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
            modelBuilder.Entity<Subject>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<TypeUser>().HasIndex(tu => tu.Name).IsUnique();


            modelBuilder.Entity<Account>()
                .HasOne(a => a.TypeUser)
                .WithMany(tu => tu.Accounts)
                .HasForeignKey(a => a.TypeUserKey);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Employee)
                .WithMany(e => e.Classes)
                .HasForeignKey(c => c.EmployeeKey).OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Discipline>()
                .HasOne(d => d.Class)
                .WithMany(c => c.Disciplines)
                .HasForeignKey(d => d.ClassKey);
            modelBuilder.Entity<Discipline>()
                .HasOne(d => d.Employee)
                .WithMany(e => e.Disciplines)
                .HasForeignKey(d => d.EmployeeKey);
            modelBuilder.Entity<Discipline>()
                .HasOne(d => d.Subject)
                .WithMany(s => s.Disciplines)
                .HasForeignKey(d => d.SubjectKey);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Account)
                .WithOne(a => a.Employee)
                .HasForeignKey<Employee>(e => e.AccountKey).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Discipline)
                .WithMany(d => d.Lessons)
                .HasForeignKey(l => l.DisciplineKey);

            modelBuilder.Entity<Mark>()
                .HasOne(m => m.Lesson)
                .WithMany(l => l.Marks)
                .HasForeignKey(m => m.LessonKey);
            modelBuilder.Entity<Mark>()
                .HasOne(m => m.Student)
                .WithMany(s => s.Marks)
                .HasForeignKey(m => m.StudentKey);

            modelBuilder.Entity<PersonalData>()
                .HasOne(pd => pd.Account)
                .WithOne(a => a.PersonalData)
                .HasForeignKey<PersonalData>(pd => pd.AccountKey).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Employees)
                .WithMany(e => e.Roles)
                .UsingEntity(j => j.ToTable("EmployeeRole"));

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassKey);
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Account)
                .WithOne(a => a.Student)
                .HasForeignKey<Student>(s => s.AccountKey).OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Учитель" },
                new Role { Id = 2, Name = "Завуч" },
                new Role { Id = 3, Name = "Администратор" }
                );
            modelBuilder.Entity<TypeUser>().HasData(
                new TypeUser { Id = 1, Name = "Ученик" },
                new TypeUser { Id = 2, Name = "Сотрудник"}
                );
        }

    }
}
