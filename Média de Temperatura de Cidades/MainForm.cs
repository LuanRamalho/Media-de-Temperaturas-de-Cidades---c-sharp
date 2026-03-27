using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Globalization;

namespace AppCidades
{
    public partial class MainForm : Form
    {
        private string nomeSelecionado = null; // Substituiu o idSelecionado
        private DataGridView grid;
        private TextBox txtCidade, txtPais, txtVerao, txtOutono, txtInverno, txtPrimavera, txtBusca;
        private ComboBox cbContinente;
        private Button btnAdicionar, btnSalvar, btnExcluir;

        public MainForm()
        {
            ConfigurarInterface();
            DatabaseHelper.InicializarBanco();
            AtualizarGrid();
        }

        private void ConfigurarInterface()
        {
            this.Text = "Climatologia Mundial - JSON NoSQL";
            this.Size = new Size(900, 650);
            this.BackColor = Color.AliceBlue;

            Panel pnlForm = new Panel { Dock = DockStyle.Top, Height = 220, Padding = new Padding(10) };
            
            Label lblCidade = new Label { Text = "Cidade:", Location = new Point(20, 20), AutoSize = true };
            txtCidade = new TextBox { Location = new Point(100, 20), Width = 150 };

            Label lblPais = new Label { Text = "País:", Location = new Point(20, 55), AutoSize = true };
            txtPais = new TextBox { Location = new Point(100, 55), Width = 150 };

            Label lblCont = new Label { Text = "Continente:", Location = new Point(20, 90), AutoSize = true };
            cbContinente = new ComboBox { Location = new Point(100, 90), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbContinente.Items.AddRange(new string[] { "África", "América", "Ásia", "Europa", "Oceania" });

            Label lblV = new Label { Text = "Verão (°C):", Location = new Point(280, 20), AutoSize = true };
            txtVerao = new TextBox { Location = new Point(380, 20), Width = 60 };
            
            Label lblO = new Label { Text = "Outono (°C):", Location = new Point(280, 55), AutoSize = true };
            txtOutono = new TextBox { Location = new Point(380, 55), Width = 60 };

            Label lblI = new Label { Text = "Inverno (°C):", Location = new Point(280, 90), AutoSize = true };
            txtInverno = new TextBox { Location = new Point(380, 90), Width = 60 };

            Label lblP = new Label { Text = "Primavera (°C):", Location = new Point(280, 125), AutoSize = true };
            txtPrimavera = new TextBox { Location = new Point(380, 125), Width = 60 };

            btnAdicionar = new Button { Text = "Adicionar", Location = new Point(20, 170), Width = 100, BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
            btnAdicionar.Click += (s, e) => AcaoSalvar();

            btnSalvar = new Button { Text = "Salvar Alteração", Location = new Point(130, 170), Width = 120, BackColor = Color.LightSkyBlue, Enabled = false, FlatStyle = FlatStyle.Flat };
            btnSalvar.Click += (s, e) => AcaoSalvar();

            btnExcluir = new Button { Text = "Excluir Selecionado", Location = new Point(260, 170), Width = 130, BackColor = Color.Salmon, FlatStyle = FlatStyle.Flat };
            btnExcluir.Click += (s, e) => AcaoExcluir();

            txtBusca = new TextBox { Location = new Point(550, 170), Width = 180 };
            Button btnBusca = new Button { Text = "Buscar", Location = new Point(740, 168), BackColor = Color.Gold, FlatStyle = FlatStyle.Flat };
            btnBusca.Click += (s, e) => AtualizarGrid(txtBusca.Text);

            pnlForm.Controls.AddRange(new Control[] { lblCidade, txtCidade, lblPais, txtPais, lblCont, cbContinente, 
                                                    lblV, txtVerao, lblO, txtOutono, lblI, txtInverno, lblP, txtPrimavera,
                                                    btnAdicionar, btnSalvar, btnExcluir, txtBusca, btnBusca });

            grid = new DataGridView { 
                Dock = DockStyle.Fill, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.DoubleClick += (s, e) => CarregarParaEdicao();

            this.Controls.Add(grid);
            this.Controls.Add(pnlForm);
        }

        private void AtualizarGrid(string busca = "")
        {
            grid.DataSource = null;
            grid.DataSource = DatabaseHelper.Listar(busca);
            
            string[] colsNumericas = { "Verao", "Outono", "Inverno", "Primavera", "Media" };
            foreach (var col in colsNumericas)
            {
                if (grid.Columns[col] != null)
                    grid.Columns[col].DefaultCellStyle.Format = "N1";
            }
        }

        private double ParseDouble(string texto)
        {
            string limpo = texto.Replace(',', '.');
            return double.Parse(limpo, CultureInfo.InvariantCulture);
        }

        private void AcaoSalvar()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCidade.Text) || string.IsNullOrWhiteSpace(cbContinente.Text))
                {
                    MessageBox.Show("Preencha ao menos o nome da cidade e o continente!");
                    return;
                }

                double v = Math.Round(ParseDouble(txtVerao.Text), 1);
                double o = Math.Round(ParseDouble(txtOutono.Text), 1);
                double i = Math.Round(ParseDouble(txtInverno.Text), 1);
                double p = Math.Round(ParseDouble(txtPrimavera.Text), 1);
                double media = Math.Round((v + o + i + p) / 4.0, 1);

                var cid = new Cidade {
                    Nome = txtCidade.Text,
                    Pais = txtPais.Text,
                    Continente = cbContinente.Text,
                    Verao = v, Outono = o, Inverno = i, Primavera = p,
                    Media = media
                };

                DatabaseHelper.Salvar(cid, nomeSelecionado);
                MessageBox.Show("Operação realizada com sucesso!");
                LimparCampos();
                AtualizarGrid();
            }
            catch (FormatException)
            {
                MessageBox.Show("Erro: Use apenas números e vírgula/ponto nas temperaturas.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado: " + ex.Message);
            }
        }

        private void CarregarParaEdicao()
        {
            if (grid.SelectedRows.Count > 0)
            {
                var cid = (Cidade)grid.SelectedRows[0].DataBoundItem;
                nomeSelecionado = cid.Nome;
                txtCidade.Text = cid.Nome;
                txtPais.Text = cid.Pais;
                cbContinente.Text = cid.Continente;
                txtVerao.Text = cid.Verao.ToString("N1");
                txtOutono.Text = cid.Outono.ToString("N1");
                txtInverno.Text = cid.Inverno.ToString("N1");
                txtPrimavera.Text = cid.Primavera.ToString("N1");

                btnAdicionar.Enabled = false;
                btnSalvar.Enabled = true;
            }
        }

        private void AcaoExcluir()
        {
            if (grid.SelectedRows.Count > 0)
            {
                var cid = (Cidade)grid.SelectedRows[0].DataBoundItem;
                var res = MessageBox.Show($"Deseja excluir {cid.Nome}?", "Confirmar", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    DatabaseHelper.Excluir(cid.Nome);
                    AtualizarGrid();
                }
            }
        }

        private void LimparCampos()
        {
            nomeSelecionado = null;
            txtCidade.Clear();
            txtPais.Clear();
            txtVerao.Clear();
            txtOutono.Clear();
            txtInverno.Clear();
            txtPrimavera.Clear();
            cbContinente.SelectedIndex = -1;
            btnAdicionar.Enabled = true;
            btnSalvar.Enabled = false;
        }
    }
}
