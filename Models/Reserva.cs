using System.ComponentModel.DataAnnotations;


namespace practica2.Models
{
public class Reserva
{
public int Id { get; set; }


[Required]
public int InmuebleId { get; set; }
public Inmueble? Inmueble { get; set; }


[Required]
public string UsuarioId { get; set; } = null!;


[Required]
public DateTime FechaCreacion { get; set; }


[Required]
public DateTime FechaExpiracion { get; set; }
}
}