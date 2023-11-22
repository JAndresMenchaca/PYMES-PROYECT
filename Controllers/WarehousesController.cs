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
    public class WarehousesController : Controller
    {
        private readonly DbPymesContext _context;

        public WarehousesController(DbPymesContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Index()
        {
            try{
                int UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                return _context.Warehouses != null ?
                View(await _context.Warehouses.Where(s => s.Status == 1 && s.UserId == UserId).ToListAsync()) :
                Problem("Entity set 'DbPymesContext.Categories'  is null.");
            }catch{
                return RedirectToAction("Privacy", "Home");
            }   
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null || _context.Warehouses == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        [Authorize(Roles = "Productor")]
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,CapacityMax,Status,RegisterDate,LastUpdate,UserId")] Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                warehouse.UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                TempData["ShowModal"] = true;
            }
            return View(warehouse);
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null || _context.Warehouses == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

       
        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(Warehouse warehouse)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                            warehouse.LastUpdate = DateTime.Now;
                            _context.Warehouses.Update(warehouse);
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
                    return View(warehouse);
                }
                return View(warehouse);
            }
        }



        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Delete(short? id)
        {
            var wareh = await _context.Warehouses.FirstOrDefaultAsync(m => m.Id == id);
            if (wareh == null)
            {
                return NotFound();
            }
            TempData["TownShipToDelete"] = (int)wareh.Id;
            TempData["Name"] = wareh.Name;
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
        public async Task<IActionResult> ConfirmDelete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var wareh = await _context.Warehouses.FindAsync(id);
            if (wareh == null)
            {
                return NotFound();
            }
            wareh.Status = 0; // O es valor Inactivo          
            _context.Warehouses.Update(wareh);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        private bool WarehouseExists(short id)
        {
          return (_context.Warehouses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
