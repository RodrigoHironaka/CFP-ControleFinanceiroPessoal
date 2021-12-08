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

namespace CFP.App.Formularios.Financeiros
{
    /// <summary>
    /// Interação lógica para UserControlHoraExtra.xam
    /// </summary>
    public partial class UserControlHoraExtra : UserControl
    {
        ISession Session;
        HoraExtra horaExtra;

        #region Carrega Combos
        private void CarregaCombos()
        {
            cmbPessoa.ItemsSource = new RepositorioPessoa(Session)
           .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
           .OrderBy(x => x.Nome)
           .ToList();
            cmbPessoa.SelectedIndex = 0;
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

        //#region PreencheDataGrid
        //private ObservableCollection<HoraExtra> horaExtra;
        //public void PreencheDataGrid()
        //{
        //    horaExtra = new ObservableCollection<HoraExtra>();
        //    dgHoraExtra.ItemsSource = horaExtra;
        //}
        //#endregion
        #region Preenche Objeto para Salvar
        DateTime meioDia = Convert.ToDateTime("12:00:00");
        private bool PreencheObjeto()
        {
            try
            {
                horaExtra.Nome = txtDescricao.Text;
                horaExtra.Pessoa = (Pessoa)cmbPessoa.SelectedItem;
                horaExtra.DataHoraExtra = (DateTime)txtDataHoraExtra.SelectedDate;
                horaExtra.HoraInicioManha = txtHoraInicio.SelectedTime != null ? txtHoraInicio.SelectedTime.Value.TimeOfDay : TimeSpan.Zero;
                horaExtra.HoraFinalManha = txtHoraFinal.SelectedTime != null ? txtHoraFinal.SelectedTime.Value.TimeOfDay : TimeSpan.Zero;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
                //return false;
            }


        }
        #endregion

        public UserControlHoraExtra(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombos();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            horaExtra = new HoraExtra();
            if (PreencheObjeto())
            {
                dgHoraExtra.Items.Add(horaExtra);
            }
        }
    }
}
