using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObjetoValor;
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
using LinqKit;
using CFP.Dominio.Dominio;

namespace CFP.App.Formularios.Financeiros
{
    /// <summary>
    /// Interação lógica para UserControlHoraExtra.xam
    /// </summary>
    public partial class UserControlHoraExtra : UserControl
    {
        ISession Session;
        HoraExtra horaExtra;
        Configuracao config;

        #region Pegando as Configuracoes
        private void ConfiguracoesSistema()
        {
            Session.Clear();
            config = new RepositorioConfiguracao(Session).ObterPorParametros(x => x.UsuarioCriacao.Id == MainWindow.UsuarioLogado.Id).FirstOrDefault();
        }
        #endregion

        #region Carrega Combos
        private void CarregaCombos()
        {
            cmbPeriodoDia.ItemsSource = Enum.GetValues(typeof(PeriodoDia));
            cmbPeriodoDia.SelectedIndex = 0;

            cmbPessoa.ItemsSource = new RepositorioPessoa(Session)
           .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
           .OrderBy(x => x.Nome)
           .ToList();
            cmbPessoa.SelectedIndex = 0;

            cmbFiltroPessoa.ItemsSource = new RepositorioPessoa(Session)
           .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
           .OrderBy(x => x.Nome)
           .ToList();
        }
        #endregion

