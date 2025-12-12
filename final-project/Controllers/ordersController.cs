using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using final_project.Data;
using final_project.Models;
using System.Text.Json;

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
        public async Task<IActionResult> cartadd(int itmId, int quantity)
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart"); if (sessionString is not null)
            {
                Bitm = JsonSerializer.Deserialize<List<BuyItem>>(sessionString);
            }
            var item = await _context.items.FromSqlRaw("select * from items	where Id= '" + itmId
+ "'	").FirstOrDefaultAsync();
            Bitm.Add(new BuyItem
            {
               name = item.name,
                price = item.price,
                quant = quantity
            });

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(Bitm));
            return RedirectToAction("CartBuy");
        }

        public async Task<IActionResult> CartBuy()
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart"); if (sessionString is not null)
            {
                Bitm = JsonSerializer.Deserialize<List<BuyItem>>(sessionString);
            }
            return View(Bitm);
        }

        public async Task<IActionResult> Buy()
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null)
            {
                Bitm = JsonSerializer.Deserialize<List<BuyItem>>(sessionString);
            }

            string ctname = HttpContext.Session.GetString("Name");
            orders itmorder = new orders();
            itmorder.total = 0;
            itmorder.custname = ctname;
            itmorder.orderdate = DateTime.Today;
            _context.orders.Add(itmorder);
            await _context.SaveChangesAsync();
            var tord = await _context.orders.FromSqlRaw("select * from bookorder where custname = '" + ctname + "' ").OrderByDescending(e => e.Id).FirstOrDefaultAsync();
            int ordid = tord.Id;
            decimal tot = 0;
            foreach (var itm in Bitm.ToList())
            {
                orderline oline = new orderline();
                oline.orderid = ordid;
                oline.itemname = itm.name;
                oline.itemquant = itm.quant;
                oline.itemprice = itm.price;
                _context.orderline.Add(oline);
                await _context.SaveChangesAsync();

                var itmm = await _context.items.FromSqlRaw("select * from items where name= '" + itm.name + "' ").FirstOrDefaultAsync();
                itmm.quantity = itmm.quantity - itm.quant;

                _context.Update(itmm);
                await _context.SaveChangesAsync();

                tot = tot + (itm.quant * itm.price);
            }
            tord.total = Convert.ToInt16(tot);
            _context.Update(tord);
            await _context.SaveChangesAsync();
            ViewData["Message"] = "Thank you See you again";
            Bitm = new List<BuyItem>();
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(Bitm));
            return RedirectToAction("MyOrder");
        }

        public async Task<IActionResult> MyOrder()
        {
            string ctname = HttpContext.Session.GetString("Name"); return View(await _context.orders.FromSqlRaw("select * from orders where custname = '" + ctname + "' ").ToListAsync());
        }

        public async Task<IActionResult> orderline(int? orid)
        {
            var buyitm = await _context.orderline.FromSqlRaw("select * from orderline where orderid = '" + orid + "' ").ToListAsync();
            return View(buyitm);
        }
        // GET: orders
        //public async Task<IActionResult> Index()
        //{
        //    var orItems = await _context.orders.FromSqlRaw("SELECT custname, SUM(total) as total FROM orders GROUP BY custname  ").ToListAsync();

        //    // تمرير البيانات إلى الـ View
        //    return View(orItems);
        //}

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
