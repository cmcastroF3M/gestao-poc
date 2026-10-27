using Microsoft.Data.Sqlite;

namespace Gestao;

public class ClienteRepositorio
{
    private readonly string _ligacao;

    public ClienteRepositorio(string caminhoFicheiro)
    {
        _ligacao = new SqliteConnectionStringBuilder
        {
            DataSource = caminhoFicheiro,
            Pooling = false
        }.ToString();

        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText =
            "CREATE TABLE IF NOT EXISTS Clientes (" +
            "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Nome TEXT NOT NULL, " +
            "Contacto TEXT NOT NULL DEFAULT '', " +
            "Notas TEXT NOT NULL DEFAULT '')";
        cmd.ExecuteNonQuery();
    }

    private SqliteConnection Abrir()
    {
        var con = new SqliteConnection(_ligacao);
        con.Open();
        return con;
    }

    private static void Validar(Cliente c)
    {
        if (string.IsNullOrWhiteSpace(c.Nome))
            throw new ArgumentException("O nome é obrigatório.");
    }

    public long Adicionar(Cliente c)
    {
        Validar(c);
        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText =
            "INSERT INTO Clientes (Nome, Contacto, Notas) VALUES ($n, $c, $o); " +
            "SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$n", c.Nome.Trim());
        cmd.Parameters.AddWithValue("$c", c.Contacto.Trim());
        cmd.Parameters.AddWithValue("$o", c.Notas.Trim());
        return (long)cmd.ExecuteScalar()!;
    }

    public List<Cliente> Listar()
    {
        var lista = new List<Cliente>();
        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText =
            "SELECT Id, Nome, Contacto, Notas FROM Clientes ORDER BY Nome COLLATE NOCASE";
        using var leitor = cmd.ExecuteReader();
        while (leitor.Read())
        {
            lista.Add(new Cliente
            {
                Id = leitor.GetInt64(0),
                Nome = leitor.GetString(1),
                Contacto = leitor.GetString(2),
                Notas = leitor.GetString(3)
            });
        }
        return lista;
    }

    public void Atualizar(Cliente c)
    {
        Validar(c);
        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText =
            "UPDATE Clientes SET Nome = $n, Contacto = $c, Notas = $o WHERE Id = $id";
        cmd.Parameters.AddWithValue("$n", c.Nome.Trim());
        cmd.Parameters.AddWithValue("$c", c.Contacto.Trim());
        cmd.Parameters.AddWithValue("$o", c.Notas.Trim());
        cmd.Parameters.AddWithValue("$id", c.Id);
        cmd.ExecuteNonQuery();
    }

    public void Remover(long id)
    {
        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM Clientes WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }
}
