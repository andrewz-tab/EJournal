using EJournal.Data;
using EJournal.Repository;
using EJournal.Repository.IRepository;
using EJournal.Service.Implementations;
using EJournal.Service.Interfaces;
using EJournal.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EJournal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Configuration.AddJsonFile("appsettings.json");

            builder.Services.AddDbContext<JournalDbContext>(options =>
            {
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 31)));
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option => { option.LoginPath = "/Login"; });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(WC.PolicyOnlyForStudent, policy =>
                {
                    policy.RequireClaim(WC.TypeUser, WC.StudentUser);
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy(WC.PolicyOnlyForEmployee, policy =>
                {
                    policy.RequireClaim(WC.TypeUser, WC.EmployeeUser);
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy(WC.PolicyOnlyForAdmin, policy =>
                {
                    policy.RequireClaim(WC.TypeUser, WC.EmployeeUser);
                    policy.RequireClaim(ClaimTypes.Role, WC.AdminRole);
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy(WC.PolicyOnlyForHeadTeacher, policy =>
                {
                    policy.RequireClaim(WC.TypeUser, WC.EmployeeUser);
                    policy.RequireClaim(ClaimTypes.Role, WC.HeadTeacherRole);
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy(WC.PolicyOnlyForTeacher, policy =>
                {
                    policy.RequireClaim(WC.TypeUser, WC.EmployeeUser);
                    policy.RequireClaim(ClaimTypes.Role, WC.TeacherRole);
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy(WC.PolicyOnlyForHeadTeacherOrAdmin, policy =>
                {
                policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == ClaimTypes.Role &&
                (claim.Value == WC.AdminRole || claim.Value == WC.HeadTeacherRole)));
                    policy.RequireAuthenticatedUser();
                });
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
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IDumpService, DumpService>();


            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(10);
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });
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
            app.UseAuthentication();
			app.UseAuthorization();
			app.UseCheckAccount();
			app.UseSession();
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
            app.MapControllerRoute(
                name: "login",
                pattern: "{controller=Login}/{action=ChangePassword}/");
            app.Run();
        }
    }
}