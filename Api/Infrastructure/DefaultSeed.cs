using Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Infrastructure
{
    public static class DefaultSeed
    {
        public static void Initialize(DatabaseContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            var now = DateTime.Now;

            var user = new User
            {
                CreateDate = now,
                MutationDate = now,
                Email = "Test@gmail.com",
                Password = "123",
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
                                Name = "surfboard",
                                Description = "is mooi",
                                Price = "bieden",
                                ImageUrl = "url",
                                Url = "url"
                            }
                        }
                    }
                }
            };

            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
