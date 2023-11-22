using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;

namespace Proyecto_Pymes.Controllers
{
    [Authorize]
    public class RawMaterialController : Controller
    {
        private readonly DbPymesContext _context;

        public RawMaterialController(DbPymesContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Index()
        {
            try{
                int userId = int.Parse(HttpContext.Session.GetString("UserID"));
                var unitType = await _context.RawMaterials
                .Include(u => u.IdUnitTypeNavigation)
                .Where(e => e.Status == 1 && e.UserId == userId)
                .ToListAsync();
                return View(unitType);
            }catch{
                return RedirectToAction("Privacy", "Home");
                
            }
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create()
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserID"));
            var unitType = await _context.UnitTypes
               .Where(t => t.Status == 1 && t.UserId == userId)
               .ToListAsync();

            ViewBag.UnitType = new SelectList(unitType, "Id", "Type");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create(RawMaterial rawMaterial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    rawMaterial.UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                    _context.RawMaterials.Add(rawMaterial);
                    await _context.SaveChangesAsync();
                    //PARA EL MODAL
                    TempData["ShowModal"] = true;
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error en la transacción.");            
            }
            return View(rawMaterial);
        }


        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Edit(int Id)
        {     
            try
            {
                var rawMaterial = await _context.RawMaterials.FindAsync(Id);

                int userId = int.Parse(HttpContext.Session.GetString("UserID"));
                var unitType = await _context.UnitTypes
                   .Where(t => t.Status == 1 && t.UserId == userId)
                   .ToListAsync();

                //Para poder Mostrar el Modal
                ViewBag.UnitType = new SelectList(unitType, "Id", "Type");
                if (rawMaterial == null)
                {
                    return NotFound();
                }
                return View(rawMaterial);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error en la transacción.");            
                return View(); // Redirige a una página de error genérica
            }   
        }



        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async  Task<IActionResult> Edit(RawMaterial rawMaterial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    rawMaterial.LastUpdate = DateTime.Now;
                    _context.RawMaterials.Update(rawMaterial);
                    await _context.SaveChangesAsync();

                    TempData["ShowModal"] = true;
                }
            }
            catch (Exception)
            {                     
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
            }

            return View(rawMaterial);
        }


        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Delete(int Id)
        {
            var rowMaterial = await _context.RawMaterials.FirstOrDefaultAsync(e => e.Id == Id);
            return PartialView("DeleteViewPartial", rowMaterial);
        }


        //para la confirmasion de la Eliminasion
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (id == null && _context.RawMaterials != null)
            {
                return NotFound();
            }

            var rawMateriial = await _context.RawMaterials.FindAsync(id);
            
            if (rawMateriial == null)
            {
                return NotFound();
            }

            rawMateriial.Status = 0; // O es valor Inactivo          
            _context.RawMaterials.Update(rawMateriial);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



    }
}
