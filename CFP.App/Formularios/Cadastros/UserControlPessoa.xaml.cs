using Dominio.Dominio;
using Dominio.ObjetoValor;
using NHibernate;
using Repositorio.Repositorios;
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
        ISession Session;
        Pessoa pessoa;
        private void CarregaComboRenda()
        {
            cmbRenda.ItemsSource = new RepositorioTipoRenda(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
                .OrderBy(x => x.Nome)
                .ToList<TipoRenda>();
        }
        public UserControlPessoa(Pessoa _pessoa, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            pessoa = _pessoa;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (GridCampos.IsEnabled)
            {
                GridCampos.IsEnabled = !GridCampos.IsEnabled;
                btSalvar.IsEnabled = !btSalvar.IsEnabled;
                btExcluir.IsEnabled = !btExcluir.IsEnabled;
                //Limpracontroles()
            }
            else
            {
                (this.Parent as StackPanel).Children.Remove(this);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaComboRenda();
        }

        private void cmbRenda_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
           //.Children.Add(new UserControlPessoasValorRenda());
        }
    }
}
