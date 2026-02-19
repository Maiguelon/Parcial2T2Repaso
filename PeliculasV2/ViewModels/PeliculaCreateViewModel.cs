using System.ComponentModel.DataAnnotations;
using MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering; // el de las listas desplegables

namespace MVC.ViewModels;

public class PeliculaCreateViewModel
{
    [Display(Name = "Pelicula a Crear")]
    [Required(ErrorMessage = "Titulo obligatorio")]
    [StringLength(100, MinimumLength = 2 ,ErrorMessage = "Titulo entre 2 y 100 caracteres.")]
    public string Titulo {get; set;}

    [Display(Name = "Anio de la Peli")]
    [Required(ErrorMessage = "El Anio es obligatorio.")]
    [Range(1800, 2026, ErrorMessage = "El Anio debe tener sentido y no ser del futuro.")]
    public int Anio {get; set;}

    [Required(ErrorMessage = "Categoria obligatoria.")]
    public Category Categoria {get; set;}
    public SelectList ListaCategorias { get; set; }

    public PeliculaCreateViewModel()
    {
    }

    public PeliculaCreateViewModel(Pelicula pelicula)
    {
        Titulo = pelicula.Titulo;
        Anio = pelicula.Anio;
        Categoria = pelicula.Categoria;
    }
}