using CFP.Dominio.ObjetoValor;
using Dominio.Dominio;
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
using System.Windows.Shapes;

namespace CFP.App.Formularios.Financeiros.TelasConfirmacoes
{
    /// <summary>
    /// Lógica interna para ConfirmacaoPagamentoParcela.xaml
    /// </summary>
    public partial class ConfirmacaoPagamentoParcela : Window
    {
        ISession Session;
        IList<ContaPagamento> linhaContaPagemento;
        public ObservableCollection<ContaPagamento> contaPagamentoAtualizado = new ObservableCollection<ContaPagamento>();

        #region Carrega Combos
        private void CarregaCombos()
        {
            cmbFormaPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<FormaPagamento>();
            cmbFormaPagamento.SelectedItem = 0;
        }
        #endregion

        #region PreencheCampos
        private void PreencheCampos()
        {
            string numParcela = string.Empty;
            decimal somaValorParcela = 0;
            foreach (var linha in linhaContaPagemento)
            {
                numParcela += linha.Numero.ToString() + " ";
                somaValorParcela = somaValorParcela + linha.ValorParcela;

                txtCodigoConta.Text = linha.Conta.Id.ToString();
                txtNumeroParcela.Text = numParcela;
                txtValorParcela.Text = somaValorParcela.ToString("N2");
                txtDataPagamento.SelectedDate = linha.DataPagamento != null ? linha.DataPagamento : DateTime.Now;
                txtJurosPorcentagem.Text = linha.JurosPorcentual != 0 ? linha.JurosPorcentual.ToString("N3") : string.Empty;
                txtJurosValor.Text = linha.JurosValor != 0 ? linha.JurosValor.ToString("N2") : string.Empty;
                txtDescontoPorcentagem.Text = linha.DescontoPorcentual != 0 ? linha.DescontoPorcentual.ToString("N3") : string.Empty;
                txtDescontoValor.Text = linha.DescontoValor != 0 ? linha.DescontoValor.ToString("N2") : string.Empty;
                txtValorReajustado.Text = linha.ValorReajustado != 0 ? linha.ValorReajustado.ToString("N2") : txtValorParcela.Text;
                txtValorPago.Text = linha.ValorPago != 0 ? linha.ValorPago.ToString("N2") : string.Empty;
                txtValorRestante.Text = linha.ValorRestante != 0 ? linha.ValorRestante.ToString("N2") : string.Empty;
                cmbFormaPagamento.SelectedIndex = 0;
            }

        }
        #endregion

        #region Calculo Juros e Desconto (% e $)
        private decimal CalculaValorPorcentagem(decimal valor, decimal porcentagem)
        {
            decimal resultado = (valor * (porcentagem / 100));
            return resultado;
        }

        //private decimal ResultadoComPorcentagem(decimal valor, decimal porcentagem)
        //{
        //    decimal resultado = (valor * (porcentagem / 100)) + valor;
        //    return resultado;
        //}

        private decimal CalculaPorcentagemValor(decimal valor, decimal valor2)
        {
            decimal porcentagem = (valor2 * 100) / valor;
            return porcentagem;
        }

        //private decimal ResultadoComPorcentagemSubtracao(decimal valor, decimal porcentagem)
        //{
        //    decimal resultado = valor - (valor * (porcentagem / 100));
        //    return resultado;
        //}

        private void CalculaValorReajustado()
        {
            decimal vparcela = txtValorParcela.Text != string.Empty ? Decimal.Parse(txtValorParcela.Text) : 0;
            decimal vjuros = txtJurosValor.Text != string.Empty ? Decimal.Parse(txtJurosValor.Text) : 0;
            decimal vdesconto = txtDescontoValor.Text != string.Empty ? Decimal.Parse(txtDescontoValor.Text) : 0;
            txtValorReajustado.Text = (vparcela + vjuros - vdesconto).ToString("N2");
        }
        #endregion

