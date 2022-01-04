using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
using CFP.Dominio.ObjetoValor;
using CFP.Ferramentas.Exportar;
using Dominio.Dominio;
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

namespace CFP.App.Formularios.Financeiros
{
    /// <summary>
    /// Interação lógica para UserControlCofre.xam
    /// </summary>
    public partial class UserControlCofre : UserControl
    {
        ISession Session;
        Cofre cofre;

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
        #endregion

        #region Preenche DataGrid
        public void PreencheDataGrid()
        {
            var predicado = Repositorio.CriarPredicado();
            predicado = predicado.And(x => x.UsuarioCriacao == MainWindow.UsuarioLogado);

            if (cmbEntradaSaida.SelectedIndex != -1)
                predicado = predicado.And(x => x.Situacao == (cmbEntradaSaida.SelectedIndex == 0 ? EntradaSaida.Entrada : EntradaSaida.Saída));

            if (cmbBanco.SelectedItem != null)
                predicado = predicado.And(x => x.Banco == cmbBanco.SelectedItem);

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
            DataGridCofre.ItemsSource = filtro;
            if (filtro.Count > 0)
                txtTotalFiltro.Text = String.Format("TOTAL: {0:C}", filtro.Sum(x => x.Valor));
        }
        #endregion

        #region Carrega Combo
        private void CarregaCombo()
        {
            cmbEntradaSaida.ItemsSource = Enum.GetValues(typeof(EntradaSaida));

            cmbBanco.ItemsSource = new RepositorioBanco(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
                .OrderBy(x => x.Nome)
                .ToList();
        }
        #endregion

        #region Verificando primeiro e ultimo dia do Mes
        private void PrimeiroUltimoDiaMes()
        {
            DateTime data = DateTime.Today;
            //dtpInicio.SelectedDate = new DateTime(data.Year, data.Month, 1);
            dtpFinal.SelectedDate = new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));
        }
        #endregion

        public UserControlCofre(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void btAdicionar_Click(object sender, RoutedEventArgs e)
        {
            ValoresCofre janela = new ValoresCofre(Session);
            janela.ShowDialog();
            PreencheDataGrid();
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Grid).Children.Remove(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PrimeiroUltimoDiaMes();
            CarregaCombo();
            PreencheDataGrid();
            btFiltro_Click(sender, e);
        }

        private void DataGridCofre_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Cofre selecao = (Cofre)DataGridCofre.SelectedItem;
            cofre = Repositorio.ObterPorCodigo(selecao.Codigo);
            ValoresCofre janela = new ValoresCofre(cofre, Session);
            janela.ShowDialog();
            PreencheDataGrid();
        }

        private void DataGridCofre_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

        private void btFiltro_Click(object sender, RoutedEventArgs e)
        {
            PreencheDataGrid();
        }

        private void cmbEntradaSaida_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbEntradaSaida.SelectedIndex = -1;
        }

        private void cmbBanco_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbBanco.SelectedItem = null;
        }

        private void menuItemExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            Ferramentas.Exportar.ExportarExcel.ExpExcel(DataGridCofre);
        }

        private void DataGridCofre_MouseUp(object sender, MouseButtonEventArgs e)
        {
            decimal valores = 0;
            List<Cofre> selecoes = new List<Cofre>();
            foreach (Cofre item in DataGridCofre.SelectedItems)
            {
                selecoes.Add(item);
                valores += item.Valor;
            }
            if (selecoes.Count > 0)
            {
                txtTotalFiltro.Text = string.Empty;
                txtTotalFiltro.Text += String.Format("TOTAL: {0:C}", valores);

            }
        }

        private void btTranferenciaCofres_Click(object sender, RoutedEventArgs e)
        {
            TransferenciaCofres janela = new TransferenciaCofres(Session);
            bool? res = janela.ShowDialog();
            if ((bool)res)
                btFiltro_Click(sender, e);
        }


        //private void menuItemExportarPdf_Click(object sender, RoutedEventArgs e)
        //{
        //    ExportarPDF p = new ExportarPDF();
        //    p.ExpPdf(DataGridCofre);
        //}
    }
}
