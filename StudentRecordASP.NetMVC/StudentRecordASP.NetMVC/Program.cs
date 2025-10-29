using StudentRecordASP.NetMVC.Repositories;
using StudentRecordASP.NetMVC.Services;

namespace StudentRecordASP.NetMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddScoped<IStudentRepository, StudentRepositoryImplementation>();
            builder.Services.AddScoped<IUserRepository, UserRepositoryImplementation>();
            builder.Services.AddScoped<IRoleRepository, RoleRepositoryImplementation>();
            builder.Services.AddScoped<IStudentService, StudentServiceImpl>();
            builder.Services.AddScoped<IUserService, UserServiceImpl>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Account/Login");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Use Session
            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
