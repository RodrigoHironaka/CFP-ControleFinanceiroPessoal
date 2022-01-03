using NHibernate;
using System;
using Dominio.Dominio;
using Repositorio.Repositorios;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Dominio.ObjetoValor;
using System.Text.RegularExpressions;
using CFP.App.Formularios.Pesquisas;
using CFP.Repositorio.Repositorio;
using CFP.Dominio.Dominio;

namespace CFP.App.Formularios.Cadastros
{
    /// <summary>
    /// Interação lógica para UserControlTipoSubGasto.xam
    /// </summary>
    public partial class UserControlTipoSubGasto : UserControl
    {
        ISession Session;
        SubGrupoGasto subGrupoGasto;

        #region Carrega Combos
        private void CarregaCombos()
        {
            //carrega combo Situacao e define Ativo 
            cmbSituacao.ItemsSource = Enum.GetValues(typeof(Situacao));
            cmbSituacao.SelectedIndex = 0;

            cmbGrupoGasto.ItemsSource = new RepositorioGrupoGasto(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
                .OrderBy(x => x.Nome)
                .ToList();
        }
        #endregion

        #region Repositorio
        private RepositorioSubGrupoGasto _repositorio;
        public RepositorioSubGrupoGasto Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioSubGrupoGasto(Session);

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
                subGrupoGasto.Nome = txtNome.Text;
                subGrupoGasto.GrupoGasto = (GrupoGasto)cmbGrupoGasto.SelectedItem;
                subGrupoGasto.Situacao = (Situacao)cmbSituacao.SelectedIndex;
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
            if (subGrupoGasto != null)
            {
                txtCodigo.Text = subGrupoGasto.Id.ToString();
                txtNome.Text = subGrupoGasto.Nome;
                cmbGrupoGasto.SelectedItem = subGrupoGasto.GrupoGasto;
                cmbSituacao.SelectedIndex = subGrupoGasto.Situacao.GetHashCode();
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

        public UserControlTipoSubGasto(SubGrupoGasto _subGrupoGasto, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            subGrupoGasto = _subGrupoGasto;
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
                    subGrupoGasto = new SubGrupoGasto();
                    LimpaCampos();
                    ControleAcessoCadastro();
                    CorPadrãoBotaoPesquisar();
                }
                else
                {
                    try
                    {
                        subGrupoGasto = Repositorio.ObterPorId(Int64.Parse(txtCodigo.Text));
                        if (subGrupoGasto != null)
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
                        subGrupoGasto = null;
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
            PesquisaSubGrupoGasto p = new PesquisaSubGrupoGasto();
            p.ShowDialog();
            if (p.objeto != null)
            {
                subGrupoGasto = Repositorio.ObterPorId(p.objeto.Id);
                PreencheCampos();
                ControleAcessoCadastro();
                CorPadrãoBotaoPesquisar();
            }
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (PreencheObjeto())
            {
                if ((subGrupoGasto.Id == 0) && (String.IsNullOrEmpty(txtCodigo.Text)))
                {
                    subGrupoGasto.DataGeracao = DateTime.Now;
                    subGrupoGasto.UsuarioCriacao = MainWindow.UsuarioLogado;
                    Repositorio.Salvar(subGrupoGasto);
                    txtCodigo.Text = subGrupoGasto.Id.ToString();
                }
                else
                {
                    subGrupoGasto.DataAlteracao = DateTime.Now;
                    subGrupoGasto.UsuarioAlteracao = MainWindow.UsuarioLogado;
                    Repositorio.Alterar(subGrupoGasto);
                }

                ControleAcessoInicial();
                FocoNoCampoCodigo();
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (subGrupoGasto != null)
                {
                    MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + subGrupoGasto.Nome + " ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        Repositorio.Excluir(subGrupoGasto);
                        LimpaCampos();
                        ControleAcessoInicial();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Não é possível excluir esse registro, ele esta sendo usado em outro lugar! Por favor inative o registro para não utilizar mais.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                Session.Clear();
            }
        }
    }
}
