using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using MVC.Models; // Clases Modelo
using MVC.Interfaces;

namespace MVC.Repositorios;

public class PeliculaRepository : IPeliculaRepository
{
    private readonly string CadenaConexion = "Data Source=./DB/streaming.db";
    public PeliculaRepository()
    {
    }

    // CRUD
    public List<Pelicula> GetAll()
    {
        var lista = new List<Pelicula>();
        const string sql = "SELECT Id, Titulo, Anio, Categoria FROM Peliculas ORDER BY Titulo ASC";

        using var conexion = new SqliteConnection(CadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            // Obtengo la cat como string, luego la parseo a enum
            string categoriaString = lector.GetString(3);
            Category cat;
            Enum.TryParse<Category>(categoriaString, true, out cat);

            var pelicula = new Pelicula
            {
                Id = lector.GetInt32(0),
                Titulo = lector.GetString(1),
                Anio = lector.GetInt32(2),
                Categoria = cat
            };
            lista.Add(pelicula);
        }

        return lista;
    }
    public Pelicula GetById(int ide)
    {
        Pelicula peli = null;

        using (var connection = new SqliteConnection(CadenaConexion))
        {
            connection.Open();
            string sql = @"SELECT Id, Titulo, Anio, Categoria
                FROM Peliculas
                WHERE Id = @id";

            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", ide);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) // si lee algo
                    {
                        string categoriaString = reader.GetString(3);
                        Category cat;
                        Enum.TryParse<Category>(categoriaString, true, out cat);

                        peli = new Pelicula()
                        {
                            Id = reader.GetInt32(0),
                            Titulo = reader.GetString(1),
                            Anio = reader.GetInt32(2),
                            Categoria = cat,
                        };
                    }
                }
            }
        }
        return peli;
    }
    public void Add(Pelicula peli)
    {
        using var conexion = new SqliteConnection(CadenaConexion);
        conexion.Open();

        string sql = @"INSERT INTO Peliculas (Titulo, Anio, Categoria)
            VALUES(@tit, @anio, @cat)";

        using var comando = new SqliteCommand(sql, conexion);

        comando.Parameters.AddWithValue("@tit", peli.Titulo);
        comando.Parameters.AddWithValue("@anio", peli.Anio);
        comando.Parameters.AddWithValue("@cat", peli.Categoria.ToString()); // para que se guarde con el nombre

        comando.ExecuteNonQuery();
    }
    public void Update(Pelicula pelicula)
    {
        using var conexion = new SqliteConnection(CadenaConexion);
        conexion.Open();

        string sql = @"UPDATE Peliculas
            SET Titulo = @tit, Anio = @anio, Categoria = @cat
            WHERE Id = @id";

        using var comando = new SqliteCommand(sql, conexion);

        comando.Parameters.AddWithValue("@id", pelicula.Id);
        comando.Parameters.AddWithValue("@tit", pelicula.Titulo);
        comando.Parameters.AddWithValue("@anio", pelicula.Anio);
        comando.Parameters.AddWithValue("@cat", pelicula.Categoria.ToString());

        comando.ExecuteNonQuery();
    }
    public void Delete(int id)
    {
        using var conexion = new SqliteConnection(CadenaConexion);
        conexion.Open();

        string sql = @"DELETE FROM Peliculas
            WHERE Id = @id";

        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        comando.ExecuteNonQuery();
    }
}