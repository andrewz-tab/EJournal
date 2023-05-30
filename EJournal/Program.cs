using EJournal.Data;
using EJournal.Repository;
using EJournal.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EJournal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<JournalDbContext>(options =>
            {
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 31)));
            });
            builder.Services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IClassRepository, ClassRepository>();
            builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
            builder.Services.AddScoped<IDisciplineRepository, DisciplineRepository>();
            builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<ILessonRepository, LessonRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/");
            app.MapControllerRoute(
                name: "classes",
                pattern: "{controller=Classes}/{action=Index}/");
            app.MapControllerRoute(
                name: "employees",
                pattern: "{controller=Employees}/{action=Index}/");
            app.MapControllerRoute(
                name: "employees",
                pattern: "{controller=Employees}/{action=Upsert}/{id?}");
            app.Run();
        }
    }
}