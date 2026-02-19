using System.Collections.Generic;
using MVC.Models; 

namespace MVC.Interfaces;

public interface IPeliculaRepository
{
    List<Pelicula> GetAll();
    Pelicula GetById(int id);
    void Add(Pelicula peli);
    void Update(Pelicula pelicula);
    void Delete(int id);
}