using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;
using Proyecto_Pymes.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_Pymes.Controllers
{
    [Authorize]
    public class ManufacturingController : Controller
    {
        private readonly DbPymesContext _context;
       

        public ManufacturingController(DbPymesContext context)
        {
            _context = context;
        }
       

        public IActionResult Index()
        {
            int UserId = int.Parse(HttpContext.Session.GetString("UserID"));
            var Manufac = _context.Manufacturings
                        .Include(p => p.IdProductNavigation)
                        .Include(d => d.ProductionDetails)
                           .ThenInclude(pd => pd.IdRawMaterialNavigation)
                        .Where(m => m.Status == 1 && m.UserId == UserId).ToList();

            return View(Manufac);
        }

        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create()
        {
           int UserId = int.Parse(HttpContext.Session.GetString("UserID"));

            var producs = await _context.Products
              .Where(p => p.Status == 1 && p.ManufacturingNeed == 1 && p.IdProducer == UserId)
              .ToListAsync();

            var rawMaterial = await _context.RawMaterials
             .Where(p => p.Status == 1 && p.UserId == UserId)
             .ToListAsync();


            ViewBag.Products = new SelectList(producs, "Id", "Name");
            ViewBag.rawMaterial = new SelectList(rawMaterial, "Id", "Name");
            return View();
        }


        void CargarCombox()
        {
            int UserId = int.Parse(HttpContext.Session.GetString("UserID"));
            var producs =  _context.Products
              .Where(p => p.Status == 1 && p.ManufacturingNeed == 1 && p.UserId == UserId)
              .ToList();

            var rawMaterial = _context.RawMaterials
             .Where(p => p.Status == 1 && p.UserId == UserId)
             .ToList();


            ViewBag.Products = new SelectList(producs, "Id", "Name");
            ViewBag.rawMaterial = new SelectList(rawMaterial, "Id", "Name");
        }




        [HttpPost]
        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Create(IFormCollection form,Manufacturing manufac,  int? CantidadMateria, int idMateriaPrima)
        {
            try
            {   
                if (form.ContainsKey("btnConfirmar"))
                {
                    if (ModelState.IsValid)
                    {
                        Manufacturing manu = new Manufacturing();

                        manu.IdProduct = manufac.IdProduct;
                        manu.CostProduction = manufac.CostProduction;
                        manu.Quantity = manufac.Quantity;
                        manu.UserId = int.Parse(HttpContext.Session.GetString("UserID"));


                        //agregamos la manufactura
                        var amun = await _context.Manufacturings.AddAsync(manu);
                        await _context.SaveChangesAsync();


                        //Agregamos el detalle               
                        foreach (var detail in manufac.ProductionDetails)
                        {
                            detail.IdRawMaterialNavigation = null;
                            detail.IdManufacturing = amun.Entity.Id;
                            _context.ProductionDetails.Add(detail);
                        }
                        await _context.SaveChangesAsync();


                        //Actualisar la materia Prima
                        foreach (var detailRaw in manufac.ProductionDetails)
                        {
                            var rawMaterial = await _context.RawMaterials.FindAsync(detailRaw.IdRawMaterial);
                            if (rawMaterial != null)
                            {
                                rawMaterial.Stock = rawMaterial.Stock - detailRaw.Quantity;
                                _context.RawMaterials.Update(rawMaterial);
                            }
                        }
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    // Si hay errores de validación, volvemos a mostrar la vista con los errores
                    CargarCombox();
                    return View(manufac);
                }
                else if (form.ContainsKey("btnAgregar"))
                {
                    if (idMateriaPrima == 0)
                    {
                        TempData["Insumos"] = "Canpo Obligatorio";
                        CargarCombox();
                        return View(manufac);
                    }

                    if (CantidadMateria == null)
                    {
                        TempData["Cantidad"] = "Campo Obligatorio, Solo números enteros";
                        CargarCombox();
                        return View(manufac);
                    }
                   
                    Regex regex = new Regex(@"^[1-9]\d{0,8}$");
                    if (!regex.IsMatch(CantidadMateria.ToString()))
                    {
                        TempData["Cantidad"] = "Solo números Positivos, como máximo 8 dígitos";
                        CargarCombox();
                        return View(manufac);
                    }

                    var rawMaterial = await _context.RawMaterials.FindAsync(idMateriaPrima);
                    if (CantidadMateria > rawMaterial.Stock )
                    {
                        TempData["Cantidad"] = "La cantidad supero el Almacen, cantidad Maxima de Insumo = "+rawMaterial.Stock;
                        CargarCombox();
                        return View(manufac);
                    }


                    var Raw = await _context.RawMaterials.FindAsync(idMateriaPrima);
                    ProductionDetail detail = new ProductionDetail();

                    // detail.IdManufacturing = getIdManufacturing();
                    detail.IdRawMaterial = idMateriaPrima;
                    detail.Quantity = CantidadMateria ?? 0;
                    detail.IdRawMaterialNavigation = Raw;

                    manufac.ProductionDetails.Add(detail);
                    manufac.CostProduction = Price(manufac.ProductionDetails.ToList());
                    ModelState.Remove("ProductionDetails");
                }

            }
            catch(Exception)
            {
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error en la transacción.");
            }
     
            CargarCombox();
            return View(manufac);
        }





        decimal Price(List<ProductionDetail> detail)
        {
            decimal priceTotal = 0;
            foreach (var item in detail)
            {
                priceTotal = item.Quantity * item.IdRawMaterialNavigation.UnitPrice;
            }
            return priceTotal;
        }



        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> Delete(int? id)
        {
            var manufac = await _context.Manufacturings.FirstOrDefaultAsync(e => e.Id == id);
            if (manufac == null)
            {
                return NotFound();
            }
            return PartialView("DeleteViewPartial", manufac);
        }




        [Authorize(Roles = "Productor")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var manufac = await _context.Manufacturings.FindAsync(id);
            if (manufac == null)
            {
                return NotFound();
            }
            manufac.Status = 0; // O es valor Inactivo          
            _context.Manufacturings.Update(manufac);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }




        //para poder Mostrar el Detalle
        public async Task<IActionResult> Detail(int? Id)
        {
            if(Id  == null)
            {
                return NotFound();
            }
            var manufac = await _context.Manufacturings
                             .Include(p => p.IdProductNavigation)
                             .Include(d => d.ProductionDetails)
                                 .ThenInclude(pd => pd.IdRawMaterialNavigation)
                             .Where(i => i.Id == Id)
                             .SingleOrDefaultAsync();

            return PartialView("DetailViewPartial", manufac);
        } 
    }
}
