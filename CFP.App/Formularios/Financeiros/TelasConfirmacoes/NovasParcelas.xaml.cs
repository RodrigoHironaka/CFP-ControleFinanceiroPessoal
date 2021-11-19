using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
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
using System.Windows.Shapes;

namespace CFP.App.Formularios.Financeiros.TelasConfirmacoes
{
    /// <summary>
    /// Lógica interna para NovasParcelas.xaml
    /// </summary>
    public partial class NovasParcelas : Window
    {

        ISession Session;
        public ObservableCollection<ContaPagamento> contaPagamento = new ObservableCollection<ContaPagamento>();
        public Conta conta;

        public NovasParcelas(Conta _conta, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            conta = _conta;
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtValorTotal.Text) || String.IsNullOrEmpty(txtQtd.Text) || dtpVencimento.SelectedDate == null)
            {
                txtAlerta.Text = "Todos os campos são obrigatórios";
                return;
            }
            else
            {
                txtAlerta.Text = string.Empty;
                try
                {
                    Decimal valorTotal = Decimal.Parse(txtValorTotal.Text);
                    Int32 qtdParcelas = Int32.Parse(txtQtd.Text);
                    DateTime dataPrimeiroVencimento = (DateTime)dtpVencimento.SelectedDate;

                    if (!valorTotal.Equals(0) && !qtdParcelas.Equals(0))
                    {
                        var numero = conta.QtdParcelas != null ? conta.QtdParcelas : 0;
                        Decimal valorParcela = Math.Round(valorTotal / qtdParcelas, 2);
                        Decimal valorDiferenca = Math.Round(valorTotal - valorParcela * qtdParcelas, 2);
                        for (int i = 0; i < qtdParcelas; i++)
                        {
                            numero++;
                            contaPagamento.Add(new ContaPagamento()
                            {
                                
                                SituacaoParcelas = SituacaoParcela.Pendente,
                                Conta = conta,
                                Numero = (int)numero,
                                ValorParcela = !(i + 1 == qtdParcelas) ? valorParcela : valorParcela + valorDiferenca,
                                DataVencimento = dataPrimeiroVencimento.AddMonths(i)
                            }) ;

                            #region Outra Opcao 
                            //var contaPagamentoNovo = new ContaPagamento();
                            //contaPagamentoNovo.SituacaoParcelas = SituacaoParcela.Pendente;
                            //contaPagamentoNovo.Conta = conta;
                            //contaPagamentoNovo.Numero = new RepositorioContaPagamento(Session).ObterPorParametros(x => x.Conta.Id == conta.Id).Select(x => x.Numero).Count() + 1;
                            //contaPagamentoNovo.ValorParcela = Decimal.Parse(!(i + 1 == qtdParcelas) ? valorParcela.ToString() : (valorParcela + valorDiferenca).ToString());
                            //contaPagamentoNovo.DataVencimento = dataPrimeiroVencimento.AddMonths(i);
                            //Repositorio.Salvar(contaPagamentoNovo);
                            #endregion
                        }
                        conta.ValorTotal = conta.ValorTotal != null ? conta.ValorTotal + valorTotal : valorTotal;
                        conta.QtdParcelas = conta.QtdParcelas != null ? conta.QtdParcelas + qtdParcelas : qtdParcelas;
                        DialogResult = true;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is DivideByZeroException)
                    {
                        txtAlerta.Text = ex.Message.ToString();
                        return;
                    }
                    else
                        throw new Exception(ex.Message.ToString());
                }
            }
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtpVencimento.SelectedDate = DateTime.Now;
        }

        private void txtValorTotal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtQtd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtValorTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtQtd_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
