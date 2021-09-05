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

namespace CFP.App.Formularios.Cadastros
{
    /// <summary>
    /// Interação lógica para UserControlPessoa.xam
    /// </summary>
    public partial class UserControlPessoa : UserControl
    {
        public UserControlPessoa()
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
