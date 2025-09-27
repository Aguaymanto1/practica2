using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using practica2.Data;
using practica2.Models;

namespace practica2.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly UserManager<IdentityUser> _userManager;
        private const int PageSize = 5;

        public CatalogController(ApplicationDbContext context, IDistributedCache cache, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _cache = cache;
            _userManager = userManager;
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
            // Guardar filtros en sesión
            HttpContext.Session.SetString("UltimosFiltros", JsonConvert.SerializeObject(new
            {
                ciudad, tipo, precioMin, precioMax, dormitorios
            }));

            // Validaciones
            if (precioMin < 0 || precioMax < 0)
            {
                ModelState.AddModelError("Precio", "Los precios no pueden ser negativos.");
            }
            if (precioMin.HasValue && precioMax.HasValue && precioMin > precioMax)
            {
                ModelState.AddModelError("Precio", "El precio mínimo no puede ser mayor al máximo.");
            }

            // Construir clave para caché
            string cacheKey = $"catalogo:{ciudad}:{tipo}:{precioMin}:{precioMax}:{dormitorios}:page{page}";

            // Intentar recuperar de caché
            string? cachedData = await _cache.GetStringAsync(cacheKey);
            List<Inmueble>? inmuebles;

            if (cachedData != null)
            {
                inmuebles = JsonConvert.DeserializeObject<List<Inmueble>>(cachedData)!;
            }
            else
            {
                var query = _context.Inmuebles.Where(i => i.Activo);

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

                inmuebles = await query
                    .OrderBy(i => i.Precio)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                // Guardar en caché 60s
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonConvert.SerializeObject(inmuebles),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                    });
            }

            // Paginación
            var totalItems = await _context.Inmuebles.CountAsync(i => i.Activo);
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            return View(inmuebles);
        }

        // GET: /Catalog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var inmueble = await _context.Inmuebles
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inmueble == null)
                return NotFound();

            // Guardar último inmueble en sesión
            HttpContext.Session.SetInt32("UltimoInmuebleId", inmueble.Id);
            HttpContext.Session.SetString("UltimoInmuebleTitulo", inmueble.Titulo);

            return View(inmueble);
        }
    }
}
