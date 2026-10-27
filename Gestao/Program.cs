namespace Gestao;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var pasta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Gestao");
        Directory.CreateDirectory(pasta);

        var repo = new ClienteRepositorio(Path.Combine(pasta, "gestao.db"));
        Application.Run(new MainForm(repo));
    }
}
