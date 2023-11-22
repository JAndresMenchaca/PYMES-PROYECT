using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;

namespace Proyecto_Pymes.Controllers
{
    [Authorize]
    public class UnitTypeController : Controller
    {
        private readonly DbPymesContext _context;

        public UnitTypeController(DbPymesContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Index()
        {
            try
            {
                int userId = int.Parse(HttpContext.Session.GetString("UserID"));
                var unitType = await _context.UnitTypes
                   .Where(e => e.Status == 1 && e.UserId == userId)
                   .ToListAsync();
                return View(unitType);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Privacy", "Home");
            }      
        }


        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create(UnitType unit)
        {
                try
                {
                    if (ModelState.IsValid)
                    {
                
                        unit.UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                        _context.UnitTypes.Add(unit);
                        await _context.SaveChangesAsync();
                        TempData["ShowModal"] = true;
                    }
                }
                catch (Exception)
                {        
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error en la transacción.");
                    ViewBag.ShowErrorAlert = true;
                } 
            return View();
        }


        // GET: People/Edit/5
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(short? id)
        {
            var prod = await _context.UnitTypes.FindAsync(id);
            if (prod == null)
            {
                return NotFound();
            }
            return View(prod);
        }

        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(UnitType unit)
        {          
                try
                {
                    if (ModelState.IsValid)
                    {
                        unit.LastUpdate = DateTime.Now;
                        _context.UnitTypes.Update(unit);
                        await _context.SaveChangesAsync();
                        TempData["ShowModal"] = true;
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario            
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                    ViewBag.ShowErrorAlert = true;

                }
            return View(unit);
        }


        // GET: People/Delete/5
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Delete(int? id)
        {
            var enterprise = await _context.UnitTypes.FirstOrDefaultAsync(e => e.Id == id);       
            return PartialView("DeleteViewPartial", enterprise);
        }


        //para la confirmasion de la Eliminasion
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> ConfirmDelete(short? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var unit = await _context.UnitTypes.FindAsync(id);
                if (unit == null)
                {
                    return NotFound();
                }
                unit.Status = 0; // O es valor Inactivo          
                _context.UnitTypes.Update(unit);
                await _context.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                ViewBag.ShowErrorAlert = true;
            }
           

            return RedirectToAction("Index");
        }

    }
}
