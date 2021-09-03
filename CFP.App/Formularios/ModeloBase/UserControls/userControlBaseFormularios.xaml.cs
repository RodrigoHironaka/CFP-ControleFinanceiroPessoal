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

namespace CFP.App.Formularios.ModeloBase.UserControls
{
    /// <summary>
    /// Interação lógica para userControlBaseFormularios.xam
    /// </summary>
    public partial class userControlBaseFormularios : UserControl
    {
        public userControlBaseFormularios()
        {
            InitializeComponent();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (GridCampos.IsEnabled)
            {
                GridCampos.IsEnabled = !GridCampos.IsEnabled;
                btOk.IsEnabled = !btOk.IsEnabled;
                btExcluir.IsEnabled = !btExcluir.IsEnabled;
                //Limpracontroles()
            }
            else
            {
                (this.Parent as StackPanel).Children.Remove(this);
            }
        }
    }
}
