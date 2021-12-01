using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
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

namespace CFP.App.Formularios.Financeiros.Consultas
{
    /// <summary>
    /// Interação lógica para ucAlertaContas.xam
    /// </summary>
    public partial class ucAlertaContas : UserControl
    {
        ISession Session;
        Caixa caixa;
        private ObservableCollection<AlertaContas> alertas;

        #region Verificando se caixa esta aberto
        private bool VerificaCaixa()
        {
            caixa = new RepositorioCaixa(Session).ObterPorParametros(x => x.Situacao == SituacaoCaixa.Aberto && x.UsuarioAbertura == MainWindow.UsuarioLogado).FirstOrDefault();
            if (caixa == null || caixa.Situacao == SituacaoCaixa.Fechado)
            {
                MessageBoxResult avisoCaixa = MessageBox.Show("Caixa esta fechado! Deseja abrir?", "Pergunta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (avisoCaixa == MessageBoxResult.Yes)
                {
                    caixa = new Caixa();
                    MessageBoxResult colocarValor = MessageBox.Show("Deseja digitar um valor inicial?", "Pergunta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (colocarValor == MessageBoxResult.Yes)
                    {
                        ConfirmaValorInicialCaixa janela = new ConfirmaValorInicialCaixa();
                        bool? res = janela.ShowDialog();
                        if ((bool)res)
                            caixa.ValorInicial = Decimal.Parse(janela.valorDigitado);
                        else
                            caixa.ValorInicial = 0;
                    }
                    else
                        caixa.ValorInicial = 0;

                    caixa.Codigo = new RepositorioCaixa(Session).RetornaUltimoCodigo() + 1;
                    caixa.DataAbertura = DateTime.Now;
                    caixa.UsuarioAbertura = MainWindow.UsuarioLogado;
                    new RepositorioCaixa(Session).Salvar(caixa);
                    return true;
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Repositorio
        private RepositorioContaPagamento _repositorioContaPagamento;
        public RepositorioContaPagamento RepositorioContaPagamento
        {
            get
            {
                if (_repositorioContaPagamento == null)
                    _repositorioContaPagamento = new RepositorioContaPagamento(Session);

                return _repositorioContaPagamento;
            }
            set { _repositorioContaPagamento = value; }
        }
        #endregion

        #region Salvar Fluxo
        private void SalvarFluxo(ContaPagamento dado)
        {
            if (dado.SituacaoParcelas == SituacaoParcela.Pago)
            {
                FluxoCaixa fluxoCaixa = new FluxoCaixa();
                if (dado.Conta.TipoConta == TipoConta.Pagar)
                {
                    fluxoCaixa.TipoFluxo = EntradaSaida.Saída;
                    fluxoCaixa.Nome = String.Format("Pagamento parcela número {0} - Conta: {1}", dado.Numero, dado.Conta.Codigo);
                    fluxoCaixa.Valor = dado.ValorPago * -1;
                }
                else
                {
                    fluxoCaixa.TipoFluxo = EntradaSaida.Entrada;
                    fluxoCaixa.Nome = String.Format("Recebimento parcela número {0} - Conta: {1}", dado.Numero, dado.Conta.Codigo);
                    fluxoCaixa.Valor = dado.ValorPago;
                }
                fluxoCaixa.DataGeracao = DateTime.Now;
                fluxoCaixa.Conta = dado.Conta;
                fluxoCaixa.UsuarioCriacao = MainWindow.UsuarioLogado;
                fluxoCaixa.Caixa = caixa;
                fluxoCaixa.FormaPagamento = dado.FormaPagamento;
                new RepositorioFluxoCaixa(Session).Salvar(fluxoCaixa);
            }
        }
        #endregion

        public ucAlertaContas(ObservableCollection<AlertaContas> _alertas, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            alertas = _alertas;
            dgAlertaContas.ItemsSource = alertas.OrderBy(x => x.ContaPagamento.DataVencimento).ToList();
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Grid).Children.Remove(this);
        }

        private void dgAlertaContas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VerificaCaixa())
            {
                List<ContaPagamento> selecao = new List<ContaPagamento>();
                foreach (AlertaContas item in dgAlertaContas.SelectedItems)
                {
                    selecao.Add(item.ContaPagamento);
                }
                ConfirmacaoPagamentoParcela janela = new ConfirmacaoPagamentoParcela(selecao, Session);
                bool? res = janela.ShowDialog();
                if ((bool)res)
                {
                    foreach (ContaPagamento parcelaAtualizada in selecao)
                    {
                        if (parcelaAtualizada.ID != 0)
                        {
                            RepositorioContaPagamento.Alterar(parcelaAtualizada);
                            SalvarFluxo(parcelaAtualizada);
                        }
                        else
                        {
                            parcelaAtualizada.Numero++;
                            parcelaAtualizada.Conta = selecao.First().Conta;
                            RepositorioContaPagamento.Salvar(parcelaAtualizada);
                        }
                    }
                    MainWindow p = new MainWindow();
                    p.ResumoTela();
                    dgAlertaContas.ItemsSource = p.alertas;
                }
            }
            else
                MessageBox.Show("Selecione uma ou mais Parcelas!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

