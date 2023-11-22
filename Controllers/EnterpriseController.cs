using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;
using static Azure.Core.HttpHeader;
using static System.Net.Mime.MediaTypeNames;

namespace Proyecto_Pymes.Controllers
{
    [Authorize]
    public class EnterpriseController : Controller
    {

        private readonly DbPymesContext _context;

        public EnterpriseController(DbPymesContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Index()
        {
            var enterprises = await _context.Enterprises
                .Where(e => e.Status == 1)
                .Include(e => e.IdTownShipNavigation)
                .ToListAsync();

            return View(enterprises);
        }


        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Create()
        {
            var towns = await _context.TownShips
                .Where(t => t.Status == 1)
                .ToListAsync();

            ViewBag.TownShips = new SelectList(towns, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Create(Enterprise enterprise, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        imagen.CopyTo(stream);
                        enterprise.Image = stream.ToArray();
                    }
                }

                enterprise.UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                _context.Enterprises.Add(enterprise);
                await _context.SaveChangesAsync();
                TempData["ShowModal"] = true;
            }
            else
            {
                var towns = await _context.TownShips
               .Where(t => t.Status == 1)
               .ToListAsync();
                ViewBag.TownShips = new SelectList(towns, "Id", "Name");

                if (imagen != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        imagen.CopyTo(stream);
                        enterprise.Image = stream.ToArray();
                    }
                }
                

            }
            return View(enterprise);
        }





        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Edit(short id)
        {
            try
            {
                var enterprise = await _context.Enterprises.FindAsync(id);
                if (enterprise == null)
                {
                    return NotFound(); 
                }
                // Cargar la lista de ciudades disponibles
                var towns = await _context.TownShips
                    .Where(t => t.Status == 1)
                    .ToListAsync();
                // Construir la lista de selección
                ViewBag.TownShips = new SelectList(towns, "Id", "Name");
                if (enterprise.Image != null)
                {
                    TempData["CurrentImage"] = enterprise.Image;
                }

                return View(enterprise);
            }
            catch (Exception ex)
            { 
                return RedirectToAction("Error", "Home"); // Redirige a una página de error genérica
            }
        }


        [HttpPost]
        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Edit(Enterprise enterprise, IFormFile? imageIF)
        {
            try
            {
                // Cargar la lista de ciudades disponibles
                var towns = await _context.TownShips
                    .Where(t => t.Status == 1)
                    .ToListAsync();

                // Construir la lista de selección
                ViewBag.TownShips = new SelectList(towns, "Id", "Name");

                // Validación del formulario
                if (ModelState.IsValid)
                {
                    if (imageIF != null && imageIF.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            imageIF.CopyTo(stream);
                            enterprise.Image = stream.ToArray();
                        }
                    }
                    else if (TempData["CurrentImage"] is byte[] currentImage)
                    {
                        // Si no se ha cargado un nuevo archivo, restaura la imagen actual desde TempData
                        enterprise.Image = currentImage;
                    }

                    enterprise.LastUpdate = DateTime.Now;
                    _context.Enterprises.Update(enterprise);
                    await _context.SaveChangesAsync();
                    TempData["ShowModal"] = true;
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos");
            }

            return View(enterprise);
        }





        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var enterprise = await _context.Enterprises.FirstOrDefaultAsync(e => e.Id == id);
            if (enterprise == null)
            {
                return NotFound();
            }
            return PartialView("DeleteViewPartial",enterprise);
        }



        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> ConfirmDelete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var enterprise = await _context.Enterprises.FindAsync(id);
            if (enterprise == null)
            {
                return NotFound();
            }

            enterprise.Status = 0;        
            _context.Enterprises.Update(enterprise);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
