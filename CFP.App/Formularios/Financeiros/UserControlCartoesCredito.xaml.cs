using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
using CFP.App.Formularios.Pesquisas;
using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
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

namespace CFP.App.Formularios.Financeiros
{
    /// <summary>
    /// Interação lógica para CartoesCredito.xam
    /// </summary>
    public partial class CartoesCredito : UserControl
    {
        ISession Session;
        CartaoCredito cartaoCredito;

        #region Repositorio
        private RepositorioCartaoCredito _repositorio;
        public RepositorioCartaoCredito Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioCartaoCredito(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }

        private RepositorioCartaoCreditoItens _repositorioCartaoCreditoItens;
        public RepositorioCartaoCreditoItens RepositorioCartaoCreditoItens
        {
            get
            {
                if (_repositorioCartaoCreditoItens == null)
                    _repositorioCartaoCreditoItens = new RepositorioCartaoCreditoItens(Session);

                return _repositorioCartaoCreditoItens;
            }
            set { _repositorioCartaoCreditoItens = value; }
        }
        #endregion

        #region PreencheDataGrid
        private void PreencheDataGrid()
        {
            cartaoCredito.CartaoCreditos = RepositorioCartaoCreditoItens.ObterPorParametros(x => x.CartaoCredito.Id == cartaoCredito.Id).ToList();
            dgCartaoCredito.ItemsSource = cartaoCredito.CartaoCreditos;
            if (cartaoCredito.CartaoCreditos.Count > 0)
                txtTotalFatura.Text = String.Format("TOTAL: {0:C}", cartaoCredito.CartaoCreditos.Sum(x => x.Valor));
        }
        #endregion

        #region AcessoBotoes
        private void AcessoBotoes(bool manterAtivo)
        {
            if (manterAtivo)
            {
                btNovoRegistro.IsEnabled = true;
                btExcluir.IsEnabled = true;
            }
            else
            {
                btNovoRegistro.IsEnabled = false;
                btExcluir.IsEnabled = false;
            }
            
        }
        #endregion

        public CartoesCredito(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void btNovoRegistro_Click(object sender, RoutedEventArgs e)
        {
            AdicionaValoresFatura janela = new AdicionaValoresFatura(new CartaoCreditoItens(), cartaoCredito, Session);
            bool? res = janela.ShowDialog();
            if ((bool)res)
                PreencheDataGrid();
        }

        private void btAbrirFecharFatura_Click(object sender, RoutedEventArgs e)
        {
            if (cartaoCredito == null)
            {
                AbrirFecharFaturaCartaoCredito janela = new AbrirFecharFaturaCartaoCredito(new CartaoCredito(), "ABRIR NOVA FATURA", Session);
                bool? res = janela.ShowDialog();
                if ((bool)res)
                {
                    cartaoCredito = janela.cartaoCredito;
                    txtFatura.Text = cartaoCredito.ToString();
                    lblSituacao.Text = cartaoCredito.SituacaoFatura.ToString();
                    new RepositorioConta(Session).CriarNovaContaPadrao(MainWindow.UsuarioLogado, String.Format("Fatura {0}", cartaoCredito.DescricaoCompleta), null, cartaoCredito.Cartao, null);
                }
            }
            else
            {
                if(cartaoCredito.SituacaoFatura != SituacaoFatura.Fechada)
                {
                    MessageBoxResult d = MessageBox.Show("Deseja fechar o fatura?", "Pergunta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        AbrirFecharFaturaCartaoCredito janela = new AbrirFecharFaturaCartaoCredito(cartaoCredito, "FECHAR FATURA", Session);
                        bool? res = janela.ShowDialog();
                        if ((bool)res)
                        {
                            btLimpaPesquisa_Click(sender, e);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Essa fatura já esta fechada!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
               
            }
        }

        private void btLimpaPesquisa_Click(object sender, RoutedEventArgs e)
        {
            if (cartaoCredito != null)
            {
                cartaoCredito.CartaoCreditos.Clear();
                cartaoCredito = null;
                txtFatura.Clear();
                lblSituacao.Text = SituacaoFatura.Fechada.ToString();
                dgCartaoCredito.ItemsSource = null;
                txtTotalFatura.Text = String.Format("TOTAL: {0:C}", 0);
                AcessoBotoes(false);
            }
        }

        private void btPesquisar_Click(object sender, RoutedEventArgs e)
        {
            PesquisaCartoesCredito p = new PesquisaCartoesCredito();
            p.ShowDialog();
            if (p.objeto != null)
            {
                cartaoCredito = Repositorio.ObterPorId(p.objeto.Id);
                PreencheDataGrid();
                txtFatura.Text = cartaoCredito.ToString();
                lblSituacao.Text = cartaoCredito.SituacaoFatura.ToString();
                if (cartaoCredito.SituacaoFatura != SituacaoFatura.Fechada)
                    AcessoBotoes(true);
                else
                    AcessoBotoes(false);

            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AcessoBotoes(false);
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Grid).Children.Remove(this);
        }

        private void miEditarItemFatura_Click(object sender, RoutedEventArgs e)
        {
            CartaoCreditoItens selecao = (CartaoCreditoItens)dgCartaoCredito.SelectedItem;
            AdicionaValoresFatura janela = new AdicionaValoresFatura(selecao, cartaoCredito, Session);
            bool? res = janela.ShowDialog();
            if ((bool)res)
                PreencheDataGrid();
        }

        private void miRemoverItemFatura_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CartaoCreditoItens selecao = (CartaoCreditoItens)dgCartaoCredito.SelectedItem;
                cartaoCredito.CartaoCreditos.Remove(selecao);// removendo da lista para ao excluir nao acontecer erro de de cascade
                if (selecao != null)
                {
                    MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + selecao.Nome + " - "+ selecao.Valor +" ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        RepositorioCartaoCreditoItens.Excluir(selecao);
                        PreencheDataGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível excluir esse registro! Erro:" + ex.ToString(), "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                Session.Clear();
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cartaoCredito != null)
                {
                    MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + cartaoCredito.DescricaoCompleta + "? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        Repositorio.Excluir(cartaoCredito);
                        btLimpaPesquisa_Click(sender, e);
                        AcessoBotoes(false);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Não foi possível excluir esse registro! Erro:" + ex.ToString(), "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                Session.Clear();
            }
        }
    }
}