        #region Definindo Cor Padrão do botão Pesquisar #FF1F3D68 
        public void CorPadrãoLabel()
        {
            var converter = new System.Windows.Media.BrushConverter();
            var HexaToBrush = (Brush)converter.ConvertFromString("#FF1F3D68");
            txtValorPago.Foreground = HexaToBrush;
        }
        #endregion

        #region Verificando se possui valor Restante
        private void CalculaValorRestante(decimal valorpago, decimal valorreajustado)
        {
            if (valorpago == valorreajustado)
            {
                CorPadrãoLabel();
                txtValorRestante.Clear();
            }
            else if (valorpago < valorreajustado)
            {
                CorPadrãoLabel();
                txtValorRestante.Text = (valorreajustado - valorpago).ToString("N2");
            }
            else
            {
                MessageBox.Show("O valor pago digitado é maior que a parcela!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                txtValorPago.Foreground = Brushes.Red;
                txtValorPago.SelectAll();
                txtValorRestante.Clear();
            }

        }
        #endregion

        #region Preenche Objeto para salvar
       DateTime dataVencimento = DateTime.Now;
        private bool PreencheObjeto()
        {
            var qtdlinhaSelecionadas = linhaContaPagemento.Count();
            var valorPago = Decimal.Parse(txtValorPago.Text);
            try
            {
                if (qtdlinhaSelecionadas > 0)
                {
                    foreach (var linha in linhaContaPagemento.OrderBy(x => x.Numero))
                    {
                        linha.DataPagamento = txtDataPagamento.SelectedDate;
                        linha.FormaPagamento = (FormaPagamento)cmbFormaPagamento.SelectedItem;
                        if (qtdlinhaSelecionadas == 1)
                        {
                            linha.JurosPorcentual = txtJurosPorcentagem.Text != string.Empty ? Decimal.Parse(txtJurosPorcentagem.Text) : 0;
                            linha.JurosValor = txtJurosValor.Text != string.Empty ? Decimal.Parse(txtJurosValor.Text) : 0;
                            linha.DescontoPorcentual = txtDescontoPorcentagem.Text != string.Empty ? Decimal.Parse(txtDescontoPorcentagem.Text) : 0;
                            linha.DescontoValor = txtDescontoValor.Text != string.Empty ? Decimal.Parse(txtDescontoValor.Text) : 0;
                        }
                        else
                        {
                            linha.JurosPorcentual = txtJurosPorcentagem.Text != string.Empty ? Decimal.Parse(txtJurosPorcentagem.Text) / qtdlinhaSelecionadas : 0;
                            linha.JurosValor = txtJurosValor.Text != string.Empty ? Decimal.Parse(txtJurosValor.Text) / qtdlinhaSelecionadas : 0;
                            linha.DescontoPorcentual = txtDescontoPorcentagem.Text != string.Empty ? Decimal.Parse(txtDescontoPorcentagem.Text) / qtdlinhaSelecionadas : 0;
                            linha.DescontoValor = txtDescontoValor.Text != string.Empty ? Decimal.Parse(txtDescontoValor.Text) / qtdlinhaSelecionadas : 0;

                        }
                        var parcela = linha.ValorParcela + linha.JurosValor - linha.DescontoValor;

                        decimal res = 0;
                        if (valorPago >= parcela)
                        {
                            valorPago -= parcela;
                            linha.ValorReajustado = txtValorReajustado.Text != string.Empty ? parcela : 0;
                            linha.ValorPago = txtValorPago.Text != string.Empty ? parcela : 0;
                            linha.ValorRestante = 0;
                        }
                        else
                        {
                            res = parcela - valorPago;
                            linha.ValorReajustado = txtValorReajustado.Text != string.Empty ? parcela : 0;
                            linha.ValorPago = txtValorPago.Text != string.Empty ? valorPago : 0;
                            linha.ValorRestante = res;
                            dataVencimento = linha.DataVencimento.Value;
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                //throw new Exception(ex.ToString());
            }
        }
        #endregion

        public ConfirmacaoPagamentoParcela(IList<ContaPagamento> _contaPagamento, ISession _session)
        {
            InitializeComponent();
            linhaContaPagemento = _contaPagamento;
            Session = _session;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombos();
            PreencheCampos();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtJurosPorcentagem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtJurosPorcentagem.Text))
            {
                txtJurosValor.Text = CalculaValorPorcentagem(Decimal.Parse(txtValorParcela.Text), Decimal.Parse(txtJurosPorcentagem.Text)).ToString("N2");
                CalculaValorReajustado();
            }
            else
            {
                txtJurosValor.Clear();
                CalculaValorReajustado();
            }
        }

        private void txtJurosValor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtJurosValor.Text))
            {
                txtJurosPorcentagem.Text = CalculaPorcentagemValor(Decimal.Parse(txtValorParcela.Text), Decimal.Parse(txtJurosValor.Text)).ToString("N3");
                CalculaValorReajustado();
            }
            else
            {
                txtJurosPorcentagem.Clear();
                CalculaValorReajustado();
            }
        }

