using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using LinqKit;
using NHibernate;
using Repositorio.Repositorios;
using System;
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

namespace CFP.App.Formularios.Financeiros.Consultas
{
    /// <summary>
    /// Interação lógica para ucConsultaContas.xam
    /// </summary>
    public partial class ucConsultaContas : UserControl
    {
        ISession Session;
        Caixa caixa;
        Configuracao config;

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

        private RepositorioConfiguracao _repositorioConfiguracao;
        public RepositorioConfiguracao RepositorioConfiguracao
        {
            get
            {
                if (_repositorioConfiguracao == null)
                    _repositorioConfiguracao = new RepositorioConfiguracao(Session);

                return _repositorioConfiguracao;
            }
            set { _repositorioConfiguracao = value; }
        }
        #endregion

        #region Pegando as Configuracoes
        private void ConfiguracoesSistema()
        {
            Session.Clear();
            config = RepositorioConfiguracao.ObterTodos().Where(x => x.UsuarioCriacao == MainWindow.UsuarioLogado).FirstOrDefault();
        }
        #endregion

        #region Preenche DataGrid
        public void PreencheDataGrid()
        {
            var predicado = RepositorioContaPagamento.CriarPredicado();
            predicado = predicado.And(x => x.Conta.UsuarioCriacao == MainWindow.UsuarioLogado);

            if(cmbSituacaoConta.SelectedIndex != -1)
                predicado = predicado.And(x => x.Conta.Situacao == (SituacaoConta)cmbSituacaoConta.SelectedIndex);
            else
                predicado = predicado.And(x => x.Conta.Situacao == SituacaoConta.Aberto);

            if (!String.IsNullOrEmpty(txtPesquisa.Text))
                predicado = predicado.And(x => x.Conta.Nome.Contains(txtPesquisa.Text) || x.ValorParcela.ToString().Contains(txtPesquisa.Text) || x.Conta.Codigo.ToString().Contains(txtPesquisa.Text) || x.Conta.NumeroDocumento.ToString().Contains(txtPesquisa.Text));

            if (cmbSituacaoParcelas.SelectedIndex != -1)
                predicado = predicado.And(x => x.SituacaoParcelas == (SituacaoParcela)cmbSituacaoParcelas.SelectedIndex);
            else
                predicado = predicado.And(x => x.SituacaoParcelas == SituacaoParcela.Pendente || x.SituacaoParcelas == SituacaoParcela.Parcial);

            if (cmbTipoConta.SelectedIndex != -1)
                predicado = predicado.And(x => x.Conta.TipoConta == (TipoConta)cmbTipoConta.SelectedIndex);

            if (cmbPeriodo.SelectedIndex != -1)
                predicado = predicado.And(x => x.Conta.TipoPeriodo == (TipoPeriodo)cmbPeriodo.SelectedIndex);

            if (cmbFormaCompra.SelectedItem != null)
                predicado = predicado.And(x => x.Conta.FormaCompra == cmbFormaCompra.SelectedItem);

            if (cmbPessoaReferenciada.SelectedItem != null)
                predicado = predicado.And(x => x.Conta.Pessoa == cmbPessoaReferenciada.SelectedItem);

            if (dtpInicio.SelectedDate != null)
                predicado = predicado.And(x => x.DataVencimento >= dtpInicio.SelectedDate);

            if (dtpFinal.SelectedDate != null)
            {
                if (dtpFinal.SelectedDate < dtpInicio.SelectedDate)
                    MessageBox.Show("Data Final é menor que a data Inicial. Por favor Verifique!", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    predicado = predicado.And(x => x.DataVencimento <= dtpFinal.SelectedDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59));
            }

            var filtro = RepositorioContaPagamento.ObterPorParametros(predicado).ToList();
            dgContasFiltradas.ItemsSource = filtro;
            if (filtro.Count > 0)
                txtTotalFiltro.Text = String.Format("Total: {0}", filtro.Sum(x => x.ValorParcela).ToString("N2"));
        }
        #endregion

