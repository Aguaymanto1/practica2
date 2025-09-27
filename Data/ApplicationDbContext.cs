using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using practica2.Models;

namespace practica2.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Inmueble> Inmuebles { get; set; }
    public DbSet<Visita> Visitas { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
        

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
base.OnModelCreating(modelBuilder);


modelBuilder.Entity<Inmueble>()
.HasIndex(i => i.Codigo)
.IsUnique();


modelBuilder.Entity<Inmueble>()
.HasCheckConstraint("CK_Inmueble_Precio_Metros", "Precio > 0 AND MetrosCuadrados > 0");


modelBuilder.Entity<Visita>()
.HasCheckConstraint("CK_Visita_Fechas", "FechaInicio < FechaFin");


modelBuilder.Entity<Visita>()
.HasOne(v => v.Inmueble)
.WithMany(i => i.Visitas)
.HasForeignKey(v => v.InmuebleId)
.OnDelete(DeleteBehavior.Cascade);


modelBuilder.Entity<Reserva>()
.HasOne(r => r.Inmueble)
.WithMany(i => i.Reservas)
.HasForeignKey(r => r.InmuebleId)
.OnDelete(DeleteBehavior.Cascade);


modelBuilder.Entity<Inmueble>().HasData(
new Inmueble { Id = 1, Codigo = "I-0001", Titulo = "Dpto céntrico 2D", Tipo = TipoInmueble.Departamento, Ciudad = "Lima", Direccion = "Av. Central 123", Dormitorios = 2, Banos = 1, MetrosCuadrados = 60, Precio = 120000m, Activo = true, Imagen = "https://via.placeholder.com/300x200" },
new Inmueble { Id = 2, Codigo = "I-0002", Titulo = "Casa familiar", Tipo = TipoInmueble.Casa, Ciudad = "Arequipa", Direccion = "Calle Falsa 456", Dormitorios = 4, Banos = 2, MetrosCuadrados = 180, Precio = 320000m, Activo = true, Imagen = "https://via.placeholder.com/300x200" },
new Inmueble { Id = 3, Codigo = "I-0003", Titulo = "Oficina en distrito financiero", Tipo = TipoInmueble.Oficina, Ciudad = "Lima", Direccion = "Torre Business 9", Dormitorios = 0, Banos = 1, MetrosCuadrados = 45, Precio = 90000m, Activo = true, Imagen = "https://via.placeholder.com/300x200" }
);
}
}

