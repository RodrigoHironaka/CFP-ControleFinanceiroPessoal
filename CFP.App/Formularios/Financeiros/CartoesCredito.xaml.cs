using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
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
    }
}
