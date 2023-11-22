using System;
using System.Collections.Generic;
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
    public class CategoriesController : Controller
    {
        private readonly DbPymesContext _context;

        public CategoriesController(DbPymesContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Index()
        {
                try{
                    int UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                return _context.Categories != null ?
                              View(await _context.Categories.Where(s => s.Status == 1 && s.UserId == UserId).ToListAsync()) :
                              Problem("Entity set 'DbPymesContext.Categories'  is null.");  
                }catch{
                    return RedirectToAction("Privacy", "Home");
                }
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [Authorize(Roles = "Productor")]
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Status,RegisterDate,LastUpdate,UserId")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {                
                    string userID = HttpContext.Session.GetString("UserID");
                    category.UserId = int.Parse(userID);
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    TempData["ShowModal"] = true;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
            }
           
            return View(category);
        }


        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

      
        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(Category category)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        category.LastUpdate = DateTime.Now;
                        _context.Categories.Update(category);
                        await _context.SaveChangesAsync();
                            // Si todo ha ido bien, realiza el commit de la transacción
                            transaction.Commit();
                            TempData["ShowModal"] = true;
                        return View(); // Redirecciona a la página de lista después de la edición
                    }
                  
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");      
                }
                return View(category);
            }
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Delete(int? id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            TempData["TownShipToDelete"] = (int)category.Id;
            TempData["Name"] = category.Name;
            TempData["ShowModalDelete"] = true;

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Productor")]
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
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            category.Status = 0; // O es valor Inactivo          
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
