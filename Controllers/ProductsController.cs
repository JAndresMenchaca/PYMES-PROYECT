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
    public class ProductsController : Controller
    {
        private readonly DbPymesContext _context;
        public List<Person> peopleList { get; set; }
        public ProductsController(DbPymesContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var dbPymesContext = _context.Products
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdSectorNavigation)
                .Include(p => p.IdProducerNavigation.IdNavigation)
                .Where(p => p.Status ==1);

            


            return View(await dbPymesContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdProducerNavigation)
                .Include(p => p.IdSectorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["IdCategory"] = new SelectList(_context.Categories.Where(p => p.Status ==1), "Id", "Name");
            // Realizar un join entre Producer y Person para obtener el Id y el Name
            var query = from producer in _context.Producers
                        join person in _context.People on producer.Id equals person.Id
                        where person.Status == 1
                        select new
                        {
                            Id = producer.Id,
                            Name = person.Name
                        };

            // Crear el SelectList a partir de los resultados del join
            ViewData["IdProducer"] = new SelectList(query.ToList(), "Id", "Name");

            ViewData["IdSector"] = new SelectList(_context.Sectors.Where(p => p.Status == 1), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(Product products)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        string userID = HttpContext.Session.GetString("UserID");
                        products.UserId = int.Parse(userID);
                        _context.Products.Add(products);
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

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["IdCategory"] = new SelectList(_context.Categories.Where(p => p.Status == 1), "Id", "Name");
            // Realizar un join entre Producer y Person para obtener el Id y el Name
            var query = from producer in _context.Producers
                        join person in _context.People on producer.Id equals person.Id
                        where person.Status == 1
                        select new
                        {
                            Id = producer.Id,
                            Name = person.Name
                        };

            // Crear el SelectList a partir de los resultados del join
            ViewData["IdProducer"] = new SelectList(query.ToList(), "Id", "Name");

            ViewData["IdSector"] = new SelectList(_context.Sectors.Where(p => p.Status == 1), "Id", "Name");
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        string userID = HttpContext.Session.GetString("UserID");
                        product.UserId = int.Parse(userID);
                        //para la tabla Productor
                        string sql1 = @"UPDATE Product SET name = @name, description = @desc, basePrise = @base, stock = @stock, unitMeasure = @unit, manufacturingNeed = @manu, idProducer = @idProducer, idCategory = @idCategory, idSector = @idSector, userID = @userId, lastUpdate = CURRENT_TIMESTAMP  ";
                        // Crea objetos SqlParameter para los parámetros de la consulta
                        SqlParameter[] parameters1 = new SqlParameter[]
                        {
                            new SqlParameter("@name", product.Name),
                            new SqlParameter("@desc", product.Description),
                            new SqlParameter("@base", product.BasePrise),
                            new SqlParameter("@stock", product.Stock),
                            new SqlParameter("@unit", product.UnitMeasure),
                            new SqlParameter("@manu", product.ManufacturingNeed),
                            new SqlParameter("@idProducer", product.IdProducer),
                            new SqlParameter("@idCategory", product.IdCategory),
                            new SqlParameter("@idSector", product.IdSector),
                            new SqlParameter("@userId", product.UserId)
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
                        return View(product);
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                    return View(product);
                }
            }
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var products = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }
            TempData["TownShipToDelete"] = (int)products.Id;
            TempData["Name"] = products.Name;
            TempData["ShowModalDelete"] = true;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateShowModalDelete(bool value)
        {
            TempData["ShowModalDelete"] = value;
            return Json(new { success = true });
        }


        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            products.Status = 0; // O es valor Inactivo          
            _context.Products.Update(products);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
