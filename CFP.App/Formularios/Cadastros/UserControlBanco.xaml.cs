using CFP.App.Formularios.Pesquisas;
using Dominio.Dominio;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFP.App.Formularios.Cadastros
{
    /// <summary>
    /// Interação lógica para UserControlBanco.xam
    /// </summary>
    public partial class UserControlBanco : UserControl
    {
        ISession Session;
        Banco banco;

        #region Carrega Combos
        private void CarregaCombos()
        {
            //carrega combo Situacao e define Ativo 
            cmbSituacao.ItemsSource = Enum.GetValues(typeof(Situacao));
            cmbSituacao.SelectedIndex = 0;
            cmbTipoConta.ItemsSource = Enum.GetValues(typeof(TipoContaBanco));
            cmbTipoConta.SelectedIndex = 0;

            cmbPessoa.ItemsSource = new RepositorioPessoa(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<Pessoa>();
            cmbPessoa.SelectedIndex = 0;
        }
        #endregion

        #region Repositorio
        private RepositorioBanco _repositorio;
        public RepositorioBanco Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioBanco(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        #region Controle de acessos Inicial e Cadastro
        private void ControleAcessoInicial()
        {
            //Bloqueando
            GridCampos.IsEnabled = !GridCampos.IsEnabled;
            btSalvar.IsEnabled = !btSalvar.IsEnabled;
            btExcluir.IsEnabled = !btExcluir.IsEnabled;

            //Desbloqueando
            btPesquisar.IsEnabled = true;
            txtCodigo.IsEnabled = true;

            txtCodigo.Focus();
            txtCodigo.SelectAll();
        }

        private void ControleAcessoCadastro()
        {
            //Bloqueando
            btPesquisar.IsEnabled = !btPesquisar.IsEnabled;
            txtCodigo.IsEnabled = !txtCodigo.IsEnabled;

            //Desbloqueando
            GridCampos.IsEnabled = true;
            btSalvar.IsEnabled = true;
            btExcluir.IsEnabled = true;

            //define o foco no primeiro campo
            txtNome.Focus();
            txtNome.Select(txtNome.Text.Length, 0);

        }
        #endregion

        #region Selecão e Foco no campo Codigo 
        private void FocoNoCampoCodigo()
        {
            txtCodigo.SelectAll();
            txtCodigo.Focus();
        }
        #endregion

        #region Limpa os campos do Cadastro
        public void LimpaCampos()
        {
            foreach (var item in GridControls.Children)
            {
                if (item is TextBox)
                    (item as TextBox).Text = string.Empty;
                if (item is ComboBox)
                    (item as ComboBox).SelectedIndex = 0;
                if (item is CheckBox)
                    (item as CheckBox).IsChecked = false;
                if (item is RadioButton)
                    (item as RadioButton).IsChecked = false;
            }
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                banco.UsuarioCriacao = MainWindow.UsuarioLogado;
                banco.Nome = txtNome.Text;
                banco.Situacao = (Situacao)cmbSituacao.SelectedIndex;
                banco.TipoContaBanco = (TipoContaBanco)cmbTipoConta.SelectedIndex;
                banco.PessoaBanco = (Pessoa)cmbPessoa.SelectedItem;
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

        #region Preenche campos no user control
        private void PreencheCampos()
        {
            if (banco != null)
            {
                txtCodigo.Text = banco.Id.ToString();
                txtNome.Text = banco.Nome;
                cmbSituacao.SelectedIndex = banco.Situacao.GetHashCode();
                cmbTipoConta.SelectedIndex = banco.TipoContaBanco.GetHashCode();
                cmbPessoa.SelectedItem = banco.PessoaBanco;
            }
        }
        #endregion

        #region Definindo Cor Padrão do botão Pesquisar #FF1F3D68 
        public void CorPadrãoBotaoPesquisar()
        {
            var converter = new System.Windows.Media.BrushConverter();
            var HexaToBrush = (Brush)converter.ConvertFromString("#FF1F3D68");
            btPesquisar.Background = HexaToBrush;
        }
        #endregion
        public UserControlBanco(Banco _banco, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            banco = _banco;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (GridCampos.IsEnabled)
            {
                ControleAcessoInicial();
                FocoNoCampoCodigo();
                LimpaCampos();
            }
            else
            {
                (this.Parent as Grid).Children.Remove(this);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ControleAcessoInicial();
            CarregaCombos();
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // verifico se campo codigo esta preenchido para incluir novos dados ou carregar um existente
                if (String.IsNullOrEmpty(txtCodigo.Text))
                {
                    banco = new Banco();
                    LimpaCampos();
                    ControleAcessoCadastro();
                    CorPadrãoBotaoPesquisar();
                }
                else
                {
                    try
                    {
                        banco = Repositorio.ObterPorId(Int64.Parse(txtCodigo.Text));
                        if (banco != null)
                        {
                            PreencheCampos();
                            ControleAcessoCadastro();
                            CorPadrãoBotaoPesquisar();
                        }
                        else
                        {
                            txtCodigo.SelectAll();
                            txtCodigo.Focus();
                            btPesquisar.Background = Brushes.DarkRed;
                        }
                    }
                    catch (Exception)
                    {
                        banco = null;
                    }
                }
            }
        }

        private void txtCodigo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtCodigo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void btPesquisar_Click(object sender, RoutedEventArgs e)
        {
            PesquisaBancos p = new PesquisaBancos();
            p.ShowDialog();
            if (p.objeto != null)
            {
                banco = Repositorio.ObterPorId(p.objeto.Id);
                PreencheCampos();
                ControleAcessoCadastro();
                CorPadrãoBotaoPesquisar();
            }
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(txtNome.Text) || cmbTipoConta.SelectedIndex == -1 || cmbPessoa.SelectedItem == null)
            {
                MessageBox.Show("Todos os campos são obrigatórios! Por favor verifique.", "Atencao", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (PreencheObjeto())
            {
                if ((banco.Id == 0) && (String.IsNullOrEmpty(txtCodigo.Text)))
                {
                    banco.DataGeracao = DateTime.Now;
                    Repositorio.Salvar(banco);
                    txtCodigo.Text = banco.Id.ToString();
                }
                else
                {
                    banco.DataAlteracao = DateTime.Now;
                    Repositorio.Alterar(banco);
                }

                ControleAcessoInicial();
                FocoNoCampoCodigo();
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (banco != null)
            {
                MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + banco.Nome + " ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
                {
                    Repositorio.Excluir(banco);
                    LimpaCampos();
                    ControleAcessoInicial();

                }
            }
        }
    }
}
