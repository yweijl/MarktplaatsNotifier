using Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Infrastructure
{
    public static class DefaultSeed
    {
        public async static Task InitializeAsync(DatabaseContext context, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            var now = DateTime.Now;

            var user = new User
            {
                UserName = "yos",
                Email = "Test@gmail.com",
                Queries = new List<Query>
                {
                    new Query
                    {
                        Name = "Surfboards",
                        MutationDate = now,
                        CreateDate = now,
                        Interval = 1,
                        Url = "www.mp.nl/surf",
                        Advertisements = new List<Advertisement>
                        {
                            new Advertisement
                            {
                                CreateDate = now,
                                MutationDate = now,
                                Title = "surfboard",
                                Description = "is mooi",
                                Price = "bieden",
                                ImageUrl = "url",
                                Url = "url"
                            }
                        }
                    }
                }
            };

            await roleManager.CreateAsync(new Role { Name = "User", NormalizedName = "User", ConcurrencyStamp = Guid.NewGuid().ToString() });
            await userManager.CreateAsync(user, "Qwerty1!");
            await userManager.AddToRoleAsync(user, "User");

            context.SaveChanges();
        }
    }
}
