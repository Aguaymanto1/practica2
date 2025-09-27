using System.ComponentModel.DataAnnotations;
using practica2.Models;


namespace Practica2.Models
{
public class CatalogFilterViewModel
{
public string? Ciudad { get; set; }
public TipoInmueble? Tipo { get; set; }


[Range(0, double.MaxValue, ErrorMessage = "Precio mínimo no puede ser negativo")]
public decimal? PrecioMin { get; set; }


[Range(0, double.MaxValue, ErrorMessage = "Precio máximo no puede ser negativo")]
public decimal? PrecioMax { get; set; }


[Range(0, int.MaxValue, ErrorMessage = "Dormitorios no puede ser negativo")]
public int? Dormitorios { get; set; }


// Resultados paginados
public List<Inmueble>? Resultados { get; set; }
public int Page { get; set; } = 1;
public int PageSize { get; set; } = 6;
public int TotalItems { get; set; }
}
}