using CFP.App.Formularios.Cadastros;
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

namespace CFP.App.Formularios.ModeloBase.UserControls
{
    /// <summary>
    /// Interação lógica para UserControlCadastros.xam
    /// </summary>
    public partial class UserControlCadastros : UserControl
    {
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

        public UserControlCadastros()
        {
            InitializeComponent();
        }

        private void btFormaPagamento_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlCadastroFormaPagamento(new FormaPagamento(), Session));
        }

        private void btTipoGasto_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlTipoGasto(new GrupoGasto(), Session));
        }

        private void btTipoRenda_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlTipoRenda(new TipoRenda(), Session));
        }

        private void btUsuario_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlUsuario());
        }

        private void btPessoa_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlPessoa());
        }

        private void btBanco_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlBanco());
        }
    }
}
