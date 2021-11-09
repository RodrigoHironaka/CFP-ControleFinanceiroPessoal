using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObejtoValor;
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

namespace CFP.App.Formularios.Financeiros
{
    /// <summary>
    /// Interação lógica para UserControlCaixa.xam
    /// </summary>
    public partial class UserControlCaixa : UserControl
    {
        ISession Session;
        Caixa caixa;
      
        #region Verifica Situação do Caixa
        private void VerificaSituacaoCaixa()
        {
            caixa = (Caixa)Repositorio.ObterPorParametros(x => x.Situacao == SituacaoCaixa.Aberto && x.UsuarioAbertura == MainWindow.UsuarioLogado).FirstOrDefault();
            if (caixa != null)
            {
                if(caixa.Situacao == SituacaoCaixa.Aberto)
                {
                    PreencheCampos();
                    ControleAcessoCadastro();
                    TotalizadoresEntradaSaida();
                }
            }
            else
                ControleAcessoInicial();
            
        }
        #endregion

        #region Repositorio
        private RepositorioCaixa _repositorio;
        public RepositorioCaixa Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioCaixa(Session);

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

        private RepositorioConta _repositorioConta;
        public RepositorioConta RepositorioConta
        {
            get
            {
                if (_repositorioConta == null)
                    _repositorioConta = new RepositorioConta(Session);

                return _repositorioConta;
            }
            set { _repositorioConta = value; }
        }


        #endregion

        #region Controle de acessos Inicial e Cadastro
        private void ControleAcessoInicial()
        {
            //Bloqueando
            GridCampos.IsEnabled = !GridCampos.IsEnabled;
            btNovoRegistroConta.IsEnabled = !btNovoRegistroConta.IsEnabled;
            btRetirandoCofre.IsEnabled = !btRetirandoCofre.IsEnabled;
            btTransferirCofre.IsEnabled = !btTransferirCofre.IsEnabled;

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
            btNovoRegistroConta.IsEnabled = true;
            btRetirandoCofre.IsEnabled = true;
            btTransferirCofre.IsEnabled = true;

        }
        #endregion

        #region Selecão e Foco no campo Codigo 
        private void FocoNoCampoCodigo()
        {
            txtCodigo.SelectAll();
            txtCodigo.Focus();
        }
        #endregion

        #region Preenche Campos
        private void PreencheCampos()
        {
            lblSituacao.Text = caixa.Situacao.ToString();
            txtCodigo.Text = caixa.Codigo.ToString();
            txtSaldoInicial.Text = string.Format("SALDO INICIAL R$ {0:n2}", caixa.ValorInicial);
            txtDataAbertura.Visibility = Visibility.Visible;
            txtDataAbertura.Text = caixa.DataAbertura.ToString();
            txtTotalEntrada.Text = string.Format("TOTAL ENTRADA R$ {0:n2}", caixa.TotalEntrada);
            txtTotalSaida.Text = string.Format("TOTAL SAÍDA R$ {0:n2}", caixa.TotalSaida);
            if(caixa.Situacao == SituacaoCaixa.Fechado)
            {
                txtSaldoFinal.Visibility = Visibility.Visible;
                txtSaldoFinal.Text = string.Format("SALDO FINAL R$ {0:n2}", caixa.BalancoFinal);
                txtDataFechamento.Visibility = Visibility.Visible;
                txtDataFechamento.Text = caixa.DataFechamento.ToString();
            }
            

        }
        #endregion

        #region Limpa os campos do Cadastro
        public void LimpaCampos()
        {
            txtDataAbertura.Visibility = Visibility.Hidden;
            txtSaldoInicial.Text = "SALDO INICIAL: R$ 0,00";
            txtTotalEntrada.Text = "TOTAL ENTRADA: R$ 0,00";
            txtTotalSaida.Text = "TOTAL SAÍDA: R$ 0,00";
            txtSaldoFinal.Text = "SALDO FINAL: R$ 0,00";
            txtDataFechamento.Visibility = Visibility.Hidden;
            //foreach (var item in GridCampos.Children)
            //{
            //    if (item is TextBox)
            //        (item as TextBox).Text = string.Empty;
            //    if (item is ComboBox)
            //        (item as ComboBox).SelectedIndex = 0;
            //    if (item is CheckBox)
            //        (item as CheckBox).IsChecked = false;
            //    if (item is RadioButton)
            //        (item as RadioButton).IsChecked = false;
            //    if (item is TextBlock)
            //        (item as TextBlock).Text = string.Empty;
            //}
        }
        #endregion

