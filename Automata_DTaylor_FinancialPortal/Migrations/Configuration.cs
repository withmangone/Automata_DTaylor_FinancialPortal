namespace Automata_DTaylor_FinancialPortal.Migrations
{
    using Automata_DTaylor_FinancialPortal.Enumerations;
    using Automata_DTaylor_FinancialPortal.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var roleManager = new RoleManager<IdentityRole>(
                            new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "HeadOfHouse"))
            {
                roleManager.Create(new IdentityRole { Name = "HeadOfHouse" });
            }

            if (!context.Roles.Any(r => r.Name == "Resident"))
            {
                roleManager.Create(new IdentityRole { Name = "Resident" });
            }

            if (!context.Roles.Any(r => r.Name == "Lobbyist"))
            {
                roleManager.Create(new IdentityRole { Name = "Lobbyist" });
            }

            //I want to write some code that'll allow me to introduce a few users
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            #region SeededLobbyAndDemoHouse
            context.Households.AddOrUpdate(
                h => h.Name,
                new Household { Name = "The Lobby", Greeting = "Welcome to the lobby!", Created = DateTimeOffset.UtcNow.ToLocalTime() },
                new Household { Name = "Demo Household", Greeting = "Welcome everyone!", Created = DateTimeOffset.UtcNow.ToLocalTime(), IsConfigured=true }
                );
            #endregion

            context.SaveChanges();
            var seedHouseId = context.Households.AsNoTracking().FirstOrDefault(h => h.Name == "Demo Household").Id;
            var seedLobbyId = context.Households.AsNoTracking().FirstOrDefault(h => h.Name == "The Lobby").Id;

            #region SeededHead
            if (!context.Users.Any(u => u.Email == "SeededHead@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    Email = "SeededHead@mailinator.com",
                    UserName = "SeededHead@mailinator.com",
                    FirstName = "Harry",
                    LastName = "Head",
                    ProfilePic = "/Avatar/Default-avatar.jpg",
                    HouseholdId = seedHouseId
                }, "Govegan1!");
            }

            var user = userManager.FindByEmail("SeededHead@mailinator.com");
            userManager.AddToRole(user.Id, "HeadOfHouse");
            #endregion

            #region SeededResident
            if (!context.Users.Any(u => u.Email == "SeededResident@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    Email = "SeededResident@mailinator.com",
                    UserName = "SeededResident@mailinator.com",
                    FirstName = "Randy",
                    LastName = "Resident",
                    HouseholdId = seedHouseId,
                    ProfilePic = "/Avatar/Default-avatar.jpg"
                }, "Govegan1!");
            }

            var userId = userManager.FindByEmail("SeededResident@mailinator.com").Id;
            userManager.AddToRole(userId, "Resident");
            #endregion

            #region SeededLobbyist
            if (!context.Users.Any(u => u.Email == "SeededLobbyist@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    Email = "SeededLobbyist@mailinator.com",
                    UserName = "SeededLobbyist@mailinator.com",
                    FirstName = "Larry",
                    LastName = "Lobbyist",
                    ProfilePic = "/Avatar/Default-avatar.jpg",
                    HouseholdId = seedLobbyId
                }, "Govegan1!");
            }

            userId = userManager.FindByEmail("SeededLobbyist@mailinator.com").Id;
            userManager.AddToRole(userId, "Lobbyist");
            #endregion

            #region BankAccount
            context.BankAccounts.AddOrUpdate(
                c => c.AccountName,
                new BankAccount { AccountName = "Primary Checking Account", AccountType = AccountType.Checking, HouseholdId = seedHouseId, StartingBalance = 1000,CurrentBalance = 1000, LowBalanceLevel = 200}
                );
            #endregion

            #region BudgetCategories
            context.BudgetCategories.AddOrUpdate(
                t => t.BudgetCategoryName,
                new BudgetCategory { BudgetCategoryName = "Bills", HouseholdId = seedHouseId, TargetAmount = 130},
                new BudgetCategory { BudgetCategoryName = "Food", HouseholdId = seedHouseId, TargetAmount = 240 },
                new BudgetCategory { BudgetCategoryName = "Car Expenses", HouseholdId = seedHouseId, TargetAmount = 120 },
                new BudgetCategory { BudgetCategoryName = "Home Expenses", HouseholdId = seedHouseId, TargetAmount = 50 },
                new BudgetCategory { BudgetCategoryName = "Recurring Subscriptions", HouseholdId = seedHouseId, TargetAmount = 35 },
                new BudgetCategory { BudgetCategoryName = "Entertainment", HouseholdId = seedHouseId, TargetAmount = 50 }
                );
            #endregion

            context.SaveChanges();
            var billsId = context.BudgetCategories.AsNoTracking().Where(h => h.BudgetCategoryName == "Bills").FirstOrDefault().Id;
            var foodId = context.BudgetCategories.AsNoTracking().Where(h => h.BudgetCategoryName == "Food").FirstOrDefault().Id;
            var carId = context.BudgetCategories.AsNoTracking().Where(h => h.BudgetCategoryName == "Car Expenses").FirstOrDefault().Id;

            #region BudgetCategoryItems
            context.BudgetCategoryItems.AddOrUpdate(
                t => t.ItemName,
                new BudgetCategoryItem { ItemName = "Gas Bill", BudgetCategoryId = billsId },
                new BudgetCategoryItem { ItemName = "Electric Bill", BudgetCategoryId = billsId },
                new BudgetCategoryItem { ItemName = "Water Bill", BudgetCategoryId = billsId },
                new BudgetCategoryItem { ItemName = "Internet/Phone Bill", BudgetCategoryId = billsId },
                new BudgetCategoryItem { ItemName = "Groceries", BudgetCategoryId = foodId },
                new BudgetCategoryItem { ItemName = "Restaurant", BudgetCategoryId = foodId },
                new BudgetCategoryItem { ItemName = "Gasoline", BudgetCategoryId = carId },
                new BudgetCategoryItem { ItemName = "Maintenance", BudgetCategoryId = carId }
                );
            #endregion

            #region SeededAdmin
            if (!context.Users.Any(u => u.Email == "wake.drew@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    Email = "wake.drew@gmail.com",
                    UserName = "wake.drew@gmail.com",
                    FirstName = "Drew",
                    LastName = "Taylor",
                    ProfilePic = "/Avatar/drew_avatar2.jpg",
                    HouseholdId = seedLobbyId
                }, "Govegan1!");
            }

            userId = userManager.FindByEmail("wake.drew@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
            #endregion
        }
    }
}
