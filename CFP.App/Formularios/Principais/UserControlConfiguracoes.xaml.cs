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

namespace CFP.App.Formularios.Principais
{
    /// <summary>
    /// Interação lógica para UserControlConfiguracoes.xam
    /// </summary>
    public partial class UserControlConfiguracoes : UserControl
    {
        public UserControlConfiguracoes()
        {
            InitializeComponent();
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
          //(this.Parent as StackPanel).Children.Remove(this);
          
        }
    }
}
