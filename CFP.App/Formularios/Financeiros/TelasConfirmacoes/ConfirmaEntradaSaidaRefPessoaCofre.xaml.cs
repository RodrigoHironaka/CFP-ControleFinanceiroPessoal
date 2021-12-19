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
    /// Lógica interna para ConfirmaEntradaSaidaRefPessoaCofre.xaml
    /// </summary>
    public partial class ConfirmaEntradaSaidaRefPessoaCofre : Window
    {

        ISession Session;
        List<ContaPagamento> contasPagamento = new List<ContaPagamento>();
        Cofre cofre = new Cofre();
        Caixa caixa;
        Configuracao config = new Configuracao();
        Decimal totalcontasPagamentoselecionadas = 0;
        Decimal totalRefPessoaContaPagar = 0;
        Decimal totalRefPessoaContaReceber = 0;

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

        #region Carrega combo
        private void CarregaCombo()
        {
            cmbBanco.ItemsSource = new RepositorioBanco(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();

            cmbFormaPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                cofre.DataGeracao = (DateTime)txtData.SelectedDate;
                cofre.Nome = txtNome.Text;
                cofre.Banco = (Banco)cmbBanco.SelectedItem;
                cofre.TransacoesBancarias = (FormaPagamento)cmbFormaPagamento.SelectedItem;
                cofre.Valor = Decimal.Parse(txtValor.Text);
                cofre.Situacao = EntradaSaida.Entrada;

                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

        #region Preeenche Campos
        private void PreencheCampos()
        {
            txtValor.Text = contasPagamento.Sum(x => x.ValorParcela).ToString("n2");
            totalcontasPagamentoselecionadas = contasPagamento.Select(x => x.ValorParcela).Sum();
            totalRefPessoaContaPagar = contasPagamento.Where(x => x.Conta.TipoConta == TipoConta.Pagar).Select(x => x.ValorParcela).Sum();
            totalRefPessoaContaReceber = contasPagamento.Where(x => x.Conta.TipoConta == TipoConta.Receber).Select(x => x.ValorParcela).Sum();
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

        public ConfirmaEntradaSaidaRefPessoaCofre(ISession _session)
        {
            InitializeComponent();
            Session = _session;
            cmbBanco.SelectedIndex = 0;
            cmbFormaPagamento.SelectedIndex = 0;
        }

        public ConfirmaEntradaSaidaRefPessoaCofre(List<ContaPagamento> _contasPagamento, Caixa _caixa, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            contasPagamento = _contasPagamento;
            caixa = _caixa;
            PreencheCampos();
        }

        //public ConfirmaEntradaSaidaRefPessoaCofre(Caixa _caixa, ISession _session)
        //{
        //    InitializeComponent();
        //    Session = _session;
        //    caixa = _caixa;
        //}

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtValor.Text) || string.IsNullOrEmpty(txtNome.Text) || cmbBanco.SelectedItem == null || cmbFormaPagamento.SelectedItem == null || string.IsNullOrEmpty(txtData.Text))
            {
                MessageBox.Show("Todos os campos são Obrigatórios. Por favor verifique!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PreencheObjeto())
            {
                try
                {
                    if (cofre.Id == 0)
                    {
                        cofre.Codigo = Repositorio.RetornaUltimoCodigo() + 1;
                        cofre.UsuarioCriacao = MainWindow.UsuarioLogado;
                        Repositorio.Salvar(cofre);
                    }
                    if (chkEnviarCaixa.IsChecked == true)
                    {
                        #region Retirada do cofre definido pelo usuario
                        foreach (var item in contasPagamento)
                        {
                            Cofre cofreRetirada = new Cofre
                            {
                                Codigo = Repositorio.RetornaUltimoCodigo() + 1,
                                Caixa = caixa,
                                Banco = (Banco)cmbBanco.SelectedItem,
                                Valor = item.ValorParcela * -1,
                                TransacoesBancarias = config.TransacaoBancariaPadrao,
                                Situacao = EntradaSaida.Saída,
                                Nome = String.Format("Transferência para caixa Ref a {0}, Conta: {1}", item.Conta.Pessoa, item.Conta.Codigo),
                                DataGeracao = (DateTime)txtData.SelectedDate,
                                UsuarioCriacao = MainWindow.UsuarioLogado
                            };
                            Repositorio.Salvar(cofreRetirada);
                        }
                        #endregion

                        #region Entrada no caixa com base na saída do cofre

                       if(totalRefPessoaContaPagar > 0)
                        {
                            FluxoCaixa fluxoCaixa = new FluxoCaixa
                            {
                                TipoFluxo = EntradaSaida.Entrada,
                                Nome = "Transferência do cofre através do referenciamento de pessoas (Entrada).",
                                Valor = totalRefPessoaContaPagar,
                                DataGeracao = (DateTime)txtData.SelectedDate,
                                Conta = null,
                                UsuarioCriacao = MainWindow.UsuarioLogado,
                                Caixa = caixa,
                                FormaPagamento = config.TransacaoBancariaPadrao
                            };
                            RepositorioFluxoCaixa.Salvar(fluxoCaixa);
                        }

                        if (totalRefPessoaContaReceber > 0)
                        {
                            FluxoCaixa fluxoCaixa = new FluxoCaixa
                            {
                                TipoFluxo = EntradaSaida.Saída,
                                Nome = "Transferência do cofre através do referenciamento de pessoas (Saída).",
                                Valor = totalRefPessoaContaReceber * -1,
                                DataGeracao = (DateTime)txtData.SelectedDate,
                                Conta = null,
                                UsuarioCriacao = MainWindow.UsuarioLogado,
                                Caixa = caixa,
                                FormaPagamento = config.TransacaoBancariaPadrao
                            };
                            RepositorioFluxoCaixa.Salvar(fluxoCaixa);
                        }

                        #endregion

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            Close();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfiguracoesSistema();
            CarregaCombo();
            txtData.SelectedDate = DateTime.Now;
        }

        private void txtValor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9]+");
        }

        private void txtValor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtValor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtValor.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtValor.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValor.Clear();
                    txtValor.Focus();
                }
            }
        }
    }
}