        private void txtDescontoPorcentagem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDescontoPorcentagem.Text))
            {
                txtDescontoValor.Text = CalculaValorPorcentagem(Decimal.Parse(txtValorParcela.Text), Decimal.Parse(txtDescontoPorcentagem.Text)).ToString("N2");
                CalculaValorReajustado();
            }
            else
            {
                txtDescontoValor.Clear();
                CalculaValorReajustado();
            }
        }

        private void txtDescontoValor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDescontoValor.Text))
            {
                txtDescontoPorcentagem.Text = CalculaPorcentagemValor(Decimal.Parse(txtValorParcela.Text), Decimal.Parse(txtDescontoValor.Text)).ToString("N3");
                CalculaValorReajustado();
            }
            else
            {
                txtDescontoPorcentagem.Clear();
                CalculaValorReajustado();
            }
        }

        private void txtValorPago_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (!String.IsNullOrEmpty(txtValorPago.Text))
                    CalculaValorRestante(Decimal.Parse(txtValorPago.Text), Decimal.Parse(txtValorReajustado.Text));
            }
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtValorPago.Text))
            {
                MessageBox.Show("Digite o valor Pago!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                txtValorPago.SelectAll();
                return;
            }
            if (Decimal.Parse(txtValorReajustado.Text) < decimal.Parse(txtValorPago.Text))
            {
                MessageBox.Show("O valor pago digitado é maior que a parcela!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                txtValorPago.SelectAll();
                return;
            }
            if (txtDataPagamento.SelectedDate == null)
            {
                MessageBox.Show("Digite a data do pagamento!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                txtDataPagamento.Focus();
                return;
            }
            if (cmbFormaPagamento.SelectedItem == null)
            {
                MessageBox.Show("Defina a forma de pagamento!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFormaPagamento.Focus();
                return;
            }

            if (PreencheObjeto())
            {
                var numeroUltimaParcela = 0;
                foreach (var item in linhaContaPagemento.OrderBy(x => x.Numero))
                {
                    numeroUltimaParcela = item.Conta.ContaPagamentos.Count() + 1;
                    item.SituacaoParcelas = SituacaoParcela.Pago;
                    contaPagamentoAtualizado.Add(item);
                }
                if (!String.IsNullOrEmpty(txtValorRestante.Text))
                {
                    contaPagamentoAtualizado.Add(new ContaPagamento()
                    {
                        SituacaoParcelas = SituacaoParcela.Parcial,
                        Numero = numeroUltimaParcela,
                        ValorParcela = Decimal.Parse(txtValorRestante.Text),
                        DataVencimento = dataVencimento
                    });
                }
                this.DialogResult = true;
            }
        }

        private void txtJurosPorcentagem_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtJurosValor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtDescontoPorcentagem_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtDescontoValor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtValorPago_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtJurosPorcentagem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtJurosValor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtDescontoPorcentagem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtDescontoValor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtValorPago_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
