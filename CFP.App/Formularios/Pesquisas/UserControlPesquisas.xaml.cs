using NHibernate;
using Repositorio.Repositorios;
using SGE.Repositorio.Configuracao;
using System.Linq;
using System.Windows.Controls;

namespace CFP.App.Formularios.Pesquisas
{
    /// <summary>
    /// Interação lógica para UserControlPesquisas.xam
    /// </summary>
    public partial class UserControlPesquisas : UserControl/* where T : class*/
    {
        //private bool SessionCompartilhada = true;

        //#region Session
        //private ISession session;
        //protected ISession Session
        //{
        //    get
        //    {
        //        if (session == null || !session.IsOpen)
        //        {
        //            if (session != null)
        //                session.Dispose();

        //            session = NHibernateHelper.GetSession();
        //            SessionCompartilhada = false;
        //        }
        //        return session;
        //    }
        //}
        //#endregion

        //#region Repositório
        //private RepositorioBase<T> _repositorio;
        //public RepositorioBase<T> Repositorio
        //{
        //    get
        //    {
        //        if (_repositorio == null)
        //            _repositorio = new RepositorioBase<T>(Session);
        //        return _repositorio;
        //    }
        //    set { _repositorio = value; }
        //}
        //#endregion

        //public T objeto;

        //private void CriaColunas()
        //{
        //    var tipo = typeof(T);
        //    var campos = tipo.GetProperties();
        //    foreach (var c in campos)
        //    {
        //        if (c.PropertyType.Name.Equals("IList'1"))
        //            continue;
        //        DataGridPesquisa.Columns.Add(new DataGridTextColumn { Header = c.Name });
        //    }
        //}

        //private void SelecionaEFecha()
        //{
        //    if (DataGridPesquisa.Items.Count != 0)
        //    {
        //        foreach (var linhas in DataGridPesquisa.SelectedItems)
        //        {
        //            objeto = (T)linhas;
        //        }
        //    }
        //}
        public UserControlPesquisas()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void btFechar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.Parent as StackPanel).Children.Remove(this);
        }
    }
}
