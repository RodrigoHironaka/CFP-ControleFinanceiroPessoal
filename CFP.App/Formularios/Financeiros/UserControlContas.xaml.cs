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

namespace CFP.App.Formularios.Financeiros
{
    /// <summary>
    /// Interação lógica para UserControlContas.xam
    /// </summary>
    public partial class UserControlContas : UserControl
    {
        ISession Session;
        Conta conta;

        #region Carrega Combos
        private void CarregaCombos()
        {
            //carrega combo Situacao e define Ativo 
            cmbSituacao.ItemsSource = Enum.GetValues(typeof(SituacaoConta));
            cmbSituacao.SelectedIndex = 0;
            cmbTipoConta.ItemsSource = Enum.GetValues(typeof(TipoConta));
            cmbTipoConta.SelectedIndex = 0;
            cmbTipoPeriodo.ItemsSource = Enum.GetValues(typeof(TipoPeriodo));
            cmbTipoConta.SelectedIndex = 0;

            cmbTipoGasto.ItemsSource = new RepositorioGrupoGasto(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
                .OrderBy(x => x.Nome)
                .ToList<GrupoGasto>();
            cmbTipoGasto.SelectedIndex = 0;

            cmbTipoPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<FormaPagamento>();
            cmbTipoPagamento.SelectedIndex = 0;

            cmbReferenciaPessoa.ItemsSource = new RepositorioPessoa(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<Pessoa>();
            cmbReferenciaPessoa.SelectedIndex = 0;


        }
        #endregion

        #region Repositorio
        private RepositorioConta _repositorio;
        public RepositorioConta Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioConta(Session);

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
                if (item is DatePicker)
                    (item as DatePicker).Text = string.Empty;
                if (item is Grid)
                {
                    foreach (var grid in GridRow4.Children)
                    {
                        if (grid is TextBox)
                            (grid as TextBox).Text = string.Empty;
                        if (item is ComboBox)
                            (item as ComboBox).SelectedIndex = 0;
                    }
                    foreach (var grid in GridRow5.Children)
                    {
                        if (grid is TextBox)
                            (grid as TextBox).Text = string.Empty;
                        if (item is ComboBox)
                            (item as ComboBox).SelectedIndex = 0;
                    }

                    foreach (var grid in GridListaDataGrid.Children)
                    {
                        if (grid is DataGrid)
                            (grid as DataGrid).Items.Clear();

                    }
                }
            }
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                //tab Geral
                conta.Nome = txtNome.Text;
                conta.TipoConta = (TipoConta)cmbTipoConta.SelectedIndex;
                conta.GrupoGasto = (GrupoGasto)cmbTipoGasto.SelectedItem;
                conta.TipoPeriodo = (TipoPeriodo)cmbTipoPeriodo.SelectedIndex;
                conta.Situacao = (SituacaoConta)cmbSituacao.SelectedIndex;
                conta.DataEmissao = (DateTime)(txtEmissao.Text != string.Empty ? txtEmissao.SelectedDate : null);
                conta.DataVencimento = (DateTime)(txtVencimento.Text != string.Empty ? txtVencimento.SelectedDate : null);
                conta.ValorTotal = txtValorTotal.Text != string.Empty ? Decimal.Parse(txtValorTotal.Text) : 0;
                conta.QtdParcelas = txtQtdParcelas.Text != string.Empty ? Int64.Parse(txtQtdParcelas.Text) : 0;
                conta.ValorParcela = txtValorParcela.Text != string.Empty ? Decimal.Parse(txtValorParcela.Text) : 0;
                conta.Pessoa = (Pessoa)(cmbReferenciaPessoa.SelectedItem ?? null);
                conta.NumeroDocumento = Int64.Parse(txtNumDocumento.Text);
                conta.TipoFormaPagamento = (FormaPagamento)cmbTipoPagamento.SelectedItem;
                conta.Observacao = txtObservacao.Text;
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
            if (conta != null)
            {
                txtCodigo.Text = conta.Id.ToString();
                txtNome.Text = conta.Nome.ToString();
                cmbTipoConta.SelectedIndex = conta.TipoConta.GetHashCode();
                cmbTipoConta.SelectedItem = conta.GrupoGasto;
                cmbTipoConta.SelectedIndex = conta.TipoPeriodo.GetHashCode();
                cmbSituacao.SelectedIndex = conta.Situacao.GetHashCode();
                txtEmissao.Text = conta.DataEmissao.ToString();
                txtVencimento.Text = conta.DataVencimento.ToString();
                txtValorTotal.Text = conta.ValorTotal.ToString();
                txtQtdParcelas.Text = conta.QtdParcelas.ToString();
                txtValorParcela.Text = conta.ValorParcela.ToString();
                cmbReferenciaPessoa.SelectedItem = conta.Pessoa;
                txtNumDocumento.Text = conta.NumeroDocumento.ToString();
                cmbTipoPagamento.SelectedItem = conta.TipoFormaPagamento;
                txtObservacao.Text = conta.Observacao;
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
        public UserControlContas(Conta _conta, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            conta = _conta;
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
                    conta = new Conta();
                    LimpaCampos();
                    ControleAcessoCadastro();
                    CorPadrãoBotaoPesquisar();
                }
                else
                {
                    try
                    {
                        conta = Repositorio.ObterPorId(Int64.Parse(txtCodigo.Text));
                        if (conta != null)
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
                        conta = null;
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
            //PesquisaContas p = new PesquisaContas();
            //p.ShowDialog();
            //if (p.objeto != null)
            //{
            //    conta = p.objeto;
            //    PreencheCampos();
            //    ControleAcessoCadastro();
            //    CorPadrãoBotaoPesquisar();
            //}
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (PreencheObjeto())
            {
                if ((conta.Id == 0) && (String.IsNullOrEmpty(txtCodigo.Text)))
                {
                    conta.DataGeracao = DateTime.Now;
                    Repositorio.Salvar(conta);
                    txtCodigo.Text = conta.Id.ToString();
                }
                else
                {
                    conta.DataAlteracao = DateTime.Now;
                    Repositorio.Alterar(conta);
                }

                ControleAcessoInicial();
                FocoNoCampoCodigo();
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (conta != null)
            {
                MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + conta.Nome + " ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
                {
                    Repositorio.Excluir(conta);
                    LimpaCampos();
                    ControleAcessoInicial();

                }
            }
        }
    }
}
