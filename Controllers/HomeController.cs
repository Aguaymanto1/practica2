using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using practica2.Models;
using practica2.Data;
using Microsoft.EntityFrameworkCore;


namespace practica2.Controllers;

public class HomeController : Controller
{
   private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /
        public async Task<IActionResult> Index()
        {
            var inmuebles = await _context.Inmuebles
                .Where(i => i.Activo)
                .ToListAsync();

            return View(inmuebles);
        }
}
