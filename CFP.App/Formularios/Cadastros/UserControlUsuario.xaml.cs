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
    /// Interação lógica para UserControlUsuario.xam
    /// </summary>
    public partial class UserControlUsuario : UserControl
    {
        ISession Session;
        Usuario usuario;

        #region Carrega Combos
        private void CarregaCombos()
        {
            cmbTipoUsuario.ItemsSource = Enum.GetValues(typeof(TipoUsuario));
            cmbTipoUsuario.SelectedIndex = 0;
            cmbSituacao.ItemsSource = Enum.GetValues(typeof(Situacao));
            cmbSituacao.SelectedIndex = 0;
        }
        #endregion

        #region Repositorio
        private RepositorioUsuario _repositorio;
        public RepositorioUsuario Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioUsuario(Session);

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
                if (item is PasswordBox)
                    (item as PasswordBox).Password = string.Empty;
            }
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                usuario.Nome = txtNome.Text;
                usuario.Senha = txtSenha.Password;
                usuario.ConfirmaSenha = txtConfirmaSenha.Password;
                usuario.TipoUsuario = (TipoUsuario)cmbTipoUsuario.SelectedIndex;
                usuario.Situacao = (Situacao)cmbSituacao.SelectedIndex;
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
            if (usuario != null)
            {
                txtCodigo.Text = usuario.Id.ToString();
                txtNome.Text = usuario.Nome;
                txtSenha.Password = usuario.Senha;
                txtConfirmaSenha.Password = usuario.ConfirmaSenha;
                cmbTipoUsuario.SelectedIndex = usuario.TipoUsuario.GetHashCode();
                cmbSituacao.SelectedIndex = usuario.Situacao.GetHashCode();
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
        public UserControlUsuario(Usuario _usuario, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            usuario = _usuario;
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
                (this.Parent as StackPanel).Children.Remove(this);
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
                    usuario = new Usuario();
                    LimpaCampos();
                    ControleAcessoCadastro();
                    CorPadrãoBotaoPesquisar();
                }
                else
                {
                    try
                    {
                        usuario = Repositorio.ObterPorId(Int64.Parse(txtCodigo.Text));
                        if (usuario != null)
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
                        usuario = null;
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
            PesquisaUsuarios p = new PesquisaUsuarios();
            p.ShowDialog();
            if (p.objeto != null)
            {
                usuario = p.objeto;
                PreencheCampos();
                ControleAcessoCadastro();
                CorPadrãoBotaoPesquisar();
            }
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (PreencheObjeto())
            {
                if ((usuario.Id == 0) && (String.IsNullOrEmpty(txtCodigo.Text)))
                {
                    usuario.DataGeracao = DateTime.Now;
                    Repositorio.Salvar(usuario);
                    txtCodigo.Text = usuario.Id.ToString();
                }
                else
                {
                    usuario.DataAlteracao = DateTime.Now;
                    Repositorio.Alterar(usuario);
                }

                ControleAcessoInicial();
                FocoNoCampoCodigo();
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (usuario != null)
            {
                MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + usuario.Nome + " ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
                {
                    Repositorio.Excluir(usuario);
                    LimpaCampos();
                    ControleAcessoInicial();
                }
            }
        }
    }
}
