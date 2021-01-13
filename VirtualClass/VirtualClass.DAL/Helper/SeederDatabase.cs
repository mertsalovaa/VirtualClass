using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VirtualClass.DAL.Entities;

namespace VirtualClass.DAL.Helper
{
    public class SeederDatabase
    {
        public static void SeedData(IServiceProvider services,
          IConfiguration config)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<UserManager<DbUser>>();
                var managerRole = scope.ServiceProvider.GetRequiredService<RoleManager<DbRole>>();
                var context = scope.ServiceProvider.GetRequiredService<EFContext>();
                SeedUsers(manager, managerRole, context);
            }
        }

        private static void SeedUsers(UserManager<DbUser> userManager, RoleManager<DbRole> roleManager, EFContext _context)
        {
            var roleName = "Admin";
            if (roleManager.FindByNameAsync(roleName).Result == null)
            {
                #region seedRoles
                var resAdmin = roleManager.CreateAsync(new DbRole
                {
                    Name = "Admin"
                }).Result;

                var resTeacher = roleManager.CreateAsync(new DbRole
                {
                    Name = "Teacher"
                }).Result;
                var resStudent = roleManager.CreateAsync(new DbRole
                {
                    Name = "Student"
                }).Result;


                string admin_ = "admin@gmail.com";
                var admin = new DbUser
                {
                    Email = admin_,
                    UserName = admin_,
                    PhoneNumber = "+380(056)7726084",
                    FullName = "Admin",
                    BirthDate = new DateTime(1997, 10, 13),
                    Image = "https://www.lansweeper.com/wp-content/uploads/2018/05/ASSET-USER-ADMIN.png",
                    Address = "Ukraine"
                };
                string teacher_ = "teacher@gmail.com";
                var teacher = new DbUser
                {
                    Email = teacher_,
                    UserName = teacher_,
                    PhoneNumber = "+380(0692)248650",
                    FullName = "Teacher",
                    BirthDate = new DateTime(1998, 03, 25),
                    Image = "https://i.pinimg.com/originals/55/69/55/5569554b4d8a9bb11965949e1af08dbf.png",
                    Address = "Ukraine"
                };
                string student_ = "student@gmail.com";
                var student = new DbUser
                {
                    Email = student_,
                    UserName = student_,
                    PhoneNumber = "+380(048)557289",
                    FullName = "Student",
                    BirthDate = new DateTime(1997, 04, 20),
                    Image = "https://cdn.iconscout.com/icon/premium/png-256-thumb/student-330-732103.png",
                    Address = "Ukraine"
                };

                var resultAdmin = userManager.CreateAsync(admin, "Qwerty1-").Result;
                resultAdmin = userManager.AddToRoleAsync(admin, "Admin").Result;

                var resultTeacher = userManager.CreateAsync(teacher, "Qwerty1-").Result;
                resultTeacher = userManager.AddToRoleAsync(teacher, "Teacher").Result;

                var resultStudent = userManager.CreateAsync(student, "Qwerty1-").Result;
                resultStudent = userManager.AddToRoleAsync(student, "Student").Result;

                #endregion
            }
        }
    }
}