        #region Totalizadores
        private decimal totalSaida = 0;
        private decimal totalEntrada = 0;
        private decimal saldoFinal = 0;
        private decimal? aReceberPessoa = 0;
        private void TotalizadoresEntradaSaida()
        {
            PreencheDataGrid();
            totalSaida = RepositorioFluxoCaixa.ObterTodos().Where(x => x.Caixa.Id == caixa.Id && x.TipoFluxo == EntradaSaida.Saída).Sum(x => x.Valor);
            totalEntrada = RepositorioFluxoCaixa.ObterTodos().Where(x => x.Caixa.Id == caixa.Id && x.TipoFluxo == EntradaSaida.Entrada).Sum(x => x.Valor);
            saldoFinal = caixa.ValorInicial + totalEntrada - totalSaida;
            //aReceberPessoa = RepositorioConta.ObterTodos().Where(x => x.Pessoa != null).Sum(x => x.ValorTotal);
            txtTotalSaida.Text = String.Format("TOTAL SAÍDA: R${0:n2}", totalSaida);
            txtTotalEntrada.Text = String.Format("TOTAL ENTRADA: R${0:n2}", totalEntrada);
            txtSaldoFinal.Text = String.Format("SALDO FINAL : R${0:n2}", saldoFinal);
            txtTotalAReceber.Text = String.Format("TOTAL: R${0:n2}", aReceberPessoa);
        }
        #endregion

        #region PreencheDataGrid
        public void PreencheDataGrid()
        {
            DataGridFluxoCaixa.ItemsSource = RepositorioFluxoCaixa.ObterPorParametros(x => x.Caixa.Id == caixa.Id);
            DataGridEntrada.ItemsSource = RepositorioFluxoCaixa.ObterPorParametros(x => x.TipoFluxo == EntradaSaida.Entrada && x.Caixa.Id == caixa.Id);
            DataGridSaida.ItemsSource = RepositorioFluxoCaixa.ObterPorParametros(x => x.TipoFluxo == EntradaSaida.Saída && x.Caixa.Id == caixa.Id);
            DataGridAReceber.ItemsSource = RepositorioConta.ObterTodos().Where(x => x.Pessoa != null).ToList();
            
            
        }   
        #endregion

        public UserControlCaixa(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            VerificaSituacaoCaixa();
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }

        private void btAbrirFecharCaixa_Click(object sender, RoutedEventArgs e)
        {
            if(caixa == null)
            {
                LimpaCampos();
                caixa = new Caixa();
                MessageBoxResult d = MessageBox.Show("Deseja digitar um valor inicial?", "Pergunta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
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

                caixa.Codigo = Repositorio.RetornaUltimoCodigo() + 1;
                caixa.DataAbertura = DateTime.Now;
                caixa.UsuarioAbertura = MainWindow.UsuarioLogado;
                Repositorio.Salvar(caixa);
                ControleAcessoCadastro();
                PreencheCampos();
            }
            else
            {
                MessageBoxResult d = MessageBox.Show("Deseja fechar o Caixa?", "Pergunta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
                {
                    caixa.TotalEntrada = totalEntrada;
                    caixa.TotalSaida = totalSaida;
                    caixa.BalancoFinal = saldoFinal;
                    caixa.DataFechamento = DateTime.Now;
                    caixa.UsuarioFechamento = MainWindow.UsuarioLogado;
                    caixa.Situacao = SituacaoCaixa.Fechado;
                    lblSituacao.Text = caixa.Situacao.ToString();

                    Repositorio.Alterar(caixa);
                    LimpaCampos();
                    ControleAcessoInicial();
                }
            }
        }

        private void btNovoRegistroConta_Click(object sender, RoutedEventArgs e)
        {
            panelCadastro.Children.Clear();
            panelCadastro.Children.Add(new UserControlContas(new Conta(),Session));
        }

        private void btRetirandoCofre_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItemAtualiza_Click(object sender, RoutedEventArgs e)
        {
            TotalizadoresEntradaSaida();
        }

        private void MenuItemAdicionaEntrada_Click(object sender, RoutedEventArgs e)
        {
            EntradaSaidaManual janela = new EntradaSaidaManual(true, caixa, Session);
            janela.ShowDialog();
            TotalizadoresEntradaSaida();
        }

        private void MenuItemAdicionaSaida_Click(object sender, RoutedEventArgs e)
        {
            EntradaSaidaManual janela = new EntradaSaidaManual(false, caixa, Session);
            janela.ShowDialog();
            TotalizadoresEntradaSaida();
        }
    }
}
