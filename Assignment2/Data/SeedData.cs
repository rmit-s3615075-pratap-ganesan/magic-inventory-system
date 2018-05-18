using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Assignment2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Assignment2.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // DoSomething(serviceProvider);

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { Constants.RetailRole, Constants.WholeSaleRole , Constants.CustomerRole};
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = role });
                }
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            await EnsureUserHasRole(userManager, "pavan.wadhe@gmail.com", Constants.CustomerRole);
            await EnsureUserHasRole(userManager, "s3635887@student.rmit.edu.au",Constants.WholeSaleRole);
            await EnsureUserHasRole(userManager, "nirajbohra@gmail.com",Constants.WholeSaleRole);
            await EnsureUserHasRole(userManager, "s3578115@student.rmit.edu.au", Constants.RetailRole);
            await EnsureUserHasRole(userManager, "13030121038@sicsr.ac.in", Constants.RetailRole);
            await EnsureUserHasStoreID(userManager, "s3578115@student.rmit.edu.au", Constants.MelbourneCBD);
            await EnsureUserHasStoreID(userManager, "13030121038@sicsr.ac.in", 3);
        }

        private static void DoSomething(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any products.
                if (context.Products.Any())
                {
                    return; // DB has been seeded.
                }

                var products = new[]
                {
                    new Product
                    {
                        Name = "Rabbit",
                        Price = 10.45M
                    },
                    new Product
                    {
                        Name = "Hat",
                        Price = 20.32M
                    },
                    new Product
                    {
                        Name = "Svengali Deck",
                        Price = 13.21M
                            
                    },
                    new Product
                    {
                        Name = "Floating Hankerchief",
                        Price = 15.00M
                    },
                    new Product
                    {
                        Name = "Wand",
                        Price = 53.12M
                    },
                    new Product
                    {
                        Name = "Broomstick",
                        Price = 33.99M
                    },
                    new Product
                    {
                        Name = "Bang Gun",
                        Price = 35.29M
                    },
                    new Product
                    {
                        Name = "Cloak of Invisibility",
                        Price = 23.33M
                    },
                    new Product
                    {
                        Name = "Elder Wand",
                        Price = 32.33M
                    },
                    new Product
                    {
                        Name = "Resurrection Stone",
                        Price = 12.22M
                    }
                };

                context.Products.AddRange(products);

                var i = 0;
                context.OwnerInventory.AddRange(
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 20
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 50
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 100
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 150
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 40
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 10
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 5
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 0
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 0
                    },
                    new OwnerInventory
                    {
                        Product = products[i],
                        StockLevel = 0
                    }
                );

                i = 0;
                var stores = new[]
                {
                    new Store
                    {
                        Name = "Melbourne CBD",
                        StoreInventory =
                        {
                            new StoreInventory
                            {
                                Product = products[i++],
                                StockLevel = 15
                            },
                            new StoreInventory
                            {
                                Product = products[i++],
                                StockLevel = 10
                            },
                            new StoreInventory
                            {
                                Product = products[i++],
                                StockLevel = 5
                            },
                            new StoreInventory
                            {
                                Product = products[i++],
                                StockLevel = 5
                            },
                            new StoreInventory
                            {
                                Product = products[i++],
                                StockLevel = 5
                            },
                            new StoreInventory
                            {
                                Product = products[i++],
                                StockLevel = 5
                            },
                            new StoreInventory
                            {
                                Product = products[i++],
                                StockLevel = 5
                            },
                            new StoreInventory
                            {
                                Product = products[i++],
                                StockLevel = 1
                            },
                            new StoreInventory
                            {
                                Product = products[i++],
                                StockLevel = 1
                            },
                            new StoreInventory
                            {
                                Product = products[i],
                                StockLevel = 1
                            },
                        }
                    },
                    new Store
                    {
                        Name = "North Melbourne",
                        StoreInventory =
                        {
                            new StoreInventory
                            {
                                Product = products[0],
                                StockLevel = 5
                            }
                        }
                    },
                    new Store
                    {
                        Name = "East Melbourne",
                        StoreInventory =
                        {
                            new StoreInventory
                            {
                                Product = products[1],
                                StockLevel = 5
                            }
                        }
                    },
                    new Store
                    {
                        Name = "South Melbourne",
                        StoreInventory =
                        {
                            new StoreInventory
                            {
                                Product = products[2],
                                StockLevel = 5
                            }
                        }
                    },
                    new Store
                    {
                        Name = "West Melbourne"
                    }
                };

                context.Stores.AddRange(stores);

                context.SaveChanges();
            }
        }

        private static async Task EnsureUserHasRole(UserManager<ApplicationUser> userManager, string userName, string role)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null && !await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }

        private static async Task EnsureUserHasStoreID(
            UserManager<ApplicationUser> userManager, string userName, int storeID)
        {
            var user = await userManager.FindByNameAsync(userName);
            user.StoreID = storeID;
            await userManager.UpdateAsync(user);
        }
    }
}
