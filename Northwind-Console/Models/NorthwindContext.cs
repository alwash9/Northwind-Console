using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace NorthwindConsole.Models
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext() : base("name=NorthwindContext") { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public void AddCategory(Category category)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            if(this.Validation(category))
            {
                logger.Info("Validation passed");

                this.Categories.Add(category);
                this.SaveChanges();
                logger.Info("Category Added");
            }
            else
            {
                Console.WriteLine("Please try again. Please ENTER to continue.");
                Console.ReadLine();
            }

        }

        public bool Validation(Category category)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


            ValidationContext vContext = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, vContext, results, true);
            if (isValid)
            {
                // check for unique name
                if (this.Categories.Any(c => c.CategoryName.ToLower() == category.CategoryName.ToLower()))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }

                if (category.Description.ToLower().Replace(" ", "") == category.CategoryName.ToLower().Replace(" ", ""))
                {
                    isValid = false;
                    results.Add(new ValidationResult("Description matches Category Name. Please make the description more detailed.", new string[] { "Description" }));
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }

                return false;
            }

            return true;
        }

    }
}

