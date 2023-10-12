using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;

namespace Proyecto_Pymes.Controllers
{
    public class SectorsController : Controller
    {
        private readonly DbPymesContext _context;

        public SectorsController(DbPymesContext context)
        {
            _context = context;
        }

        // GET: Sectors
        public async Task<IActionResult> Index()
        {
            var dbPymesContext = _context.Sectors.Include(s => s.IdWareHouseNavigation).Where(s => s.Status == 1);
            return View(await dbPymesContext.ToListAsync());
        }

        // GET: Sectors/Details/5
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

        // GET: Sectors/Create
        public IActionResult Create()
        {
            ViewData["IdWareHouse"] = new SelectList(_context.Warehouses.Where(w => w.Status == 1), "Id", "Name");
            return View();
        }

        // POST: Sectors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(Sector sector)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        string userID = HttpContext.Session.GetString("UserID");
                        var ultimoIdInsertado = _context.Sectors
                            .OrderByDescending(s => s.Id)
                            .Select(s => s.Id)
                            .FirstOrDefault();

                        sector.Id = (short)(ultimoIdInsertado + 1);
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


        // GET: Sectors/Edit/5
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
            ViewData["IdWareHouse"] = new SelectList(_context.Warehouses.Where(s => s.Status == 1), "Id", "Name", sector.IdWareHouse);
            return View(sector);
        }

        // POST: Sectors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(Sector sector)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //para la tabla Productor
                        string sql1 = @"UPDATE Sector SET name = @name, capacityMax = @capacity, idWareHouse=@ware,userID =@userId WHERE id = @id;";
                        // Crea objetos SqlParameter para los parámetros de la consulta
                        SqlParameter[] parameters1 = new SqlParameter[]
                        {
                            new SqlParameter("@name", sector.Name),
                            new SqlParameter("@capacity", sector.CapacityMax),
                            new SqlParameter("@ware", sector.IdWareHouse),
                            new SqlParameter("@userId", sector.UserId),
                            new SqlParameter("@id", sector.Id),
                        };
                        int affectedRows1 = await _context.Database.ExecuteSqlRawAsync(sql1, parameters1);

                        if (affectedRows1 > 0)
                        {
                            // Si todo ha ido bien, realiza el commit de la transacción
                            transaction.Commit();
                            TempData["ShowModal"] = true;
                        }

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



        // GET: Sectors/Delete/5
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
        public IActionResult UpdateShowModalDelete(bool value)
        {
            TempData["ShowModalDelete"] = value;
            return Json(new { success = true });
        }


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
