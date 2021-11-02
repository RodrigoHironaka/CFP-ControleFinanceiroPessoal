using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObejtoValor;
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
                }
            }
            else
            {
                ControleAcessoInicial();
            }
            
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
                txtSaldoFinal.Text = string.Format("ALDO FINAL R$ {0:n2}", caixa.BalancoFinal);
                txtDataFechamento.Visibility = Visibility.Visible;
                txtDataFechamento.Text = caixa.DataFechamento.ToString();
            }
            

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


                }
            }
        }
    }
}
