using CFP.App.Formularios.Cadastros;
using CFP.App.Formularios.Financeiros;
using CFP.App.Formularios.Financeiros.Consultas;
using CFP.App.Formularios.ModeloBase.UserControls;
using CFP.App.Formularios.Principais;
using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Ferramentas;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObjetoValor;
using LinqKit;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using NHibernate;
using Repositorio.Repositorios;
using SGE.Repositorio.Configuracao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CFP.App
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        Configuracao config;
        List<ContaPagamento> contaPagamento = new List<ContaPagamento>();

        #region Usuario Logado
        public static Usuario UsuarioLogado;
        #endregion

        #region Session
        private static ISession session;
        protected static ISession Session
        {
            get
            {
                if (session == null || !session.IsOpen)
                {
                    if (session != null)
                        session.Dispose();
                    session = NHibernateHelper.GetSession();
                }
                return session;
            }
        }
        #endregion

        #region Resumo
        public void ResumoTela()
        {
            DateTime data = DateTime.Today;
            DateTime primeiroDia = new DateTime(data.Year, data.Month, 1);
            DateTime ultimoDia = new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));

            contaPagamento.Clear();
            contaPagamento = new RepositorioContaPagamento(Session).ObterTodos()
                .Where(x => x.DataVencimento <= ultimoDia.Date.AddHours(23).AddMinutes(59).AddSeconds(59) &&
                x.Conta.UsuarioCriacao.Id == UsuarioLogado.Id).ToList();

            #region Totais do resumo
            Decimal totalPagar = contaPagamento.Where(x => (x.SituacaoParcelas == SituacaoParcela.Pendente || x.SituacaoParcelas == SituacaoParcela.Parcial) && x.Conta.TipoConta == TipoConta.Pagar && x.Conta.FormaCompra.UsadoParaCompras == SimNao.Não).Select(x => x.ValorReajustado).Sum();
            Decimal totalReceber = contaPagamento.Where(x => (x.SituacaoParcelas == SituacaoParcela.Pendente || x.SituacaoParcelas == SituacaoParcela.Parcial) && x.Conta.TipoConta == TipoConta.Receber).Select(x => x.ValorReajustado).Sum();
            Decimal totalCartao = contaPagamento.Where(x => (x.SituacaoParcelas == SituacaoParcela.Pendente || x.SituacaoParcelas == SituacaoParcela.Parcial) && x.Conta.TipoConta == TipoConta.Pagar && x.Conta.FormaCompra.UsadoParaCompras == SimNao.Sim).Select(x => x.ValorReajustado).Sum();
           
            txtValorTotalPagar.Text = String.Format("{0:C}", totalPagar);

            txtValorTotalReceber.Text = String.Format("{0:C}", totalReceber);

            txtValorTotalCartoes.Text = String.Format("{0:C}", totalCartao);
            #endregion

            #region Calculo Restante do mes
            List<ContaPagamento> novoContaPagamento = new List<ContaPagamento>();
            novoContaPagamento = new RepositorioContaPagamento(Session).ObterTodos()
               .Where(x => x.DataVencimento >= primeiroDia.Date && x.DataVencimento <= ultimoDia.Date.AddHours(23).AddMinutes(59).AddSeconds(59) &&
               x.Conta.UsuarioCriacao.Id == UsuarioLogado.Id).ToList();
            Decimal totalPagarFixo = novoContaPagamento.Where(x => x.Conta.TipoConta == TipoConta.Pagar && x.Conta.FormaCompra.UsadoParaCompras == SimNao.Não).Select(x => x.ValorReajustado).Sum();
            Decimal totalReceberFixo = novoContaPagamento.Where(x => x.Conta.TipoConta == TipoConta.Receber).Select(x => x.ValorReajustado).Sum();
            Decimal totalCartaoFixo = novoContaPagamento.Where(x => x.Conta.TipoConta == TipoConta.Pagar && x.Conta.FormaCompra.UsadoParaCompras == SimNao.Sim).Select(x => x.ValorReajustado).Sum();
            Decimal totalBancosFixo = new RepositorioCofre(Session).ObterTodos().Where(x => x.Banco.UsarValorParaCalculos == SimNao.Sim).Select(x => x.Valor).Sum();

            List<decimal> valoresRendas = new RepositorioPessoa(Session).ObterPorParametros(x => x.UsarRendaParaCalculos == SimNao.Sim).Select(x => x.ValorTotalLiquido).ToList();
            decimal ValorRenda = valoresRendas.Count != 0 ? valoresRendas.Sum() : 0;

            decimal RestanteMes = ValorRenda + totalBancosFixo + totalReceberFixo - totalCartaoFixo - totalPagarFixo;
                       
            txtRestante.Text = String.Format("RESTANTE DO MÊS {0:C}", RestanteMes);
            #endregion

            AlertaContas();
        }
        #endregion

        #region Pegando as Configuracoes
        private void ConfiguracoesSistema()
        {
            Session.Clear();
            config = new RepositorioConfiguracao(Session).ObterPorParametros(x => x.UsuarioCriacao.Id == UsuarioLogado.Id).FirstOrDefault();
        }
        #endregion

        #region Consulta Alerta
        public ObservableCollection<AlertaContas> alertas;
      
        private void AlertaContas()
        {
            string msg = string.Empty;
            TipoAlertaContas tipoAlerta = TipoAlertaContas.Aviso;
            ConfiguracoesSistema();
            alertas = new ObservableCollection<AlertaContas>();
            foreach (var parcela in contaPagamento.Where(x => x.SituacaoParcelas == SituacaoParcela.Pendente || x.SituacaoParcelas == SituacaoParcela.Parcial))
            {
                if(DateTime.Now > parcela.DataVencimento.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59))
                {
                    if (parcela.Conta.TipoConta == TipoConta.Pagar)
                        msg = "Parcela já passou da data de vencimento! VERIFIQUE!";
                    else
                        msg = "Recebimento já passou da data! VÁ RECEBER!";
                    tipoAlerta = TipoAlertaContas.Atrasado;
                }
                else if (DateTime.Now.Date == parcela.DataVencimento.Value.Date)
                {
                    if(parcela.Conta.TipoConta == TipoConta.Pagar)
                        msg = "Parcela vence hoje! NÃO DEIXE ATRASAR!";
                    else
                        msg = "Recebimento para hoje! NÃO ESQUEÇA!";
                    tipoAlerta = TipoAlertaContas.Atrasado;
                }
                else
                {
                    var dias = DateTime.Parse(parcela.DataVencimento.ToString()).Subtract(DateTime.Now).Days;
                    if (config != null)
                    {
                        if (dias > 0 && dias <= config.DiasAlertaVencimento)
                        {
                            if (parcela.Conta.TipoConta == TipoConta.Pagar)
                                msg = "Parcela esta próxima do vencimento! FIQUE DE OLHO!";
                            else
                                msg = "Esta chegando o dia para receber! PREPARE-SE!";
                            tipoAlerta = TipoAlertaContas.Aviso;
                        }
                        else
                            continue;
                    }
                }
                alertas.Add(new AlertaContas()
                {
                    TipoAlertaContas = tipoAlerta,
                    Mensagem = msg,
                    ContaPagamento = parcela
                });
            }
            if(alertas.Count == 0)
                bAlertas.Visibility = Visibility.Hidden;
            else
                bAlertas.Visibility = Visibility.Visible;
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonPopUpSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int indexOpcoesMenu = ListViewMenu.SelectedIndex;

            switch (indexOpcoesMenu)
            {
                case 0:
                    GridPrincipal.Children.Clear();
                    ConfiguracoesSistema();
                    ResumoTela();
                    ListViewItemHome.IsSelected = false;
                    break;
                case 1:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new ucConsultaContas(Session));
                    ListVIewItemConsultaConta.IsSelected = false;
                    break;
                case 2:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlContas(new Conta(), Session));
                    ListVIewItemContas.IsSelected = false;
                    break;
                case 3:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new CartoesCredito(Session));
                    ListVIewItemCartoes.IsSelected = false;
                    break;
                case 4:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlCaixa(Session));
                    ListVIewItemCaixa.IsSelected = false;
                    break;
                case 5:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlCadastros());
                    ListVIewItemCadastro.IsSelected = false;
                    break;
                default:
                    break;
            }
        }

        private void Principal_Loaded(object sender, RoutedEventArgs e)
        {
            #region Login
            Principal.Visibility = Visibility.Hidden;
            Login login = new Login();
            bool? confirmacao = login.ShowDialog();
            if ((bool)confirmacao)
            {
                UsuarioLogado = login.UsuarioLogado;
                lblNomeUsuario.Text = UsuarioLogado.Nome.ToUpper();
                Principal.Visibility = Visibility.Visible;
                login.Close();
                ConfiguracoesSistema();
                ResumoTela();
            }
            else
            {
                Close();
            }
            #endregion
        }

        private void ButtonConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlConfiguracoes());
        }

        private void ButtonBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = System.DateTime.Now.ToShortDateString().Replace("/", "");
                string hora = System.DateTime.Now.ToLongTimeString().Replace(":", "");
                string caminhoPadrao = config.CaminhoBackup;//new RepositorioConfiguracao(Session).ObterTodos().Where(x => x.UsuarioCriacao == MainWindow.UsuarioLogado).FirstOrDefault().CaminhoBackup;
                string backupSalvar = caminhoPadrao + "\\CFP_" + data + "_" + hora + ".sql";

                if (caminhoPadrao != null)
                {
                    using (MySqlConnection conn = new MySqlConnection(ArquivosXML.StringConexao()))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup bk = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                Mouse.OverrideCursor = Cursors.Wait;
                                bk.ExportToFile(backupSalvar);
                                Mouse.OverrideCursor = null;
                                conn.Close();
                                MessageBox.Show("Backup Salvo em " + backupSalvar);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Defina o caminho padrão para realizar o backup em configurações!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ButtonUsuarios_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlUsuario(new Usuario(), Session));
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GridPrincipal.Children.Clear();
            ConfiguracoesSistema();
            ResumoTela();
        }

        private void btAlertas_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new ucAlertaContas(alertas,Session));
        }

        private void btAPagar_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new ucConsultaContas(0, Session));
        }

        private void btReceber_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new ucConsultaContas(1, Session));
        }

        private void btCartoes_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new ucConsultaContas(Session));
        }

        private void ButtonPopUpMax_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized )
            {
                ButtonPopUpMax.Content = "Maximizar";
                WindowState = WindowState.Normal;
            }
            else
            {
                ButtonPopUpMax.Content = "Rest. Tamanho";
                WindowState = WindowState.Maximized;
            }
        }

        private void Principal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ButtonPopUpMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ListVIewItemCartoes_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
