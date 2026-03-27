using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AppCidades
{
    public static class DatabaseHelper
    {
        private static string dbPath = "cidades.json";

        public static void InicializarBanco()
        {
            if (!File.Exists(dbPath))
            {
                File.WriteAllText(dbPath, "[]");
            }
        }

        public static List<Cidade> Listar(string busca = "")
        {
            if (!File.Exists(dbPath)) return new List<Cidade>();

            string json = File.ReadAllText(dbPath);
            
            // Configuração para aceitar nomes minúsculos do JSON nas propriedades maiúsculas do C#
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            var lista = JsonSerializer.Deserialize<List<Cidade>>(json, options) ?? new List<Cidade>();

            if (string.IsNullOrWhiteSpace(busca)) return lista;

            return lista.Where(c => 
                c.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase) || 
                c.Pais.Contains(busca, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public static void Salvar(Cidade novaCidade, string nomeOriginal = null)
        {
            var lista = Listar();

            if (!string.IsNullOrEmpty(nomeOriginal))
            {
                // Modo Edição: Remove a versão antiga e adiciona a nova
                lista.RemoveAll(c => c.Nome.Equals(nomeOriginal, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                // Evita duplicados no "Adicionar" se já existir o mesmo nome
                lista.RemoveAll(c => c.Nome.Equals(novaCidade.Nome, StringComparison.OrdinalIgnoreCase));
            }

            lista.Add(novaCidade);
            string json = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dbPath, json);
        }

        public static void Excluir(string nome)
        {
            var lista = Listar();
            lista.RemoveAll(c => c.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
            
            string json = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dbPath, json);
        }
    }
}
