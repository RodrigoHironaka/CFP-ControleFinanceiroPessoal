using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
using CFP.App.Formularios.Pesquisas;
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
                if (caixa.Situacao == SituacaoCaixa.Aberto)
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
            btCofre.IsEnabled = !btCofre.IsEnabled;
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
            btCofre.IsEnabled = true;
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
            if (caixa.Situacao == SituacaoCaixa.Fechado)
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
            DataGridEntrada.ItemsSource = null;
            DataGridSaida.ItemsSource = null;
            DataGridFluxoCaixa.ItemsSource = null;
            DataGridAReceber.ItemsSource = null;
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

        #region Situacao do caixa
        private void VerificandoSituacaoCaixa()
        {
            switch (caixa.Situacao)
            {
                case SituacaoCaixa.Aberto:
                    MenuItemAtualiza.IsEnabled = true;
                    MenuItemAdicionaEntrada.IsEnabled = true;
                    MenuItemAdicionaSaida.IsEnabled = true;
                    btNovoRegistroConta.Visibility = Visibility.Visible;
                    btCofre.Visibility = Visibility.Visible;
                    btTransferirCofre.Visibility = Visibility.Visible;
                    btAbrirFecharCaixa.Visibility = Visibility.Visible;
                    break;
                case SituacaoCaixa.Fechado:
                    btPesquisar.IsEnabled = true;
                    txtCodigo.IsEnabled = true;
                    MenuItemAtualiza.IsEnabled = false;
                    MenuItemAdicionaEntrada.IsEnabled = false;
                    MenuItemAdicionaSaida.IsEnabled = false;
                    btNovoRegistroConta.Visibility = Visibility.Hidden;
                    btCofre.Visibility = Visibility.Hidden;
                    btTransferirCofre.Visibility = Visibility.Hidden;
                    btAbrirFecharCaixa.Visibility = Visibility.Hidden;
                    break;

            }
        }
        #endregion

        #region Totalizadores - Deixei variaveis fora pq uso no metodo SalvarTotais()
        private decimal totalSaida = 0;
        private decimal totalEntrada = 0;
        private decimal saldoFinal = 0;
        private decimal aReceberPessoa = 0;
        private void TotalizadoresEntradaSaida()
        {
            PreencheDataGrid();

            #region Total Saída
            totalSaida = RepositorioFluxoCaixa.ObterTodos()
                .Where(x => x.Caixa.Id == caixa.Id && x.TipoFluxo == EntradaSaida.Saída)
                .Sum(x => x.Valor) * -1;
            txtTotalSaida.Text = String.Format("TOTAL SAÍDA: R${0:n2}", totalSaida);
            #endregion

            #region Total Entrada
            totalEntrada = RepositorioFluxoCaixa.ObterTodos()
                .Where(x => x.Caixa.Id == caixa.Id && x.TipoFluxo == EntradaSaida.Entrada)
                .Sum(x => x.Valor);
            txtTotalEntrada.Text = String.Format("TOTAL ENTRADA: R${0:n2}", totalEntrada);
            #endregion

            #region Saldo Final
            saldoFinal = caixa.ValorInicial + totalEntrada - totalSaida;
            txtSaldoFinal.Text = String.Format("SALDO FINAL : R${0:n2}", saldoFinal);
            #endregion

            #region Total A receber de pessoas referenciadas
            aReceberPessoa = new RepositorioContaPagamento(Session)
                .ObterTodos()
                .Where(x => x.Conta.Pessoa != null &&
                (x.SituacaoParcelas == SituacaoParcela.Pendente ||
                x.SituacaoParcelas == SituacaoParcela.Parcial) &&
                x.DataVencimento.Value.Month <= DateTime.Now.Month &&
                x.DataVencimento.Value.Year <= DateTime.Now.Year &&
                x.Conta.UsuarioCriacao == MainWindow.UsuarioLogado)
                .Sum(x => x.ValorParcela);
            txtTotalAReceber.Text = String.Format("TOTAL: R${0:n2}", aReceberPessoa);
            #endregion

            SalvaTotais();
        }
        #endregion

        #region PreencheDataGrid
        public void PreencheDataGrid()
        {
            #region Agrupamento por forma de pagamento e soma dos valores do fluxo de caixa - ENTRADAS
            DataGridEntrada.ItemsSource = RepositorioFluxoCaixa.ObterPorParametros(x => x.TipoFluxo == EntradaSaida.Entrada && x.Caixa.Id == caixa.Id)
               .ToList()
               .GroupBy(x => x.FormaPagamento)
               .Select(m => new
               {
                   FormaPagamento = m.Key,
                   Valor = m.Sum(x => x.Valor)
               }).OrderBy(x => x.FormaPagamento.Nome);
            #endregion

            #region Agrupamento por forma de pagamento e soma dos valores do fluxo de caixa -SAÍDAS
            DataGridSaida.ItemsSource = RepositorioFluxoCaixa.ObterPorParametros(x => x.TipoFluxo == EntradaSaida.Saída && x.Caixa.Id == caixa.Id)
                .ToList()
                .GroupBy(m => m.FormaPagamento)
                .Select(m => new
                {
                    FormaPagamento = m.Key,
                    Valor = m.Sum(x => x.Valor)
                }).OrderBy(x => x.FormaPagamento.Nome);
            #endregion

            #region Lista inteira Fluxo de Caixa
            DataGridFluxoCaixa.ItemsSource = RepositorioFluxoCaixa.ObterPorParametros(x => x.Caixa.Id == caixa.Id);
            #endregion

            #region Lista dos valores a receber no mês de pessoas referenciadas
            DataGridAReceber.ItemsSource = new RepositorioContaPagamento(Session)
                .ObterPorParametros(x => x.Conta.Pessoa != null &&
                (x.SituacaoParcelas == SituacaoParcela.Pendente ||
                 x.SituacaoParcelas == SituacaoParcela.Parcial) &&
                 x.DataVencimento.Value.Month <= DateTime.Now.Month &&
                 x.DataVencimento.Value.Year <= DateTime.Now.Year &&
                 x.Conta.UsuarioCriacao == MainWindow.UsuarioLogado)
                 .OrderBy(x => x.DataVencimento)
                 .ToList();
            #endregion
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

        #region Salva os totais
        private void SalvaTotais()
        {
            caixa.TotalEntrada = totalEntrada;
            caixa.TotalSaida = totalSaida;
            caixa.BalancoFinal = saldoFinal;
            Repositorio.Alterar(caixa);
        }
        #endregion

        #region Entrada e saida do Fluxo caixa - Cofre
        public void EntradaSaidaCofre(Cofre _cofre)
        {
            FluxoCaixa fluxoCaixa = new FluxoCaixa();
            if (_cofre.Situacao == EntradaSaida.Entrada)
            {
                fluxoCaixa.TipoFluxo = EntradaSaida.Saída;
                fluxoCaixa.Nome = fluxoCaixa.Nome = String.Format("Transferencia para o cofre. Banco: {0}", _cofre.Banco);
                
            }
            else
            {
                fluxoCaixa.TipoFluxo = EntradaSaida.Entrada;
                fluxoCaixa.Nome = fluxoCaixa.Nome = String.Format("Recebimento do cofre. Banco: {0}", _cofre.Banco);
            }
            fluxoCaixa.DataGeracao = DateTime.Now;
            fluxoCaixa.Conta = null;
            fluxoCaixa.UsuarioLogado = MainWindow.UsuarioLogado;
            fluxoCaixa.Valor = _cofre.Valor * -1;
            fluxoCaixa.Caixa = caixa;
            fluxoCaixa.FormaPagamento = _cofre.TransacoesBancarias;
            new RepositorioFluxoCaixa(Session).Salvar(fluxoCaixa);
            TotalizadoresEntradaSaida();
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
            if (caixa == null)
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
                TotalizadoresEntradaSaida();
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
                    caixa = null;
                }
            }
        }

        private void btNovoRegistroConta_Click(object sender, RoutedEventArgs e)
        {
            panelCadastro.Children.Clear();
            panelCadastro.Children.Add(new UserControlContas(new Conta(), Session));
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

        private void btPesquisar_Click_1(object sender, RoutedEventArgs e)
        {
            PesquisaCaixas p = new PesquisaCaixas();
            p.ShowDialog();
            if (p.objeto != null)
            {
                caixa = Repositorio.ObterPorCodigo(p.objeto.Codigo);
                TotalizadoresEntradaSaida();
                PreencheCampos();
                ControleAcessoCadastro();
                VerificandoSituacaoCaixa();
                CorPadrãoBotaoPesquisar();
            }
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    // verifico se campo codigo esta preenchido para incluir novos dados ou carregar um existente
                    if (!String.IsNullOrEmpty(txtCodigo.Text))
                    {
                        #region Conta existente
                        caixa = Repositorio.ObterPorCodigo(Int64.Parse(txtCodigo.Text));
                        if (caixa != null)
                        {
                            TotalizadoresEntradaSaida();
                            PreencheCampos();
                            ControleAcessoCadastro();
                            VerificandoSituacaoCaixa();
                            CorPadrãoBotaoPesquisar();
                        }
                        else
                        {
                            txtCodigo.SelectAll();
                            txtCodigo.Focus();
                            btPesquisar.Background = Brushes.DarkRed;
                        }
                        #endregion
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
                    caixa = null;
                }
            }
        }

        private void txtCodigo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtCodigo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void btTransferirCofre_Click(object sender, RoutedEventArgs e)
        {
            ValoresCofre janela = new ValoresCofre(caixa, Session);
            bool? res = janela.ShowDialog();
            if ((bool)res)
                EntradaSaidaCofre(janela.cofre);
        }

        private void btCofre_Click(object sender, RoutedEventArgs e)
        {
            panelCadastro.Children.Clear();
            panelCadastro.Children.Add(new UserControlCofre(Session));
        }
    }
}
