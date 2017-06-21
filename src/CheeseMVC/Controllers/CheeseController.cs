using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private CheeseDbContext context;

        public CheeseController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            // ensures that when each cheese object is retrieved, its Category is also retrieved
            IList<Cheese> cheeses = context.Cheeses.Include(c => c.Category).ToList();

            return View(cheeses);
        }

        public IActionResult Add()
        {
            AddCheeseViewModel addCheeseViewModel = new AddCheeseViewModel(context.Categories.ToList());
            return View(addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                CheeseCategory newCheeseCategory =
                        context.Categories.Single(c => c.ID == addCheeseViewModel.CategoryID);

                // Add the new cheese to my existing cheeses
                Cheese newCheese = new Cheese
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory,
                };

                context.Cheeses.Add(newCheese);
                context.SaveChanges();

                return Redirect("/Cheese");
            }

            return View(addCheeseViewModel);
        }

        public IActionResult Remove()
        {
            ViewBag.title = "Remove Cheeses";
            ViewBag.cheeses = context.Cheeses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                Cheese theCheese = context.Cheeses.Single(c => c.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }

            context.SaveChanges();

            return Redirect("/");
        }

        public IActionResult Edit(int cheeseId)
        {
            Cheese theCheese = context.Cheeses.Single(p => p.ID == cheeseId);

            AddEditCheeseViewModel editCheeseViewModel = new AddEditCheeseViewModel(context.Categories.ToList());
            editCheeseViewModel.Name = theCheese.Name;
            editCheeseViewModel.Description = theCheese.Description;
            editCheeseViewModel.cheeseId = theCheese.ID;
            
            return View(editCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Edit(AddEditCheeseViewModel editCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                Cheese theCheese = context.Cheeses.Single(p => p.ID == editCheeseViewModel.cheeseId);

                theCheese.Name = editCheeseViewModel.Name;
                theCheese.Description = editCheeseViewModel.Description;
                theCheese.CategoryID = editCheeseViewModel.CategoryID;

                context.SaveChanges();

                return Redirect("/Cheese");
            }

            return View(editCheeseViewModel);
        }
    }
}
