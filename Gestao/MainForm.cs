namespace Gestao;

public class MainForm : Form
{
    private readonly ClienteRepositorio _repo;
    private readonly DataGridView _grelha = new();
    private readonly TextBox _nome = new();
    private readonly TextBox _contacto = new();
    private readonly TextBox _notas = new() { Multiline = true };
    private long? _idAtual;

    public MainForm(ClienteRepositorio repo)
    {
        _repo = repo;
        Text = "Gestão de Clientes";
        Width = 800;
        Height = 580;
        StartPosition = FormStartPosition.CenterScreen;

        _grelha.Dock = DockStyle.Fill;
        _grelha.ReadOnly = true;
        _grelha.AllowUserToAddRows = false;
        _grelha.MultiSelect = false;
        _grelha.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grelha.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grelha.CellClick += (_, e) =>
        {
            if (e.RowIndex >= 0 && _grelha.Rows[e.RowIndex].DataBoundItem is Cliente c)
                Carregar(c);
        };

        var tabela = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 200,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(10)
        };
        tabela.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        tabela.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tabela.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        tabela.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        tabela.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tabela.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        Linha(tabela, 0, "Nome", _nome);
        Linha(tabela, 1, "Contacto", _contacto);
        Linha(tabela, 2, "Notas", _notas);

        var botoes = new FlowLayoutPanel { Dock = DockStyle.Fill };
        var guardar = new Button { Text = "Guardar", Width = 100 };
        var novo = new Button { Text = "Novo", Width = 100 };
        var apagar = new Button { Text = "Apagar", Width = 100 };
        guardar.Click += (_, _) => Guardar();
        novo.Click += (_, _) => Novo();
        apagar.Click += (_, _) => Apagar();
        botoes.Controls.AddRange(new Control[] { guardar, novo, apagar });
        tabela.Controls.Add(botoes, 1, 3);

        Controls.Add(_grelha);
        Controls.Add(tabela);

        Novo();
    }

    private static void Linha(TableLayoutPanel t, int linha, string rotulo, Control campo)
    {
        t.Controls.Add(new Label { Text = rotulo, AutoSize = true, Anchor = AnchorStyles.Left }, 0, linha);
        campo.Dock = DockStyle.Fill;
        t.Controls.Add(campo, 1, linha);
    }

    private void Carregar(Cliente c)
    {
        _idAtual = c.Id;
        _nome.Text = c.Nome;
        _contacto.Text = c.Contacto;
        _notas.Text = c.Notas;
    }

    private void Novo()
    {
        _idAtual = null;
        _nome.Clear();
        _contacto.Clear();
        _notas.Clear();
        _grelha.DataSource = _repo.Listar();
        _grelha.ClearSelection();
        _nome.Focus();
    }

    private void Guardar()
    {
        try
        {
            var c = new Cliente
            {
                Id = _idAtual ?? 0,
                Nome = _nome.Text,
                Contacto = _contacto.Text,
                Notas = _notas.Text
            };
            if (_idAtual is null) _repo.Adicionar(c);
            else _repo.Atualizar(c);
            Novo();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Atenção");
        }
    }

    private void Apagar()
    {
        if (_idAtual is null)
        {
            MessageBox.Show("Selecione primeiro um cliente na lista.", "Atenção");
            return;
        }
        var resposta = MessageBox.Show("Apagar este cliente?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (resposta == DialogResult.Yes)
        {
            _repo.Remover(_idAtual.Value);
            Novo();
        }
    }
}
