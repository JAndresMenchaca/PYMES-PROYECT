using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;

namespace Proyecto_Pymes.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly DbPymesContext _context;

        public WarehousesController(DbPymesContext context)
        {
            _context = context;
        }

        // GET: Warehouses
        public async Task<IActionResult> Index()
        {
            var dbPymesContext = _context.Warehouses
           .Where(t => t.Status == 1);


            return View(await dbPymesContext.ToListAsync());
        }

        // GET: Warehouses/Details/5
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

        // GET: Warehouses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Warehouses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Warehouses/Edit/5
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

        // POST: Warehouses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(Warehouse warehouse)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //para la tabla Productor
                        warehouse.UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                        string sql1 = @"UPDATE Warehouse SET name = @name, location = @location, capacityMax = @capacity, lastUpdate = CURRENT_TIMESTAMP, userID = @userId WHERE id = @id";
                        // Crea objetos SqlParameter para los parámetros de la consulta
                        SqlParameter[] parameters1 = new SqlParameter[]
                        {
                            new SqlParameter("@name", warehouse.Name),
                            new SqlParameter("@location", warehouse.Location),
                            new SqlParameter("@capacity", warehouse.CapacityMax),
                            new SqlParameter("@id", warehouse.Id),
                            new SqlParameter("@userId", warehouse.UserId),
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
                        return View(warehouse);
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                    return View(warehouse);
                }
            }
        }



        // GET: Warehouses/Delete/5
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
