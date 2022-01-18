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
    /// Lógica interna para AbrirFecharFaturaCartaoCredito.xaml
    /// </summary>
    public partial class AbrirFecharFaturaCartaoCredito : Window
    {
        ISession Session;
        public CartaoCredito cartaoCredito;

        #region Carrega Combos
        private void CarregaCombos()
        {
            cmbMes.ItemsSource = Enum.GetValues(typeof(Meses));
            var mes = DateTime.Now.Month;
            cmbMes.SelectedIndex = mes - 1;

            cmbAno.ItemsSource = Enumerable.Range(2010, DateTime.Now.Year - 2009).Reverse().ToList();
            var ano = DateTime.Now.Year;
            cmbAno.SelectedItem = ano;

            cmbCartao.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.UsadoParaCompras == SimNao.Sim)
               .OrderBy(x => x.Nome)
               .ToList();
            cmbCartao.SelectedIndex = 0;
        }
        #endregion

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

        #region Preenche Campos
        private void PreencheCampos()
        {
            if(cartaoCredito != null)
            {
                cmbMes.SelectedIndex = cartaoCredito.MesReferencia.GetHashCode() - 1;
                cmbAno.SelectedItem = cartaoCredito.AnoReferencia;
                cmbCartao.SelectedItem = cartaoCredito.Cartao;
                txtValor.Text = cartaoCredito.CartaoCreditos != null ? cartaoCredito.CartaoCreditos.Select(x => x.Valor).Sum().ToString("N2") : "0,00";
                txtObservacao.Text = cartaoCredito.Nome;
            }
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                //tab Geral
                cartaoCredito.MesReferencia = (int)cmbMes.SelectedItem;
                cartaoCredito.AnoReferencia = (int)cmbAno.SelectedItem;
                cartaoCredito.Cartao = (FormaPagamento)cmbCartao.SelectedItem;
                cartaoCredito.ValorFatura = Decimal.Parse(txtValor.Text);
                cartaoCredito.Nome = txtObservacao.Text;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
                //return false;
            }
        }

       
        #endregion

        public AbrirFecharFaturaCartaoCredito(CartaoCredito _cartaCredito, String nomeTela, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            lblNomeTela.Text = nomeTela;
            cartaoCredito = _cartaCredito;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombos();
            if(cartaoCredito.Id > 0)
                PreencheCampos();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (PreencheObjeto())
            {
                if (cartaoCredito.Id == 0) 
                {
                    cartaoCredito.DataGeracao = DateTime.Now;
                    cartaoCredito.UsuarioCriacao = MainWindow.UsuarioLogado;
                    cartaoCredito.SituacaoFatura = SituacaoFatura.Aberta;
                    Repositorio.Salvar(cartaoCredito);
                }
                else
                {
                    cartaoCredito.DataAlteracao = DateTime.Now;
                    cartaoCredito.UsuarioAlteracao = MainWindow.UsuarioLogado;
                    cartaoCredito.SituacaoFatura = SituacaoFatura.Fechada;
                    Repositorio.Alterar(cartaoCredito);
                    
                }
                DialogResult = true;
            }
        }
    }
}
