using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MVC.Models; // Asumimos que los Modelos están aquí
using MVC.ViewModels; // ❗ IMPORTANTE: Los nuevos ViewModels
using Microsoft.AspNetCore.Mvc.Rendering; // Necesario para SelectList
using MVC.Interfaces;

namespace MVC.Controllers;

public class PeliculaController : Controller
{
    private IPeliculaRepository _peliRepo;
    private IAuthenticationService _authService;

    public PeliculaController(IPeliculaRepository repo, IAuthenticationService authService)
    {
        _peliRepo = repo;
        _authService = authService;
    }

    // ----- Listar Index
    public IActionResult Index()
    {
        // Controles de autenticación
        if (!_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        var peliculas = _peliRepo.GetAll();

        // mapeo manual
        var listaViewModels = new List<PeliculaIndexViewModel>();
        foreach (var peli in peliculas)
        {
            listaViewModels.Add(new PeliculaIndexViewModel(peli));
        }
        return View(listaViewModels);
    }

    // GetByID
    public IActionResult GetById(int id)
    {
        Pelicula peli = _peliRepo.GetById(id);
        if (peli == null)
        {
            return NotFound();
        }
        return View(peli);
    }

    // ----- CREATE 
    public IActionResult Create()
    {
        if (!_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        if (!_authService.HasAccessLevel("Administrador"))
        {
            return RedirectToAction("AccesoDenegado", "Login");
        }

        // Instanciamos el modelo
        var model = new PeliculaCreateViewModel();

        // Cargamos las opciones ANTES de ir a la vista
        var items = Enum.GetValues(typeof(Category))
                        .Cast<Category>()
                        .Select(c => new SelectListItem
                        {
                            Value = c.ToString(),
                            Text = c.ToString()
                        }).ToList();

        model.ListaCategorias = new SelectList(items, "Value", "Text");

        // 3. Mandamos el modelo lleno
        return View(model);
    }

    // ----- CREATE (POST)
    [HttpPost]
    public IActionResult Create(PeliculaCreateViewModel model)
    {
        // 1. Control de seguridad rápido
        if (!_authService.HasAccessLevel("Administrador"))
        {
            return RedirectToAction("AccesoDenegado", "Login");
        }

        // 2. Si el formulario es válido (cumple los [Required], rangos, etc.)
        if (ModelState.IsValid)
        {
            // 3. MAPEO MANUAL: De ViewModel a Modelo de Dominio
            var nuevaPeli = new Pelicula
            {
                Titulo = model.Titulo,
                Anio = model.Anio,
                Categoria = model.Categoria // Acá pasamos el enum
            };

            // 4. Guardamos en la base de datos
            _peliRepo.Add(nuevaPeli);

            // 5. Volvemos al listado
            return RedirectToAction("Index");
        }

        // 6. Si hay error en el formulario (ej: año 3000), recargamos la lista del desplegable
        var items = Enum.GetValues(typeof(Category))
                        .Cast<Category>()
                        .Select(c => new SelectListItem
                        {
                            Value = c.ToString(),
                            Text = c.ToString()
                        }).ToList();

        model.ListaCategorias = new SelectList(items, "Value", "Text");

        // Y le devolvemos la vista con los errores
        return View(model);
    }

    // ----- UPDATE / MODIFICAR (GET)
    public IActionResult Update(int id)
    {
        // 1. Seguridad
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
        if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction("AccesoDenegado", "Login");

        // 2. Buscar la peli
        var peli = _peliRepo.GetById(id);
        if (peli == null) return NotFound();

        // 3. MAPEO MANUAL al ViewModel de Update
        var model = new PeliculaUpdateViewModel(peli);

        // 4. EL MACHETE: Cargar las categorías para el desplegable
        var items = Enum.GetValues(typeof(Category))
                        .Cast<Category>()
                        .Select(c => new SelectListItem { Value = c.ToString(), Text = c.ToString() }).ToList();
        
        model.ListaCategorias = new SelectList(items, "Value", "Text");

        return View(model);
    }

    // ----- UPDATE / MODIFICAR (POST)
    [HttpPost]
    public IActionResult Update(PeliculaUpdateViewModel model)
    {
        // 1. Seguridad en el POST
        if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction("AccesoDenegado", "Login");

        if (ModelState.IsValid)
        {
            // 2. Mapeo inverso (ViewModel -> Modelo de Dominio)
            var peliculaEditar = new Pelicula
            {
                Id = model.Id,
                Titulo = model.Titulo,
                Anio = model.Anio,
                Categoria = model.Categoria
            };

            // 3. Guardar y volver
            _peliRepo.Update(peliculaEditar);
            return RedirectToAction(nameof(Index));
        }

        // 4. Si falla la validación, recargar el SelectList para que no explote la vista
        var items = Enum.GetValues(typeof(Category)).Cast<Category>().Select(c => new SelectListItem { Value = c.ToString(), Text = c.ToString() }).ToList();
        model.ListaCategorias = new SelectList(items, "Value", "Text");
        
        return View(model);
    }

    // ----- DELETE (GET)
    public IActionResult Delete(int id)
    {
        // 1. Seguridad
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
        if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction("AccesoDenegado", "Login");

        // 2. Buscar
        var pelicula = _peliRepo.GetById(id);
        if (pelicula == null) return NotFound();

        // 3. Usamos el IndexViewModel para mostrar los datos seguros en la vista de confirmación
        var model = new PeliculaIndexViewModel(pelicula);
        
        return View(model);
    }

    // ----- DELETE (POST)
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        // 1. Seguridad en el POST
        if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction("AccesoDenegado", "Login");

        // 2. Borrar y volver
        _peliRepo.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}