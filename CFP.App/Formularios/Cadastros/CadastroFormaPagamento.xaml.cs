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
using System.Windows.Shapes;

namespace CFP.App.Formularios.Cadastros
{
    /// <summary>
    /// Lógica interna para CadastroFormaPagamento.xaml
    /// </summary>
    public partial class CadastroFormaPagamento : Window
    {
        public CadastroFormaPagamento()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btOk.IsEnabled = !btOk.IsEnabled;
            btExcluir.IsEnabled = !btExcluir.IsEnabled;
            gridDadosFormulario.IsEnabled = !gridDadosFormulario.IsEnabled;
        }
    }
}
