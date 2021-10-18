using CFP.App.Formularios.Financeiros;
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
    /// Interação lógica para UserControlFinanceiro.xam
    /// </summary>
    public partial class UserControlFinanceiro : UserControl
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

        public UserControlFinanceiro()
        {
            InitializeComponent();
        }

        private void BtConta_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlContas(new Conta(), Session));
        }

        private void btCaixa_Click(object sender, RoutedEventArgs e)
        {
            panelCadastros.Children.Clear();
            panelCadastros.Children.Add(new UserControlCaixa(new Caixa(), Session));
        }
    }
}
