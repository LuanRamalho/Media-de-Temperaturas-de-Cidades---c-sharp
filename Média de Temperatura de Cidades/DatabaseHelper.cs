using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite; // Mudou aqui
using System.IO;

namespace AppCidades
{
    public static class DatabaseHelper
    {
        private static string dbPath = "cidades.db";
        private static string connectionString = $"Data Source={dbPath};"; // String simplificada

        public static void InicializarBanco()
        {
            using (var conexao = new SqliteConnection(connectionString))
            {
                conexao.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS cidades (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    cidade TEXT, pais TEXT, continente TEXT,
                    verao REAL, outono REAL, inverno REAL, primavera REAL, media REAL)";
                using (var comando = new SqliteCommand(sql, conexao)) comando.ExecuteNonQuery();
            }
        }

        public static void Salvar(Cidade c)
        {
            using (var conexao = new SqliteConnection(connectionString))
            {
                conexao.Open();
                string sql = c.Id == 0 
                    ? "INSERT INTO cidades (cidade, pais, continente, verao, outono, inverno, primavera, media) VALUES (@c, @p, @con, @v, @o, @i, @pri, @m)"
                    : "UPDATE cidades SET cidade=@c, pais=@p, continente=@con, verao=@v, outono=@o, inverno=@i, primavera=@pri, media=@m WHERE id=@id";

                using (var cmd = new SqliteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@c", c.Nome);
                    cmd.Parameters.AddWithValue("@p", c.Pais);
                    cmd.Parameters.AddWithValue("@con", c.Continente);
                    cmd.Parameters.AddWithValue("@v", c.Verao);
                    cmd.Parameters.AddWithValue("@o", c.Outono);
                    cmd.Parameters.AddWithValue("@i", c.Inverno);
                    cmd.Parameters.AddWithValue("@pri", c.Primavera);
                    cmd.Parameters.AddWithValue("@m", c.Media);
                    if (c.Id > 0) cmd.Parameters.AddWithValue("@id", c.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Cidade> Listar(string busca = "")
        {
            var lista = new List<Cidade>();
            using (var conexao = new SqliteConnection(connectionString))
            {
                conexao.Open();
                string sql = "SELECT * FROM cidades WHERE cidade LIKE @b OR pais LIKE @b";
                using (var cmd = new SqliteCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@b", "%" + busca + "%");
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Cidade {
                                Id = rdr.GetInt32(0),
                                Nome = rdr.GetString(1),
                                Pais = rdr.GetString(2),
                                Continente = rdr.GetString(3),
                                Verao = rdr.GetDouble(4),
                                Outono = rdr.GetDouble(5),
                                Inverno = rdr.GetDouble(6),
                                Primavera = rdr.GetDouble(7),
                                Media = rdr.GetDouble(8)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public static void Excluir(int id)
        {
            using (var conexao = new SqliteConnection(connectionString))
            {
                conexao.Open();
                using (var cmd = new SqliteCommand("DELETE FROM cidades WHERE id=@id", conexao))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}