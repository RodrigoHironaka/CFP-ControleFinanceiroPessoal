using Dominio.Dominio;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            lblSituacao.Text = SituacaoConta.Finalizado.ToString();
            cmbTipoConta.ItemsSource = Enum.GetValues(typeof(TipoConta));
            cmbTipoConta.SelectedIndex = 0;
            cmbTipoPeriodo.ItemsSource = Enum.GetValues(typeof(TipoPeriodo));
            cmbTipoPeriodo.SelectedIndex = 0;

            cmbTipoGasto.ItemsSource = new RepositorioGrupoGasto(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
                .OrderBy(x => x.Nome)
                .ToList<GrupoGasto>();
            cmbTipoGasto.SelectedIndex = 0;

            cmbFormaCompra.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.QtdParcelas > 0)
               .OrderBy(x => x.Nome)
               .ToList<FormaPagamento>();
            cmbFormaCompra.SelectedIndex = 0;

            cmbReferenciaPessoa.ItemsSource = new RepositorioPessoa(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<Pessoa>();


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
            lblSituacao.Text = SituacaoConta.Pendente.ToString();
            foreach (var item in GridControls.Children)
            {
                if (item is TabControl)
                {
                    tabItemGeral.IsSelected = true;
                    foreach (var gridControls2 in GridControls2.Children)
                    {
                        if (gridControls2 is TextBox)
                            (gridControls2 as TextBox).Text = string.Empty;
                        if (gridControls2 is ComboBox)
                            (gridControls2 as ComboBox).SelectedIndex = 0;
                        if (gridControls2 is CheckBox)
                            (gridControls2 as CheckBox).IsChecked = false;
                        if (gridControls2 is RadioButton)
                            (gridControls2 as RadioButton).IsChecked = false;
                        if (gridControls2 is DatePicker)
                            (gridControls2 as DatePicker).Text = string.Empty;

                        if (gridControls2 is Grid)
                        {
                            foreach (var gridRow4 in GridRow4.Children)
                            {
                                if (gridRow4 is TextBox)
                                    (gridRow4 as TextBox).Text = string.Empty;
                                if (gridRow4 is ComboBox)
                                    (gridRow4 as ComboBox).SelectedIndex = 0;
                            }
                            foreach (var gridRow5 in GridRow5.Children)
                            {
                                if (gridRow5 is TextBox)
                                    (gridRow5 as TextBox).Text = string.Empty;
                                if (gridRow5 is ComboBox)
                                    (gridRow5 as ComboBox).SelectedIndex = 0;
                            }
                        }
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
                conta.Situacao = ((SituacaoConta)Enum.Parse(typeof(SituacaoConta), lblSituacao.Text));
                conta.DataEmissao = (DateTime)(txtEmissao.Text != string.Empty ? txtEmissao.SelectedDate : conta.DataEmissao.GetValueOrDefault());
                conta.DataVencimento = (DateTime)(txtVencimento.Text != string.Empty ? txtVencimento.SelectedDate : conta.DataVencimento.GetValueOrDefault());
                conta.ValorTotal = txtValorTotal.Text != string.Empty ? Decimal.Parse(txtValorTotal.Text) : 0;
                conta.QtdParcelas = txtQtdParcelas.Text != string.Empty ? Int64.Parse(txtQtdParcelas.Text) : 0;
                conta.ValorParcela = txtValorParcela.Text != string.Empty ? Decimal.Parse(txtValorParcela.Text) : 0;
                conta.Pessoa = (Pessoa)(cmbReferenciaPessoa.SelectedItem ?? null);
                conta.NumeroDocumento = txtNumDocumento.Text != string.Empty ? Int64.Parse(txtNumDocumento.Text) : 0;
                conta.FormaCompra = (FormaPagamento)cmbFormaCompra.SelectedItem;
                conta.Observacao = txtObservacao.Text;
                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
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
                lblSituacao.Text = conta.Situacao.ToString();
                txtEmissao.Text = conta.DataEmissao != DateTime.MinValue ? conta.DataEmissao.ToString() : string.Empty;
                txtVencimento.Text = conta.DataVencimento != DateTime.MinValue ? conta.DataVencimento.ToString() : string.Empty;
                txtValorTotal.Text = conta.ValorTotal > 0 ? conta.ValorTotal.ToString() : string.Empty;
                txtQtdParcelas.Text = conta.QtdParcelas > 0 ? conta.QtdParcelas.ToString() : string.Empty;
                txtValorParcela.Text = conta.ValorParcela > 0 ? conta.ValorParcela.ToString() : string.Empty;
                cmbReferenciaPessoa.SelectedItem = conta.Pessoa;
                txtNumDocumento.Text = conta.NumeroDocumento > 0 ? conta.NumeroDocumento.ToString() : string.Empty;
                cmbFormaCompra.SelectedItem = conta.FormaCompra;
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

        #region Vericando Tipo de Periodo
        private void VerificaTipoPeriodo()
        {
            switch (cmbTipoPeriodo.SelectedIndex)
            {
                case 0:
                    txtQtdParcelas.IsEnabled = false;
                    txtValorParcela.IsEnabled = false;
                    txtValorParcela.IsReadOnly = true;
                    btParcelas.IsEnabled = false;
                    txtQtdParcelas.Clear();
                    txtValorParcela.Clear();
                    break;
                case 1:
                    txtQtdParcelas.IsEnabled = true;
                    txtValorParcela.IsEnabled = true;
                    txtValorParcela.IsReadOnly = true;
                    btParcelas.IsEnabled = true;
                    break;
                case 2:
                    txtQtdParcelas.IsEnabled = false;
                    txtValorParcela.IsEnabled = false;
                    txtValorParcela.IsReadOnly = true;
                    btParcelas.IsEnabled = false;
                    txtQtdParcelas.Clear();
                    txtValorParcela.Clear();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Divisao Valor Total e Qtd de Parcelas
        private decimal DivisaoTotalPorQtd(string total, string qtd)
        {
            Decimal valortotal = total != string.Empty ? Decimal.Parse(total) : 0;
            Int64 quantidade = qtd != string.Empty ? Int64.Parse(qtd) : 0;
            try
            {
                if (!String.IsNullOrEmpty(txtValorTotal.Text) || !String.IsNullOrEmpty(txtQtdParcelas.Text))
                {
                    var resultado = valortotal / quantidade;
                    return resultado;
                }
                else
                {
                    MessageBox.Show("Verifique se os campos foram preeenchidos corretamente!");
                    return 0;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Verifique se os campos foram preeenchidos corretamente!");
                return 0;
            }

        }
        #endregion

        #region ValidaCampos
        public bool ValidaCampos()
        {
            if (String.IsNullOrEmpty(txtNome.Text) || String.IsNullOrEmpty(txtEmissao.Text) || String.IsNullOrEmpty(txtVencimento.Text))
            {
                MessageBox.Show(" Os campos  Nome, Data emissão e Data Vencimento são Obrigatorios, por favor verifique!");
                return false;
            }
            return true;
                
        }
        #endregion

        #region Gerar Parcelas
        private void GerarParcelas()
        {
            if (!String.IsNullOrEmpty(txtQtdParcelas.Text) || !txtQtdParcelas.Text.Equals(0))
            {
                PreencheDataGrid();
                for (int i = 0; i < Int64.Parse(txtQtdParcelas.Text); i++)
                {
                    contaPagamento.Add(new ContaPagamento()
                    {
                        SituacaoParcelas = SituacaoConta.Pendente,
                        Conta = conta
                        
                    });

                }
            }
        }
        #endregion

        #region PreencheDataGrid
        private ObservableCollection<ContaPagamento> contaPagamento;
        public void PreencheDataGrid()
        {
            contaPagamento = new ObservableCollection<ContaPagamento>();
            DataGridContaPagamento.ItemsSource = contaPagamento;
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
                cmbReferenciaPessoa.SelectedIndex = -1;
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
                    cmbReferenciaPessoa.SelectedIndex = -1;
                    VerificaTipoPeriodo();
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
            if (ValidaCampos())
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

        private void cmbTipoPeriodo_LostFocus(object sender, RoutedEventArgs e)
        {
            VerificaTipoPeriodo();
        }

        private void txtQtdParcelas_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtQtdParcelas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtValorTotal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = Regex.IsMatch(e.Text, @"[0-9]*[\,]\d{2}");
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
            //e.Handled = Regex.IsMatch(e.Text, @"^-?[0-9][0-9,\.]+$");

        }

        private void txtValorParcela_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtValorParcela_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtValorTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void btParcelas_Click(object sender, RoutedEventArgs e)
        {
            txtValorParcela.Text = DivisaoTotalPorQtd(txtValorTotal.Text, txtQtdParcelas.Text).ToString("N2");
           
        }

        private void txtVencimento_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtVencimento.SelectedDate < txtEmissao.SelectedDate)
                MessageBox.Show(" data de vencimento nao pode ser menor que data de emissão!");
        }
    }
}
