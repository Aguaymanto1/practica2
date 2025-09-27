using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practica2.Data;
using practica2.Models;

namespace practica2.Areas.Broker.Controllers
{
    [Area("Broker")]
    [Authorize(Roles = "Broker")]
    public class AgendaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgendaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var visitas = await _context.Visitas
                .Include(v => v.Inmueble)
                .Where(v => v.FechaInicio.Date == hoy)
                .ToListAsync();

            return View(visitas);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, EstadoVisita nuevoEstado)
        {
            var visita = await _context.Visitas.FindAsync(id);
            if (visita == null) return NotFound();

            visita.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
