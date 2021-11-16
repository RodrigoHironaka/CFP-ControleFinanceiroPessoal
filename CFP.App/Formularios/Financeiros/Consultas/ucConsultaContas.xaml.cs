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
        private RepositorioConta _repositorio;
        public RepositorioConta Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioConta(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        #region Preenche DataGrid
        public void PreencheDataGrid()
        {
            var predicado = Repositorio.CriarPredicado();
            predicado = predicado.And(x => x.UsuarioCriacao == MainWindow.UsuarioLogado);

            if(!String.IsNullOrEmpty(txtPesquisa.Text))
                predicado = predicado.And(x => x.Nome.Contains(txtPesquisa.Text) || x.ValorTotal.ToString().Contains(txtPesquisa.Text) || x.Codigo.ToString().Contains(txtPesquisa.Text));

            if (cmbTipoConta.SelectedIndex != -1)
                predicado = predicado.And(x => x.TipoConta == (cmbTipoConta.SelectedIndex == 0 ? TipoConta.Pagar : TipoConta.Receber));

            if (cmbPeriodo.SelectedIndex != -1)
                predicado = predicado.And(x => x.TipoPeriodo == (TipoPeriodo)cmbPeriodo.SelectedIndex);

            if (cmbFormaCompra.SelectedItem != null)
                predicado = predicado.And(x => x.FormaCompra == cmbFormaCompra.SelectedItem);

            if (cmbPessoaReferenciada.SelectedItem != null)
                predicado = predicado.And(x => x.Pessoa == cmbPessoaReferenciada.SelectedItem);

            if (dtpInicio.SelectedDate != null)
                predicado = predicado.And(x => x.DataGeracao >= dtpInicio.SelectedDate);

            if (dtpFinal.SelectedDate != null)
            {
                if (dtpFinal.SelectedDate < dtpInicio.SelectedDate)
                    MessageBox.Show("Data Final é menor que a data Inicial. Por favor Verifique!", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    predicado = predicado.And(x => x.DataGeracao <= dtpFinal.SelectedDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59));
            }

           

            var filtro = Repositorio.ObterPorParametros(predicado).ToList();
            dgContasFiltradas.ItemsSource = filtro;
            //if (filtro.Count > 0)
            //    txtTotalFiltro.Text = String.Format("Total: {0}", filtro.Sum(x => x.Valor).ToString("N2"));
        }
        #endregion

        #region Carrega Combo
        private void CarregaCombo()
        {
            cmbTipoConta.ItemsSource = Enum.GetValues(typeof(TipoConta));
            cmbPeriodo.ItemsSource = Enum.GetValues(typeof(TipoPeriodo));

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
    }
}
