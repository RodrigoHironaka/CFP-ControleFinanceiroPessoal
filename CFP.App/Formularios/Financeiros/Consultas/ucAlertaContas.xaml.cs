using CFP.Dominio.Dominio;
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

namespace CFP.App.Formularios.Financeiros.Consultas
{
    /// <summary>
    /// Interação lógica para ucAlertaContas.xam
    /// </summary>
    public partial class ucAlertaContas : UserControl
    {
        ISession Session;

        private ObservableCollection<AlertaContas> alertas;

        public ucAlertaContas(ObservableCollection<AlertaContas> _alertas,ISession _session)
        {
            InitializeComponent();
            Session = _session;
            alertas = _alertas;
            dgAlertaContas.ItemsSource = alertas.OrderBy(x => x.VencimentoParcela).ToList();
        }

        private void ShowHideDetails(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Teste");
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Grid).Children.Remove(this);
        }
    }
}