        #region Carrega Combo
        private void CarregaCombo()
        {
            cmbTipoConta.ItemsSource = Enum.GetValues(typeof(TipoConta));
            cmbPeriodo.ItemsSource = Enum.GetValues(typeof(TipoPeriodo));
            cmbSituacaoParcelas.ItemsSource = Enum.GetValues(typeof(SituacaoParcela));
            cmbSituacaoConta.ItemsSource = Enum.GetValues(typeof(SituacaoConta));

            cmbFormaCompra.ItemsSource = new RepositorioFormaPagamento(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.TransacoesBancarias == SimNao.Não)
                .OrderBy(x => x.Nome)
                .ToList();

            cmbPessoaReferenciada.ItemsSource = new RepositorioPessoa(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
        }
        #endregion

        #region Verificando primeiro e ultimo dia do Mes
        private void PrimeiroUltimoDiaMes()
        {
            DateTime data = DateTime.Today;
            dtpInicio.SelectedDate = new DateTime(data.Year, data.Month, 1);
            dtpFinal.SelectedDate = new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));
        }
        #endregion

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

        public ucConsultaContas(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombo();
            PrimeiroUltimoDiaMes();
            ConfiguracoesSistema();
        }

        private void btFiltro_Click(object sender, RoutedEventArgs e)
        {
            PreencheDataGrid();
        }

        private void dtpInicio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                dtpInicio.Text = string.Empty;
        }

        private void dtpFinal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                dtpFinal.Text = string.Empty;
        }

        private void cmbTipoConta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbTipoConta.SelectedIndex = -1;
        }

        private void cmbPeriodo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbPeriodo.SelectedIndex = -1;
        }

        private void cmbFormaCompra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbFormaCompra.SelectedItem = null;
        }

        private void cmbPessoaReferenciada_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbPessoaReferenciada.SelectedItem = null;
        }

        private void txtPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                txtPesquisa.Text = string.Empty;
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Grid).Children.Remove(this);
        }

        private void cmbSituacaoParcelas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbSituacaoParcelas.SelectedIndex = -1;
        }

        private void btPagar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificaCaixa())
            {
                if (config != null && config.FormaPagamentoPadraoConta != null )
                {
                    var selecoes = dgContasFiltradas.SelectedItems;
                    foreach (ContaPagamento parcela in selecoes)
                    {
                        parcela.SituacaoParcelas = SituacaoParcela.Pago;
                        parcela.ValorPago = parcela.ValorParcela;
                        parcela.DataPagamento = DateTime.Now;
                        parcela.FormaPagamento = config.FormaPagamentoPadraoConta;
                        RepositorioContaPagamento.Alterar(parcela);
                        SalvarFluxo(parcela);
                    }
                    btFiltro_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Por favor defina a forma de pagamento padrão em Configurações para continuar!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
        }

        private void dgContasFiltradas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VerificaCaixa())
            {
                IList<ContaPagamento> linhasContaPagamento = new List<ContaPagamento>();
                linhasContaPagamento.Add((ContaPagamento)dgContasFiltradas.SelectedItem);
                ConfirmacaoPagamentoParcela janela = new ConfirmacaoPagamentoParcela(linhasContaPagamento, Session);
                bool? res = janela.ShowDialog();
                if ((bool)res)
                {
                    foreach (ContaPagamento parcelaAtualizada in linhasContaPagamento)
                    {
                        if (parcelaAtualizada.ID != 0)
                        {
                            RepositorioContaPagamento.Alterar(parcelaAtualizada);
                            SalvarFluxo(parcelaAtualizada);
                        }
                        else
                        {
                            parcelaAtualizada.Numero++;
                            parcelaAtualizada.Conta = linhasContaPagamento.First().Conta;
                            RepositorioContaPagamento.Salvar(parcelaAtualizada);
                        }
                    }
                    btFiltro_Click(sender, e);
                }
            }
        }

        private void cmbSituacaoConta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbSituacaoConta.SelectedIndex = -1;
        }

        private void menuItemExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            Ferramentas.Exportar.ExportarExcel.ExpExcel(dgContasFiltradas);
        }
    }
}
