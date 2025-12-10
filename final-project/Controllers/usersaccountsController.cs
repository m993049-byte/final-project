using final_project.Data;
using final_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace final_project.Controllers
{
    public class usersaccountsController : Controller
    {
        private readonly final_projectContext _context;

        public usersaccountsController(final_projectContext context)
        {
            _context = context;
        }

        // GET: usersaccounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.usersaccounts.ToListAsync());
        }

        // GET: usersaccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersaccounts = await _context.usersaccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersaccounts == null)
            {
                return NotFound();
            }

            return View(usersaccounts);
        }

        // GET: usersaccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        //login view 
        public IActionResult Login()
        {
            return View();
        }
        //login post action
        [HttpPost, ActionName("login")]
        public async Task<IActionResult> login(string na, string pa)
        {
            var ur = await _context.usersaccounts.FromSqlRaw("SELECT * FROM usersaccounts where name ='" + na + "' and  pass ='" + pa + "' ").FirstOrDefaultAsync();
            
            if (ur != null)
            {

                int id = ur.Id;
                string na1 = ur.name;
                string ro = ur.role;
                HttpContext.Session.SetString("userid", Convert.ToString(id));
                HttpContext.Session.SetString("Name", na1);
                HttpContext.Session.SetString("Role", ro);

                if (ro == "customer")
                    return RedirectToAction("customer");
                else if (ro == "admin")
                    return RedirectToAction("admin");
                else
                    return View();
            }
            else
            {
                ViewData["Message"] = "wrong user name password";
                return View();
            }
        }
        public IActionResult admin()
        {


            var role = HttpContext.Session.GetString("Role");
            var name = HttpContext.Session.GetString("Name");

            if (role != "admin")
            {
                return RedirectToAction("login", "usersaccounts");
            }

            return View();
        }
        //customer page 
        public IActionResult customer()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role == null)
            {

                return RedirectToAction("login", "usersaccounts");
            }
            return View();
        }

     

       



        // POST: usersaccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,pass,role")] usersaccounts usersaccounts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usersaccounts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usersaccounts);
        }

        // GET: usersaccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersaccounts = await _context.usersaccounts.FindAsync(id);
            if (usersaccounts == null)
            {
                return NotFound();
            }
            return View(usersaccounts);
        }

        // POST: usersaccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,pass,role")] usersaccounts usersaccounts)
        {
            if (id != usersaccounts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usersaccounts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!usersaccountsExists(usersaccounts.Id))
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
            return View(usersaccounts);
        }

        // GET: usersaccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersaccounts = await _context.usersaccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersaccounts == null)
            {
                return NotFound();
            }

            return View(usersaccounts);
        }

        // POST: usersaccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usersaccounts = await _context.usersaccounts.FindAsync(id);
            if (usersaccounts != null)
            {
                _context.usersaccounts.Remove(usersaccounts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool usersaccountsExists(int id)
        {
            return _context.usersaccounts.Any(e => e.Id == id);
        }
    
        public IActionResult Email()
        {
            return View();
        }


        //registration view

        public IActionResult registration()
        {
            return View();
        }

        //registation post

        [HttpPost]
        public async Task<IActionResult> registration(customer cli, string pass,string confpass)
        {
            // check if client name exists
            var existing = await _context.customers
                .FromSqlRaw("SELECT * FROM customer WHERE name = {0}", cli.name)
                .FirstOrDefaultAsync();
            if (pass != confpass) {

                ViewData["message"] = "the passwords don't match!";
                return View();
            
            }
            if (existing != null)
            {
                ViewData["message"] = "name already exists";
                return View();
            }

            // Insert into customer table
            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO customer (name, email, job, married, gender,location) VALUES ({0}, {1}, {2}, {3}, {4},{5})",
                cli.name, cli.email, cli.job, cli.married, cli.gender,cli.location
            );

           

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO usersaccounts (name, pass,role) VALUES ({0}, {1},{2})",
                cli.name, pass, "customer"
            );

            ViewData["success"] = "Successfully added (customer + useraccount)";
            return View();
        }



        [HttpPost]
        public IActionResult SendEmail(string email, string message)
        {
            try
            {
               
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("your-email@gmail.com", "your-email-password"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@gmail.com"),
                    Subject = "Nn ",
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                
                smtpClient.Send(mailMessage);

                return Content("DONE!");
            }
            catch (Exception ex)
            {
                return Content($"Error {ex.Message}");
            }
        }
    }
}

