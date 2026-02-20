using System.ComponentModel.DataAnnotations;
using MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering; // el de las listas desplegables

namespace MVC.ViewModels;

public class PeliculaUpdateViewModel : PeliculaCreateViewModel // heredo campos y validaciones
{
    [Required]
    public int Id { get; set; } 

    public PeliculaUpdateViewModel() : base()
    {
    }

    public PeliculaUpdateViewModel(Pelicula pelicula) : base(pelicula)
    {
        Id = pelicula.Id;
    }
}