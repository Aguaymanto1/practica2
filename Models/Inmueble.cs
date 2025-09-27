using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Practica2.Models
{
public class Inmueble
{
public int Id { get; set; }


[Required]
[StringLength(50)]
public string Codigo { get; set; } = null!; // Unico, definido en Fluent API


[Required]
public string Titulo { get; set; } = null!;


public string? Imagen { get; set; }


[Required]
public TipoInmueble Tipo { get; set; }


[Required]
public string Ciudad { get; set; } = null!;


public string? Direccion { get; set; }


[Range(0, int.MaxValue)]
public int Dormitorios { get; set; }


[Range(0, int.MaxValue)]
public int Banos { get; set; }


[Range(0.0001, double.MaxValue, ErrorMessage = "MetrosCuadrados debe ser mayor que 0")]
public double MetrosCuadrados { get; set; }


[Range(0.01, double.MaxValue, ErrorMessage = "Precio debe ser mayor que 0")]
public decimal Precio { get; set; }


public bool Activo { get; set; } = true;


public List<Visita>? Visitas { get; set; }
public List<Reserva>? Reservas { get; set; }
}
}