using System.ComponentModel.DataAnnotations;
using MVC.Models;

namespace MVC.ViewModels;

public class PeliculaIndexViewModel
{
    public int Id {get; set;}
    public string Titulo {get; set;}
    public int Anio {get; set;}
    public Category Categoria {get; set;}

    public PeliculaIndexViewModel()
    {
    }

    public PeliculaIndexViewModel(Pelicula pelicula)
    {
        Id = pelicula.Id;
        Titulo = pelicula.Titulo;
        Anio = pelicula.Anio;
        Categoria = pelicula.Categoria;
    }
}