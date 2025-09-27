using System.ComponentModel.DataAnnotations;


namespace practica2.Models
{
public class Visita
{
public int Id { get; set; }


[Required]
public int InmuebleId { get; set; }
public Inmueble? Inmueble { get; set; }


[Required]
public string UsuarioId { get; set; } = null!; // FK a Identity user


[Required]
public DateTime FechaInicio { get; set; }


[Required]
public DateTime FechaFin { get; set; }


[Required]
public EstadoVisita Estado { get; set; } = EstadoVisita.Solicitada;


public string? Notas { get; set; }
}
}