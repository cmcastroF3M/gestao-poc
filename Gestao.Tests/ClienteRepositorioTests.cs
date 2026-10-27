using Gestao;
using Xunit;

public class ClienteRepositorioTests : IDisposable
{
    private readonly string _ficheiro =
        Path.Combine(Path.GetTempPath(), $"teste_{Guid.NewGuid():N}.db");

    private ClienteRepositorio Criar() => new(_ficheiro);

    public void Dispose()
    {
        if (File.Exists(_ficheiro)) File.Delete(_ficheiro);
    }

    [Fact]
    public void Adicionar_e_listar_devolve_o_cliente()
    {
        var repo = Criar();
        repo.Adicionar(new Cliente { Nome = "Ana Silva", Contacto = "912345678" });

        var lista = repo.Listar();

        Assert.Single(lista);
        Assert.Equal("Ana Silva", lista[0].Nome);
    }

    [Fact]
    public void Atualizar_altera_os_dados()
    {
        var repo = Criar();
        var id = repo.Adicionar(new Cliente { Nome = "Ana" });

        repo.Atualizar(new Cliente { Id = id, Nome = "Ana Maria", Notas = "VIP" });

        var c = repo.Listar().Single();
        Assert.Equal("Ana Maria", c.Nome);
        Assert.Equal("VIP", c.Notas);
    }

    [Fact]
    public void Remover_apaga_o_cliente()
    {
        var repo = Criar();
        var id = repo.Adicionar(new Cliente { Nome = "Rui" });

        repo.Remover(id);

        Assert.Empty(repo.Listar());
    }

    [Fact]
    public void Nome_vazio_e_rejeitado()
    {
        var repo = Criar();
        Assert.Throws<ArgumentException>(() => repo.Adicionar(new Cliente { Nome = "   " }));
    }
}
