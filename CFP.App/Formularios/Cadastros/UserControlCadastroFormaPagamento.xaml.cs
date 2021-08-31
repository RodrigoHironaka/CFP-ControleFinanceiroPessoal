using CFP.App.Formularios.ModeloBase.UserControls;
using Dominio.Dominio;
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
    /// Interação lógica para UserControlCadastroFormaPagamento.xam
    /// </summary>
    public partial class UserControlCadastroFormaPagamento : UserControl
    {
        FormaPagamento formaPagamento;
        public UserControlCadastroFormaPagamento()
        {
            InitializeComponent();
        }
      

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
           if(panelDadosFormulario.IsEnabled)
            {
                panelDadosFormulario.IsEnabled = !panelDadosFormulario.IsEnabled;
                panelDadosFormulario.Visibility = Visibility.Hidden;
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
