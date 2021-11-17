using CFP.Dominio.ObjetoValor;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using LinqKit;
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

namespace CFP.App.Formularios.Financeiros.Consultas
{
    /// <summary>
    /// Interação lógica para ucConsultaContas.xam
    /// </summary>
    public partial class ucConsultaContas : UserControl
    {
        ISession Session;

        #region Repositorio
        private RepositorioContaPagamento _repositorioContaPagamento;
        public RepositorioContaPagamento RepositorioContaPagamento
        {
            get
            {
                if (_repositorioContaPagamento == null)
                    _repositorioContaPagamento = new RepositorioContaPagamento(Session);

                return _repositorioContaPagamento;
            }
            set { _repositorioContaPagamento = value; }
        }
        #endregion

        #region Preenche DataGrid
        public void PreencheDataGrid()
        {
            var predicado = RepositorioContaPagamento.CriarPredicado();
            predicado = predicado.And(x => x.Conta.UsuarioCriacao == MainWindow.UsuarioLogado);

            if(!String.IsNullOrEmpty(txtPesquisa.Text))
                predicado = predicado.And(x => x.Conta.Nome.Contains(txtPesquisa.Text) || x.ValorParcela.ToString().Contains(txtPesquisa.Text) || x.Conta.Codigo.ToString().Contains(txtPesquisa.Text) || x.Conta.NumeroDocumento.ToString().Contains(txtPesquisa.Text));

            if (cmbSituacaoParcelas.SelectedIndex != -1)
                predicado = predicado.And(x => x.SituacaoParcelas == (SituacaoParcela)cmbSituacaoParcelas.SelectedIndex);
            else
                predicado = predicado.And(x => x.SituacaoParcelas == SituacaoParcela.Pendente || x.SituacaoParcelas == SituacaoParcela.Parcial);

            if (cmbTipoConta.SelectedIndex != -1)
                predicado = predicado.And(x => x.Conta.TipoConta == (TipoConta)cmbTipoConta.SelectedIndex);

            if (cmbPeriodo.SelectedIndex != -1)
                predicado = predicado.And(x => x.Conta.TipoPeriodo == (TipoPeriodo)cmbPeriodo.SelectedIndex);
           
            if (cmbFormaCompra.SelectedItem != null)
                predicado = predicado.And(x => x.Conta.FormaCompra == cmbFormaCompra.SelectedItem);

            if (cmbPessoaReferenciada.SelectedItem != null)
                predicado = predicado.And(x => x.Conta.Pessoa == cmbPessoaReferenciada.SelectedItem);

            if (dtpInicio.SelectedDate != null)
                predicado = predicado.And(x => x.DataVencimento >= dtpInicio.SelectedDate);

            if (dtpFinal.SelectedDate != null)
            {
                if (dtpFinal.SelectedDate < dtpInicio.SelectedDate)
                    MessageBox.Show("Data Final é menor que a data Inicial. Por favor Verifique!", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    predicado = predicado.And(x => x.DataVencimento <= dtpFinal.SelectedDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59));
            }

            var filtro = RepositorioContaPagamento.ObterPorParametros(predicado).ToList();
            dgContasFiltradas.ItemsSource = filtro;
            if (filtro.Count > 0)
                txtTotalFiltro.Text = String.Format("Total: {0}", filtro.Sum(x => x.ValorParcela).ToString("N2"));
        }
        #endregion

        #region Carrega Combo
        private void CarregaCombo()
        {
            cmbTipoConta.ItemsSource = Enum.GetValues(typeof(TipoConta));
            cmbPeriodo.ItemsSource = Enum.GetValues(typeof(TipoPeriodo));
            cmbSituacaoParcelas.ItemsSource = Enum.GetValues(typeof(SituacaoParcela));

            cmbFormaCompra.ItemsSource = new RepositorioFormaPagamento(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.TransacoesBancarias == SimNao.Não)
                .OrderBy(x => x.Nome)
                .ToList();

            cmbPessoaReferenciada.ItemsSource = new RepositorioPessoa(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
        }
        #endregion

        #region Verificando primeiro e ultimo dia do Mes
        private void PrimeiroUltimoDiaMes()
        {
            DateTime data = DateTime.Today;
            dtpInicio.SelectedDate = new DateTime(data.Year, data.Month, 1);
            dtpFinal.SelectedDate = new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));
        }
        #endregion

        public ucConsultaContas(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombo();
            PrimeiroUltimoDiaMes();
        }

        private void btFiltro_Click(object sender, RoutedEventArgs e)
        {
            PreencheDataGrid();
        }

        private void dtpInicio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                dtpInicio.Text = string.Empty;
        }

        private void dtpFinal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                dtpFinal.Text = string.Empty;
        }

        private void cmbTipoConta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbTipoConta.SelectedIndex = -1;
        }

        private void cmbPeriodo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbPeriodo.SelectedIndex = -1;
        }

        private void cmbFormaCompra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbFormaCompra.SelectedItem = null;
        }

        private void cmbPessoaReferenciada_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbPessoaReferenciada.SelectedItem = null;
        }

        private void txtPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                txtPesquisa.Text = string.Empty;
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }

        private void cmbSituacaoParcelas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbSituacaoParcelas.SelectedIndex = -1;
        }
    }
}
