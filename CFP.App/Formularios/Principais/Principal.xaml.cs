using CFP.App.Formularios.Financeiros.Consultas;
using CFP.App.Formularios.ModeloBase.UserControls;
using CFP.App.Formularios.Principais;
using CFP.Ferramentas;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using MySql.Data.MySqlClient;
using NHibernate;
using SGE.Repositorio.Configuracao;
using System;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CFP.App
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Usuario Logado
        public static Usuario UsuarioLogado;
        #endregion

        #region Session
        private static ISession session;
        protected static ISession Session
        {
            get
            {
                if (session == null || !session.IsOpen)
                {
                    if (session != null)
                        session.Dispose();
                    session = NHibernateHelper.GetSession();
                }
                return session;
            }
        }
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
                    //GridPrincipal.Children.Add(new UserControlInicio());
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
                    GridPrincipal.Children.Add(new ucConsultaContas(Session));
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
                lblNomeUsuario.Text = UsuarioLogado.Nome.ToUpper();
                Principal.Visibility = Visibility.Visible;
                login.Close();
            }
            else
            {
                Close();
            }
            #endregion

            //if(UsuarioLogado != null)
            //    GridPrincipal.Children.Add(new UserControlInicio());
        }

        private void ButtonConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            GridPrincipal.Children.Add(new UserControlConfiguracoes());
        }

        private void ButtonBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = System.DateTime.Now.ToShortDateString().Replace("/", "");
                string hora = System.DateTime.Now.ToLongTimeString().Replace(":", "");
                string caminhoPadrao = new RepositorioConfiguracao(Session).ObterTodos().FirstOrDefault().CaminhoBackup;
                string backupSalvar = caminhoPadrao + "\\CFP_" + data + "_" + hora + ".sql";

                if (caminhoPadrao != null)
                {
                    using (MySqlConnection conn = new MySqlConnection(ArquivosXML.StringConexao()))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup bk = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                Mouse.OverrideCursor = Cursors.Wait;
                                bk.ExportToFile(backupSalvar);
                                Mouse.OverrideCursor = null;
                                conn.Close();
                                MessageBox.Show("Backup Salvo em " + backupSalvar);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Defina o caminho padrão para realizar o backup em configurações!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult d = MessageBox.Show(" Deseja realmente Restaurar o Banco de Dados? ", " ATENÇÃO ", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (d == MessageBoxResult.Yes)
            {
                using (MySqlConnection conn = new MySqlConnection(ArquivosXML.StringConexao()))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup bk = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            Mouse.OverrideCursor = Cursors.Wait;
                            bk.ImportFromFile(@"D:\teste\backup\CFP_18112021_132833.sql");
                            Mouse.OverrideCursor = null;
                            conn.Close();
                            MessageBox.Show("Restauração bem Sucedida. O sistema será fechado.");
                            Application.Current.Shutdown();
                        }
                    }
                }

            }
        }
    }
}
