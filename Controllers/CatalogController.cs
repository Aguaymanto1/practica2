using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practica2.Data;
using practica2.Models;

namespace practica2.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Catalog
        public async Task<IActionResult> Index(
            string? ciudad,
            TipoInmueble? tipo,
            decimal? precioMin,
            decimal? precioMax,
            int? dormitorios,
            int page = 1)
        {
            // Validaciones server-side
            if (precioMin < 0 || precioMax < 0)
            {
                ModelState.AddModelError("Precio", "Los precios no pueden ser negativos.");
            }

            if (precioMin.HasValue && precioMax.HasValue && precioMin > precioMax)
            {
                ModelState.AddModelError("Precio", "El precio mínimo no puede ser mayor al máximo.");
            }

            var query = _context.Inmuebles.Where(i => i.Activo).AsQueryable();

            if (!string.IsNullOrEmpty(ciudad))
                query = query.Where(i => i.Ciudad.Contains(ciudad));

            if (tipo.HasValue)
                query = query.Where(i => i.Tipo == tipo);

            if (precioMin.HasValue)
                query = query.Where(i => i.Precio >= precioMin);

            if (precioMax.HasValue)
                query = query.Where(i => i.Precio <= precioMax);

            if (dormitorios.HasValue)
                query = query.Where(i => i.Dormitorios >= dormitorios);

            var totalItems = await query.CountAsync();

            var inmuebles = await query
                .OrderBy(i => i.Precio)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Mantener filtros en la vista
            ViewBag.Ciudad = ciudad;
            ViewBag.Tipo = tipo;
            ViewBag.PrecioMin = precioMin;
            ViewBag.PrecioMax = precioMax;
            ViewBag.Dormitorios = dormitorios;

            return View(inmuebles);
        }

        // GET: /Catalog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var inmueble = await _context.Inmuebles
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }
    }
}
