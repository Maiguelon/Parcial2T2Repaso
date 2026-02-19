namespace MVC.Models;

public enum Category
{
    Accion = 1,
    Drama = 2,
    SciFi = 3,
    Comedia = 4
}

public class Pelicula
{
    public int Id {get; set;}
    public string Titulo {get; set;}
    public int Anio {get; set;}
    public Category Categoria {get; set;}
}