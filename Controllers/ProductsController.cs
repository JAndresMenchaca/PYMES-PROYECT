using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;

namespace Proyecto_Pymes.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly DbPymesContext _context;
        public List<Person> peopleList { get; set; }
        public ProductsController(DbPymesContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Index()
        {
            try{
                int UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                var dbPymesContext = _context.Products
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdSectorNavigation)
                .Include(p => p.IdProducerNavigation.IdNavigation)
                .Where(p => p.Status ==1 && p.IdProducer == UserId );

            return View(await dbPymesContext.ToListAsync());
            }catch{
                return RedirectToAction("Privacy", "Home");
            }
        }



        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdProducerNavigation)
                .Include(p => p.IdSectorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Productor")]
        public IActionResult Create()
        {
            int userID = int.Parse(HttpContext.Session.GetString("UserID"));
            ViewData["IdCategory"] = new SelectList(_context.Categories.Where(p => p.Status ==1 && p.UserId ==  userID), "Id", "Name");          
            ViewData["IdSector"] = new SelectList(_context.Sectors.Where(p => p.Status == 1 && p.UserId == userID), "Id", "Name");
            return View();
        }

      
        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create(Product products)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        int userID = int.Parse(HttpContext.Session.GetString("UserID"));

                        products.UserId = userID;
                        products.IdProducer = userID;
                        _context.Products.Add(products);
                        await _context.SaveChangesAsync();

                        // Commit la transacción si todo fue exitoso
                        transaction.Commit();
                        //PARA EL MODAL
                        TempData["ShowModal"] = true;
                    }
                }
                catch (Exception)
                {
                    // Si se produce una excepción, puedes realizar un rollback de la transacción
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error en la transacción.");
                }
            }
            return View(products);
        }


        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            int userID = int.Parse(HttpContext.Session.GetString("UserID"));
            ViewData["IdCategory"] = new SelectList(_context.Categories.Where(p => p.Status == 1 && p.UserId == userID), "Id", "Name");
            ViewData["IdSector"] = new SelectList(_context.Sectors.Where(p => p.Status == 1 && p.UserId == userID), "Id", "Name");

            return View(product);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(Product product)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {          
                            product.LastUpdate = DateTime.Now;
                        // Si todo ha ido bien, realiza el commit de la transacción
                        _context.Products.Update(product);
                        await _context.SaveChangesAsync();

                         transaction.Commit();
                         TempData["ShowModal"] = true;
     

                         return View(); // Redirecciona a la página de lista después de la edición
                    }
                    else
                    {
                        // Si hay errores en el modelo, no se realizará el commit de la transacción
                        return View(product);
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                    return View(product);
                }
            }
        }


        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Delete(int? id)
        {
            var products = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }
            TempData["TownShipToDelete"] = (int)products.Id;
            TempData["Name"] = products.Name;
            TempData["ShowModalDelete"] = true;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateShowModalDelete(bool value)
        {
            TempData["ShowModalDelete"] = value;
            return Json(new { success = true });
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            products.Status = 0; // O es valor Inactivo          
            _context.Products.Update(products);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
