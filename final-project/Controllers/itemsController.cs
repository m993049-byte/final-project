using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using final_project.Data;
using final_project.Models;


namespace final_project.Controllers
{
    public class itemsController : Controller
    {
        private readonly final_projectContext _context;

        public itemsController(final_projectContext context)
        {
            _context = context;
        }

        // GET: items
        public async Task<IActionResult> Index()
        {


            return View(await _context.items.ToListAsync());
        }

        // GET: items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // GET: items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,description,price,discount,category,quantity,imgfile")] items items)
        {
            if (ModelState.IsValid)
            {
                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(items);
        }

        // GET: items/Edit/5
        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Role")?.ToLower() != "admin")
                return RedirectToAction("login", "usersaccounts");

            if (id == null) return NotFound();

            var item = await _context.items.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }


        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,description,price,discount,category,quantity,imgfile")] items formItem)
        {
            if (HttpContext.Session.GetString("Role")?.ToLower() != "admin")
                return RedirectToAction("login", "usersaccounts");

            if (id != formItem.Id) return NotFound();

            // (اختياري) لو عندك Required قوي ويسبب لك مشاكل، خلّك على هذا:
            if (!ModelState.IsValid)
                return View(formItem);

            // ✅ تحديث كامل
            _context.Update(formItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OrderItems(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var itemsList = await _context.orders
                .Where(i => i.Id == id)
                .ToListAsync();

            if (itemsList == null || itemsList.Count == 0)
            {
                return NotFound();
            }



            return View(itemsList);
        }
        private bool ItemExists(int id)
        {
            return _context.items.Any(e => e.Id == id);
        }

    
       
       

        // GET: items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var items = await _context.items.FindAsync(id);
            if (items != null)
            {
                _context.items.Remove(items);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool itemsExists(int id)
        {
            return _context.items.Any(e => e.Id == id);
        }



        public async Task<IActionResult> Dashboard()
        {
            var cats = await _context.items
                .GroupBy(i => i.category)
                .Select(g => new CategoryStat
                {
                    Category = g.Key,
                    Count = g.Count(),
                    ImageUrl = g
                        .Where(x => x.imgfile != null && x.imgfile != "")
                        .Select(x => x.imgfile)
                        .FirstOrDefault()
                })
                .OrderBy(c => c.Category)
                .ToListAsync();

            ViewData["totalItems"] = await _context.items.CountAsync();

            ViewData["soldQty"] = await _context.orderline
                .SumAsync(o => (int?)o.itemquant) ?? 0;

            return View(cats);
        }

        public async Task<IActionResult> list()
        {

            if (HttpContext.Session.GetString("Role") != "admin")
            {
                return RedirectToAction("Login", "usersaccounts");
            }


            return View(await _context.items.ToListAsync());
        }



    }
}