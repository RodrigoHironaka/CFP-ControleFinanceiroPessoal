using CFP.App.Formularios.Pesquisas;
using Dominio.Dominio;
using Dominio.ObjetoValor;
using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CFP.App.Formularios.Cadastros
{
    /// <summary>
    /// Interação lógica para UserControlCadastroFormaPagamento.xam
    /// </summary>
    public partial class UserControlCadastroFormaPagamento : UserControl
    {
        ISession Session;
        FormaPagamento formaPagamento;

        #region Carrega Combos
        private void CarregaCombos()
        {
            //carrega combo Situacao e define Ativo 
            cmbSituacao.ItemsSource = Enum.GetValues(typeof(Situacao));
        }
        #endregion

        #region Repositorio
        private RepositorioFormaPagamento _repositorio;
        public RepositorioFormaPagamento Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioFormaPagamento(Session);

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
                    (item as ComboBox).SelectedIndex = -1;
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
                formaPagamento.Nome = txtNome.Text;
                formaPagamento.QtdParcelas = Int32.Parse(txtQtdParcelas.Text);
                formaPagamento.DiasParaVencimento = Int32.Parse(txtDiasVencimento.Text);
                formaPagamento.Situacao = (Situacao)cmbSituacao.SelectedIndex;
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
            if(formaPagamento != null)
            {
                txtCodigo.Text = formaPagamento.Id.ToString();
                txtNome.Text = formaPagamento.Nome;
                txtQtdParcelas.Text = formaPagamento.QtdParcelas.ToString();
                txtDiasVencimento.Text = formaPagamento.DiasParaVencimento.ToString();
                cmbSituacao.SelectedIndex = formaPagamento.Situacao.GetHashCode();
            }
        }
        #endregion

        public UserControlCadastroFormaPagamento(FormaPagamento _formapagamento, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            formaPagamento = _formapagamento;
        }

        //Verifica se GridCampos (cadastro) esta ativo e limpa os campos e depois desativa
        //ou remove usercontrol e volta para botoes
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

        //Ao iniciar UserControl bloqueia paineis e carrega combos necessarios
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ControleAcessoInicial();
            CarregaCombos();
        }

        //ao teclar em enter é verificado se campo codigo esta digitado e libera campos para inclusao ou alteração
        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // verifico se campo codigo esta preenchido para incluir novos dados ou carregar um existente
                if (String.IsNullOrEmpty(txtCodigo.Text))
                {
                    formaPagamento = new FormaPagamento();
                    LimpaCampos();
                    ControleAcessoCadastro();
                    var converter = new System.Windows.Media.BrushConverter();
                    var HexaToBrush = (Brush)converter.ConvertFromString("#FF1F3D68");
                    btPesquisar.Background = HexaToBrush;
                }
                else
                {
                    try
                    {
                        formaPagamento = Repositorio.ObterPorId(Int64.Parse(txtCodigo.Text));
                        if (formaPagamento != null)
                        {
                            PreencheCampos();
                            ControleAcessoCadastro();
                            var converter = new System.Windows.Media.BrushConverter();
                            var HexaToBrush = (Brush)converter.ConvertFromString("#FF1F3D68");
                            btPesquisar.Background = HexaToBrush;
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
                        formaPagamento = null;
                    }
                }
            }
        }

        //aceita apenas numero (usando Regex)
        private void txtCodigo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        //Bloqueia tecla de espaço
        private void txtCodigo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        //aceita apenas numero (usando Regex)
        private void txtQtdParcelas_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        //Bloqueia tecla de espaço
        private void txtQtdParcelas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        //aceita apenas numero (usando Regex)
        private void txtDiasVencimento_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        //Bloqueia tecla de espaço
        private void txtDiasVencimento_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void btPesquisar_Click(object sender, RoutedEventArgs e)
        {
            JanelaPesquisas p = new JanelaPesquisas();
            p.ShowDialog();
            if(p.objeto != null)
            {
                formaPagamento = p.objeto;
                PreencheCampos();
                ControleAcessoCadastro();
            }
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (PreencheObjeto())
            {
                if ((formaPagamento.Id == 0) && (String.IsNullOrEmpty(txtCodigo.Text)))
                {
                    formaPagamento.DataGeracao = DateTime.Now;
                    Repositorio.Salvar(formaPagamento);
                    txtCodigo.Text = formaPagamento.Id.ToString();
                }
                else
                {
                    formaPagamento.DataAlteracao = DateTime.Now;
                    Repositorio.Alterar(formaPagamento);
                }

                ControleAcessoInicial();
                FocoNoCampoCodigo();
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            if(formaPagamento != null)
            {
                MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + formaPagamento.Nome + " ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
                {
                    Repositorio.Excluir(formaPagamento);
                    LimpaCampos();
                    ControleAcessoInicial();
                   
                }
            }
  
        }
    }
}
