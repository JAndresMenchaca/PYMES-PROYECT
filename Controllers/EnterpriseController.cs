using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;
using PymesDAO.Model;
using static Azure.Core.HttpHeader;
using static System.Net.Mime.MediaTypeNames;

namespace Proyecto_Pymes.Controllers
{
    public class EnterpriseController : Controller
    {

        private readonly DbPymesContext _context;

        public EnterpriseController(DbPymesContext context)
        {
            _context = context;
        }

        //Vista del Index
        public async Task<IActionResult> Index()
        {
            var enterprises = await _context.Enterprises
                .Where(e => e.Status == 1)
                .Include(e => e.IdTownShipNavigation) // Incluye la relación con la tabla de ciudades
                .ToListAsync();

            return View(enterprises);
        }



        //for the create----------------------------------------------------------------------------------
        public async Task<IActionResult> Create()
        {
            var towns = await _context.TownShips
                .Where(t => t.Status == 1)
                .ToListAsync();

            ViewBag.TownShips = new SelectList(towns, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Enterprise enterprise, IFormFile imagen)
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

                    enterprise.UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                    _context.Enterprises.Add(enterprise);
                        await _context.SaveChangesAsync();
                }              
                TempData["ShowModal"] = true;
            }
            else
            {
                var towns = await _context.TownShips
               .Where(t => t.Status == 1)
               .ToListAsync();
                ViewBag.TownShips = new SelectList(towns, "Id", "Name");

                using (var stream = new MemoryStream())
                {
                    imagen.CopyTo(stream);
                    enterprise.Image = stream.ToArray();
                }

            }
            return View(enterprise);
        }

       



        //para el edit------------------------------------------
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

                return View(enterprise);
            }
            catch (Exception ex)
            { 
                return RedirectToAction("Error", "Home"); // Redirige a una página de error genérica
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Enterprise enterprise, IFormFile? imageIF)
        {
              try
                {
                    if (imageIF != null && imageIF.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            imageIF.CopyTo(stream);
                            enterprise.Image = stream.ToArray();
                        }              
                    }
                    else{
                       // var enterpriseOld = await _context.Enterprises.FindAsync(enterprise.Id);
                        
                    }
                    //validasion del form
                    if (ModelState.IsValid)
                    {                     
                            enterprise.LastUpdate = DateTime.Now;

                             _context.Enterprises.Update(enterprise);
                             await _context.SaveChangesAsync();

                             return RedirectToAction("Index");
                    }
                    else
                    {
                        // Si hay errores en el modelo, no se realizará el commit de la transacción
                        return RedirectToAction("Edit", new { id = enterprise.Id });
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario               
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                    return View(enterprise);
                }
            
        }





        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var enterprise = await _context.Enterprises.FirstOrDefaultAsync(e => e.Id == id);
            //if (enterprise == null)
            //{
            //    return NotFound();
            //}
            //TempData["EnterpriseID"] =(int)enterprise.Id;
            //TempData["Name"] = enterprise.GroupName;        
            //TempData["ShowModalDeleteEnprise"] = true;
            return PartialView("DeleteViewPartial",enterprise);
        }



        //para la confirmasion de la Eliminasion
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
            enterprise.Status = 0; // O es valor Inactivo          
            _context.Enterprises.Update(enterprise);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
