using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using practica2.Data;
using practica2.Models;

namespace practica2.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;
        private readonly UserManager<IdentityUser> _userManager;

        public CatalogController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
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
                .Include(i => i.Visitas)
                .Include(i => i.Reservas)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inmueble == null)
            {
                return NotFound();
            }

            // Verificar si hay reserva activa
            var reservaActiva = inmueble.Reservas
                .Any(r => r.FechaExpiracion > DateTime.Now);

            ViewBag.ReservaActiva = reservaActiva;

            return View(inmueble);
        }

        // POST: /Catalog/AgendarVisita
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgendarVisita(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, string notas)
        {

            if (fechaInicio >= fechaFin)
            {
                TempData["Error"] = "La fecha de inicio debe ser menor a la fecha de fin.";
                return RedirectToAction("Details", new { id = inmuebleId });
            }

            // Validar horario laboral (08:00–19:00)
            if (fechaInicio.Hour < 8 || fechaFin.Hour > 19)
            {
                TempData["Error"] = "Las visitas deben estar entre las 08:00 y 19:00.";
                return RedirectToAction("Details", new { id = inmuebleId });
            }

            // Validar solapamiento
            var existeSolapada = await _context.Visitas
                .AnyAsync(v =>
                    v.InmuebleId == inmuebleId &&
                    v.Estado != EstadoVisita.Cancelada &&
                    fechaInicio < v.FechaFin &&
                    fechaFin > v.FechaInicio);

            if (existeSolapada)
            {
                TempData["Error"] = "Ya existe una visita en ese intervalo.";
                return RedirectToAction("Details", new { id = inmuebleId });
            }

            var visita = new Visita
            {
                InmuebleId = inmuebleId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Estado = EstadoVisita.Solicitada,
                Notas = notas
            };

            _context.Visitas.Add(visita);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Visita solicitada correctamente.";
            return RedirectToAction("Details", new { id = inmuebleId });
        }


        // POST: /Catalog/Reservar
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(int inmuebleId)
        {
            var userId = _userManager.GetUserId(User);

            // Verificar reserva activa
            var reservaActiva = await _context.Reservas
                .AnyAsync(r => r.InmuebleId == inmuebleId && r.FechaExpiracion > DateTime.Now);

            if (reservaActiva)
            {
                TempData["Error"] = "Ya existe una reserva activa para este inmueble.";
                return RedirectToAction("Details", new { id = inmuebleId });
            }

            var reserva = new Reserva
            {
                InmuebleId = inmuebleId,
                UsuarioId = userId!,
                FechaCreacion = DateTime.Now,
                FechaExpiracion = DateTime.Now.AddHours(48)
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reserva creada por 48 horas.";
            return RedirectToAction("Details", new { id = inmuebleId });
        }
    }
}
