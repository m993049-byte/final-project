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
    public class ordersController : Controller
    {
        private readonly final_projectContext _context;

        List<BuyItem> Bitm = new List<BuyItem>();
        public ordersController(final_projectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CatalogueBuy()
        {
            return View(await _context.items.ToListAsync());
        }
        public async Task<IActionResult> ItemBuyDetail(int? id)
        {
            var item = await _context.items.FindAsync(id);
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> cartadd(int bookId, int quantity)
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart"); if (sessionString is not null)
            {
                Bitm = JsonSerializer.Deserialize<List<items>>(sessionString);
            }
            var book = await _context.items.FromSqlRaw("select * from items	where Id= '" + bookId
+ "'	").FirstOrDefaultAsync();
            Bbks.Add(new buybook
            {
                Name = items.name,
                Price = items.price,
                quant = quantity
            });

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(Bbks));
            return RedirectToAction("CartBuy");
        }
        // GET: orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.items.ToListAsync());
        }

        // GET: orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,custname,orderdate,total")] orders orders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.items.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,custname,orderdate,total")] orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ordersExists(orders.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.items.FindAsync(id);
            if (orders != null)
            {
                _context.items.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
            return _context.items.Any(e => e.Id == id);
        }
    }
}