        #region Repositorio
        private RepositorioHoraExtra _repositorio;
        public RepositorioHoraExtra Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioHoraExtra(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        #region Preenche Objeto para Salvar
        //DateTime meioDia = Convert.ToDateTime("12:00:00");
        private bool PreencheObjeto()
        {
            try
            {
                horaExtra.Nome = txtDescricao.Text;
                horaExtra.Pessoa = (Pessoa)cmbPessoa.SelectedItem;
                horaExtra.DataHoraExtra = (DateTime)txtDataHoraExtra.SelectedDate;
                switch (cmbPeriodoDia.SelectedItem)
                {
                    case PeriodoDia.Manha:
                        horaExtra.HoraInicioManha = txtHoraInicio.SelectedTime != null ? txtHoraInicio.SelectedTime.Value.TimeOfDay : TimeSpan.Zero;
                        horaExtra.HoraFinalManha = txtHoraFinal.SelectedTime != null ? txtHoraFinal.SelectedTime.Value.TimeOfDay : TimeSpan.Zero;
                        break;
                    case PeriodoDia.Tarde:
                        horaExtra.HoraInicioTarde = txtHoraInicio.SelectedTime != null ? txtHoraInicio.SelectedTime.Value.TimeOfDay : TimeSpan.Zero;
                        horaExtra.HoraFinalTarde = txtHoraFinal.SelectedTime != null ? txtHoraFinal.SelectedTime.Value.TimeOfDay : TimeSpan.Zero;
                        break;
                    case PeriodoDia.Noite:
                        horaExtra.HoraInicioNoite = txtHoraInicio.SelectedTime != null ? txtHoraInicio.SelectedTime.Value.TimeOfDay : TimeSpan.Zero;
                        horaExtra.HoraFinalNoite = txtHoraFinal.SelectedTime != null ? txtHoraFinal.SelectedTime.Value.TimeOfDay : TimeSpan.Zero;
                        break;
                }
                return true;
            }
            catch (Exception)
            {
                //throw new Exception(ex.ToString());
                return false;
            }
        }
        #endregion

        #region Preenche campos no user control
        private void PreencheCampos()
        {
            if (horaExtra != null)
            {
                txtCodigo.Text = horaExtra.Id.ToString();
                txtDescricao.Text = horaExtra.Nome;
                cmbPessoa.SelectedItem = horaExtra.Pessoa;
                txtDataHoraExtra.SelectedDate = horaExtra.DataHoraExtra;
            }
        }
        #endregion

        #region Limpa os campos do Cadastro
        public void LimpaCampos()
        {
            foreach (var item in GridControls.Children)
            {
                txtDescricao.Clear();
                cmbPessoa.SelectedIndex = 0;
                cmbPeriodoDia.SelectedIndex = 0;
                txtDataHoraExtra.Text = string.Empty;
                txtHoraInicio.Text = string.Empty;
                txtHoraFinal.Text = string.Empty;
                txtCodigo.Text = string.Empty;
                GridControls.IsEnabled = false;
            }
        }
        #endregion

        #region PreencheDataGrid
        private void PreencheDataGrid()
        {
            var registros = Repositorio.ObterTodos().OrderBy(x => x.DataHoraExtra).ToList();
            foreach (var item in registros)
            {
                if (item.TotalManha == TimeSpan.Zero)
                    item.HoraFinalDia = item.TotalManha + item.TotalTarde + item.TotalNoite - config.HorasTrabalhadasPorPeriodo;
                else if (item.TotalTarde == TimeSpan.Zero)
                    item.HoraFinalDia = item.TotalManha + item.TotalTarde + item.TotalNoite - config.HorasTrabalhadasPorPeriodo;
                else
                    item.HoraFinalDia = item.TotalManha + item.TotalTarde + item.TotalNoite - config.HorasTrabalhadasPorDia;
            }

            dgHoraExtra.ItemsSource = registros;
        }
        #endregion

        #region Controle paineis
        //private void ControleAcessoInicial()
        //{
        //    GridControls.IsEnabled = !GridControls.IsEnabled;
        //    btExcluir.IsEnabled = !btExcluir.IsEnabled;
        //}
        #endregion

       

        public UserControlHoraExtra(HoraExtra _horaExtra, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            horaExtra = _horaExtra;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ConfiguracoesSistema();
            CarregaCombos();
            PreencheDataGrid();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtDescricao.Text) || String.IsNullOrEmpty(txtDataHoraExtra.Text) ||
                String.IsNullOrEmpty(txtHoraInicio.Text) || String.IsNullOrEmpty(txtHoraFinal.Text) ||
                cmbPessoa.SelectedIndex == -1 || cmbPeriodoDia.SelectedIndex == -1)
            {
                MessageBox.Show("Todos os campos são obrigatórios!", "Informacão", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (PreencheObjeto())
            {
                if (horaExtra.Id == 0)
                {
                    horaExtra.DataGeracao = DateTime.Now;
                    horaExtra.UsuarioCriacao = MainWindow.UsuarioLogado;
                    Repositorio.Salvar(horaExtra);
                }
                else
                {
                    horaExtra.DataAlteracao = DateTime.Now;
                    horaExtra.UsuarioAlteracao = MainWindow.UsuarioLogado;
                    Repositorio.Alterar(horaExtra);
                }
                GridControls.IsEnabled = false;
                btExcluir.IsEnabled = false;
                btFiltro_Click(sender, e);
                txtCodigo.Text = horaExtra.Id.ToString();
            }
            PreencheDataGrid();
        }

        private void dgHoraExtra_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            horaExtra = (HoraExtra)dgHoraExtra.SelectedItem;
            if (horaExtra != null)
            {
                GridControls.IsEnabled = true;
                btExcluir.IsEnabled = true;
                PreencheCampos();
            }
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            if (GridControls.IsEnabled != true)
            {
                LimpaCampos();
                GridControls.IsEnabled = true;
                horaExtra = new HoraExtra();
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (horaExtra != null && horaExtra.Id != 0)
            {
                MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + horaExtra.Nome + " ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
                {
                    Repositorio.Excluir(horaExtra);
                    LimpaCampos();
                    GridControls.IsEnabled = false;
                    btExcluir.IsEnabled = false;
                    PreencheDataGrid();
                }
            }
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            if (GridControls.IsEnabled)
            {
                GridControls.IsEnabled = false;
                btExcluir.IsEnabled = false;
                LimpaCampos();
            }
            else
            {
                (Parent as Grid).Children.Remove(this);
            }
        }

        private void btFiltro_Click(object sender, RoutedEventArgs e)
        {
            if (GridControls.IsEnabled)
            {
                MessageBox.Show("Termine o processo atual para realizar o filtro!", "Informacão", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var predicado = Repositorio.CriarPredicado();
            if (!String.IsNullOrEmpty(txtInicio.Text))
                predicado = predicado.And(x => x.DataHoraExtra >= txtInicio.SelectedDate);

            if (!String.IsNullOrEmpty(txtFinal.Text))
            {
                if (txtFinal.SelectedDate < txtInicio.SelectedDate)
                    MessageBox.Show("Data final é menor que a data inicial", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    predicado = predicado.And(x => x.DataHoraExtra <= txtFinal.SelectedDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59));
            }

            if (cmbFiltroPessoa.SelectedItem != null)
                predicado = predicado.And(x => x.Pessoa == cmbFiltroPessoa.SelectedItem);

            var filtro = Repositorio.ObterPorParametros(predicado).ToList();
            dgHoraExtra.ItemsSource = filtro.OrderBy(x => x.DataHoraExtra);

            if (filtro.Count > 0)
            {
                TimeSpan somaHoraFinalDia = TimeSpan.Zero;
                foreach (var item in filtro.Select(x => x.HoraFinalDia))
                {
                    somaHoraFinalDia = somaHoraFinalDia.Add(item);
                }
                lblTotalGeralHoras.Text = String.Format("Total Horas\r\n {0}", somaHoraFinalDia.ToString());
            }
        }

        private void txtInicio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                txtInicio.Text = string.Empty;
        }

        private void txtFinal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                txtFinal.Text = string.Empty;
        }

        private void cmbFiltroPessoa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbFiltroPessoa.SelectedIndex = -1;
        }
    }
}
