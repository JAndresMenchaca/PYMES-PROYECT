using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;

using System.Globalization;

namespace Proyecto_Pymes.Controllers
{
    public class TownShipsController : Controller
    {
        private readonly DbPymesContext _context;

        public TownShipsController(DbPymesContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Define la consulta SQL para seleccionar todos los municipios
            //var sql = @" select * from TownShip t ";

            // Ejecuta la consulta SQL y obtén los resultados
            // var townships = await _context.TownShips.FromSqlRaw(sql).ToListAsync();
            var dbPymesContext = _context.TownShips
             .Include(t => t.IdTownNavigation)
             .Where(t => t.Status == 1);


            return View(await dbPymesContext.ToListAsync());

        }

        public IActionResult Create()
        {
            ViewData["IdTown"] = new SelectList(_context.Towns, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TownShip townShip)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        string userID = HttpContext.Session.GetString("UserID");
                        townShip.RegisterDate = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        townShip.UserId = int.Parse(userID);
                        _context.TownShips.Add(townShip);
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

        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null || _context.TownShips == null)
            {
                return NotFound();
            }

            var townShip = await _context.TownShips.FindAsync(id);
            if (townShip == null)
            {
                return NotFound();
            }
            ViewData["IdTown"] = new SelectList(_context.Towns, "Id", "Name", townShip.IdTown);
            return View(townShip);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TownShip townShip)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //para la tabla Productor
                        string sql1 = @"UPDATE TownShip SET name = @name, idTown = @idTown, lastUpdate = CURRENT_TIMESTAMP WHERE id = @id;";
                        // Crea objetos SqlParameter para los parámetros de la consulta
                        SqlParameter[] parameters1 = new SqlParameter[]
                        {
                            new SqlParameter("@name", townShip.Name),
                            new SqlParameter("@idTown", townShip.IdTown),
                            new SqlParameter("@id", townShip.Id),
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
                        return View(townShip);
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                    return View(townShip);
                }
            }
        }


        private bool TownShipExists(short id)
        {
            return (_context.TownShips?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Delete(short? id)
        {
            var township = await _context.TownShips.FirstOrDefaultAsync(m => m.Id == id);
            if (township == null)
            {
                return NotFound();
            }
            TempData["TownShipToDelete"] = (int)township.Id;
            TempData["Name"] = township.Name;
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
            var townShip = await _context.TownShips.FindAsync(id);
            if (townShip == null)
            {
                return NotFound();
            }
            townShip.Status = 0; // O es valor Inactivo          
            _context.TownShips.Update(townShip);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
