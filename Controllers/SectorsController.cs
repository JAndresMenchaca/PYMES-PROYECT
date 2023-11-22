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
    public class SectorsController : Controller
    {
        
        private readonly DbPymesContext _context;

        public SectorsController(DbPymesContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Index()
        {
            try{
                int UserId = int.Parse(HttpContext.Session.GetString("UserID"));
            var dbPymesContext = _context.Sectors.Include(s => s.IdWareHouseNavigation).Where(s => s.Status == 1 && s.UserId == UserId);
            return View(await dbPymesContext.ToListAsync());

            }catch{
                return RedirectToAction("Privacy", "Home");
            }
        
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null || _context.Sectors == null)
            {
                return NotFound();
            }

            var sector = await _context.Sectors
                .Include(s => s.IdWareHouseNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sector == null)
            {
                return NotFound();
            }

            return View(sector);
        }

        [Authorize(Roles = "Productor")]
        public IActionResult Create()
        {
            int UserId = int.Parse(HttpContext.Session.GetString("UserID"));
            ViewData["IdWareHouse"] = new SelectList(_context.Warehouses.Where(w => w.Status == 1 && w.UserId == UserId), "Id", "Name");
            return View();
        }

        
        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create(Sector sector)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //var ultimoIdInsertado = _context.Sectors
                        //    .OrderByDescending(s => s.Id)
                        //    .Select(s => s.Id)
                        //    .FirstOrDefault();

                        //sector.Id = (short)(ultimoIdInsertado + 1);
                        // sector.UserId = int.Parse(userID);
                        string userID = HttpContext.Session.GetString("UserID");
                        sector.UserId = int.Parse(userID);
                        _context.Sectors.Add(sector);
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
            return View();
        }


        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null || _context.Sectors == null)
            {
                return NotFound();
            }

            var sector = await _context.Sectors.FindAsync(id);
            if (sector == null)
            {
                return NotFound();
            }
            int UserId = int.Parse(HttpContext.Session.GetString("UserID"));
            ViewData["IdWareHouse"] = new SelectList(_context.Warehouses.Where(s => s.Status == 1 && s.UserId == UserId), "Id", "Name", sector.IdWareHouse);
            return View(sector);
        }

       
        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(Sector sector)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                      
                            sector.LastUpdate = DateTime.Now;
                            _context.Sectors.Update(sector);
                            await _context.SaveChangesAsync();
                            // Si todo ha ido bien, realiza el commit de la transacción
                            transaction.Commit();
                            TempData["ShowModal"] = true;
                      

                        return View(); // Redirecciona a la página de lista después de la edición
                    }
                    else
                    {
                        // Si hay errores en el modelo, no se realizará el commit de la transacción
                        return View(sector);
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                    return View(sector);
                }
            }
        }



        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Delete(short? id)
        {
            var sector = await _context.Sectors.FirstOrDefaultAsync(m => m.Id == id);
            if (sector == null)
            {
                return NotFound();
            }
            TempData["TownShipToDelete"] = (int)sector.Id;
            TempData["Name"] = sector.Name;
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
            var sector = await _context.Sectors.FindAsync(id);
            if (sector == null)
            {
                return NotFound();
            }
            sector.Status = 0; // O es valor Inactivo          
            _context.Sectors.Update(sector);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SectorExists(short id)
        {
          return (_context.Sectors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
