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
    /// Lógica interna para TransferenciaCofres.xaml
    /// </summary>
    public partial class TransferenciaCofres : Window
    {
        ISession Session;
        Configuracao config;

        #region Carrega combo
        private void CarregaCombo()
        {
            cmbBancoEntrada.ItemsSource = new RepositorioBanco(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
            cmbBancoEntrada.SelectedIndex = 0;

            cmbBancoSaida.ItemsSource = new RepositorioBanco(Session)
              .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
              .OrderBy(x => x.Nome)
              .ToList();
            cmbBancoEntrada.SelectedIndex = 0;
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
            var valorTotalBancoSaida = Repositorio.ObterPorParametros(x => x.Banco == cmbBancoSaida.SelectedItem && x.DataGeracao <= dataFinal).Select(x => x.Valor).ToList();
            var res = valorTotalBancoSaida.Count > 0 ? valorTotalBancoSaida.Sum() : 0;
            if (res < decimal.Parse(txtValorSaida.Text))
            {
                MessageBox.Show(String.Format("Saldo do banco {0} indisponível!", cmbBancoSaida.SelectedItem), "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
       
        #endregion

        public TransferenciaCofres(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfiguracoesSistema();
            CarregaCombo();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBancoEntrada.SelectedIndex == -1 || cmbBancoSaida.SelectedIndex == -1  || String.IsNullOrEmpty(txtValorSaida.Text))
            {
                MessageBox.Show("Todos os campos são obrigatórios", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SaldoBanco())
            {
                #region Retirada do cofre
                Cofre cofreRetirada = new Cofre
                {
                    Codigo = Repositorio.RetornaUltimoCodigo() + 1,
                    Caixa = null,
                    Banco = (Banco)cmbBancoSaida.SelectedItem,
                    Valor = Decimal.Parse(txtValorSaida.Text) * -1,
                    TransacoesBancarias = config.TransacaoBancariaPadrao,
                    Situacao = EntradaSaida.Saída,
                    Nome = String.Format("Transferência saída para o banco {0}", cmbBancoEntrada.SelectedItem),
                    DataGeracao = DateTime.Now,
                    UsuarioCriacao = MainWindow.UsuarioLogado
                };
                Repositorio.Salvar(cofreRetirada);
                #endregion

                #region Entrada no cofre
                Cofre cofreEntrada = new Cofre
                {
                    Codigo = Repositorio.RetornaUltimoCodigo() + 1,
                    Caixa = null,
                    Banco = (Banco)cmbBancoEntrada.SelectedItem,
                    Valor = Decimal.Parse(txtValorSaida.Text),
                    TransacoesBancarias = config.TransacaoBancariaPadrao,
                    Situacao = EntradaSaida.Entrada,
                    Nome = String.Format("Transferência entrada do banco {0}", cmbBancoSaida.SelectedItem),
                    DataGeracao = DateTime.Now,
                    UsuarioCriacao = MainWindow.UsuarioLogado
                };
                Repositorio.Salvar(cofreEntrada);
                #endregion

                DialogResult = true;
            }
            else
            {
                txtValorSaida.Focus();
                txtValorSaida.SelectAll();
            }
        }

        private void txtValorSaida_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtValorSaida_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,]+");
        }
    }
}
