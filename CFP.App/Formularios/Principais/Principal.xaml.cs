using CFP.App.Formularios.Cadastros;
using CFP.App.Formularios.ModeloBase;
using CFP.App.Formularios.ModeloBase.UserControls;
using CFP.App.Formularios.Principais;
using CFP.Dominio.Dominio;
using Dominio.Dominio;
using NHibernate;
using SGE.Repositorio.Configuracao;
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

namespace CFP.App
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Usuario Logado
        public static string UsuarioLogado;
        public static string TipoUsuario;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonPopUpSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int indexOpcoesMenu = ListViewMenu.SelectedIndex;

            switch (indexOpcoesMenu)
            {
                case 0:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlInicio());
                    break;
                case 1:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlFinanceiro());
                    break;
                case 2:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlCadastros());
                    break;
                case 3:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlRelatorios());
                    break;
                default:
                    break;
            }
        }

        private void Principal_Loaded(object sender, RoutedEventArgs e)
        {

            #region Login
            Principal.Visibility = Visibility.Hidden;
            Login login = new Login();
            bool? confirmacao = login.ShowDialog();
            if ((bool)confirmacao)
            {
                UsuarioLogado = login.UsuarioLogado;
                lblNomeUsuario.Text = UsuarioLogado.ToUpper();
                Principal.Visibility = Visibility.Visible;
                login.Close();
            }
            else
            {
                Close();
            }
            #endregion

            if(UsuarioLogado != null)
                GridPrincipal.Children.Add(new UserControlInicio());
        }

        private void ButtonConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlConfiguracoes());
        }
    }
}
