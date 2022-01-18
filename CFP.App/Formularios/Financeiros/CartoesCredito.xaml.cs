using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
using CFP.App.Formularios.Pesquisas;
using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using NHibernate;
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
        #endregion

        #region PreencheDataGrid
        private void PreencheDataGrid()
        {

        }
        #endregion
        public CartoesCredito(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void btNovoRegistro_Click(object sender, RoutedEventArgs e)
        {
            AdicionaValoresFatura janela = new AdicionaValoresFatura(Session);
            janela.ShowDialog();
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
                cartaoCredito = null;
                txtFatura.Clear();
                dgCartaoCredito.Items.Clear();
                lblSituacao.Text = SituacaoFatura.Fechada.ToString();
            }
        }

        private void btPesquisar_Click(object sender, RoutedEventArgs e)
        {
            PesquisaCartoesCredito p = new PesquisaCartoesCredito();
            p.ShowDialog();
            if (p.objeto != null)
            {
                cartaoCredito = Repositorio.ObterPorId(p.objeto.Id);
                //PreencheDataGrid();
                txtFatura.Text = cartaoCredito.ToString();
                lblSituacao.Text = cartaoCredito.SituacaoFatura.ToString();

            }
        }
    }
}
