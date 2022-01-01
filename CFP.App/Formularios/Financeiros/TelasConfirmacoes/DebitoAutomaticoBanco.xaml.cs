using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObjetoValor;
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
using System.Windows.Shapes;

namespace CFP.App.Formularios.Financeiros.TelasConfirmacoes
{
    /// <summary>
    /// Lógica interna para DebitoAutomaticoBanco.xaml
    /// </summary>
    public partial class DebitoAutomaticoBanco : Window
    {
        ISession Session;
        Configuracao config;
        Caixa caixa;
        List<ContaPagamento> contaPagamentos;

        #region Carrega combo
        private void CarregaCombo()
        {
            cmbBanco.ItemsSource = new RepositorioBanco(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
        }
        #endregion

        #region Repositorio
        private RepositorioCofre _repositorio;
        public RepositorioCofre Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioCofre(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
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

        private RepositorioFluxoCaixa _repositorioFluxoCaixa;
        public RepositorioFluxoCaixa RepositorioFluxoCaixa
        {
            get
            {
                if (_repositorioFluxoCaixa == null)
                    _repositorioFluxoCaixa = new RepositorioFluxoCaixa(Session);

                return _repositorioFluxoCaixa;
            }
            set { _repositorioFluxoCaixa = value; }
        }
        #endregion

        #region Pegando as Configuracoes
        private void ConfiguracoesSistema()
        {
            Session.Clear();
            config = RepositorioConfiguracao.ObterTodos().Where(x => x.UsuarioCriacao.Id == MainWindow.UsuarioLogado.Id).FirstOrDefault();
            if (config == null || config.TransacaoBancariaPadrao == null)
            {
                MessageBox.Show("Por favor verifique suas configurações!\r\nPode ser que ela não esteja criada ou sua transação bancária padrão não esteja definida!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }
        #endregion

        #region verificando saldo no banco
        private bool SaldoBanco()
        {
            DateTime data = DateTime.Today;
            //var dataInicio = new DateTime(data.Year, data.Month, 1);
            var dataFinal = new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));
            var valorTotalBancoSaida = Repositorio.ObterPorParametros(x => x.Banco == cmbBanco.SelectedItem && x.DataGeracao <= dataFinal).Select(x => x.Valor).ToList();
            var res = valorTotalBancoSaida.Count > 0 ? valorTotalBancoSaida.Sum() : 0;
            if (res < decimal.Parse(txtValor.Text))
            {
                MessageBox.Show(String.Format("Saldo do banco {0} indisponível!", cmbBanco.SelectedItem), "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        #endregion

        public DebitoAutomaticoBanco(List<ContaPagamento> _contaPagamentos, ISession _session, Caixa _caixa)
        {
            InitializeComponent();
            Session = _session;
            caixa = _caixa;
            contaPagamentos = _contaPagamentos;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfiguracoesSistema();
            CarregaCombo();
            txtValor.Text = contaPagamentos.Where(x => x.Conta.TipoConta == TipoConta.Pagar).Select(x => x.ValorPago).Sum().ToString("n2");
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBanco.SelectedIndex == -1 || String.IsNullOrEmpty(txtValor.Text))
            {
                MessageBox.Show("Todos os campos são obrigatórios", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SaldoBanco())
            {
                foreach (var item in contaPagamentos)
                {
                    #region Retirada do cofre
                    Cofre cofreRetirada = new Cofre
                    {
                        Codigo = Repositorio.RetornaUltimoCodigo() + 1,
                        Caixa = caixa,
                        Banco = (Banco)cmbBanco.SelectedItem,
                        Valor = item.ValorPago * -1,
                        TransacoesBancarias = config.TransacaoBancariaPadrao,
                        Situacao = EntradaSaida.Saída,
                        Nome = String.Format("Débito Automático do {0} para o caixa! Conta {1} Parcela - {2}", cmbBanco.SelectedItem, item.Conta.Nome, item.Numero),
                        DataGeracao = DateTime.Now,
                        UsuarioCriacao = MainWindow.UsuarioLogado
                    };
                    Repositorio.Salvar(cofreRetirada);
                    #endregion

                    #region Entrada no Caixa
                    FluxoCaixa fluxoCaixa = new FluxoCaixa
                    {
                        TipoFluxo = EntradaSaida.Entrada,
                        Nome = String.Format("Transferência do cofre através do pagamento pelo Débito Automático. Conta {0} Parcela - {1}", item.Conta.Nome, item.Numero),
                        Valor = item.ValorPago,
                        DataGeracao = DateTime.Now,
                        Conta = item.Conta,
                        UsuarioCriacao = MainWindow.UsuarioLogado,
                        Caixa = caixa,
                        FormaPagamento = config.TransacaoBancariaPadrao
                    };
                    RepositorioFluxoCaixa.Salvar(fluxoCaixa);
                    #endregion
                }

                DialogResult = true;
            }
            else
            {
                txtValor.Focus();
                txtValor.SelectAll();
            }
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
