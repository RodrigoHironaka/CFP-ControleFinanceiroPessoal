using Dominio.ObjetoValor;
using LinqKit;
using NHibernate;
using Repositorio.Repositorios;
using SGE.Repositorio.Configuracao;
using System.Windows;
using System.Windows.Controls;

namespace CFP.App.Formularios.Pesquisas
{
    /// <summary>
    /// Lógica interna para JanelaPesquisas.xaml
    /// </summary>
    public partial class JanelaPesquisas : Window
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
        //        DataGridPesquisa.Columns.Add(new DataGridTextColumn { Header = c.Name});
        //    }
        //}

        //private void SelecionaEFecha()
        //{
        //    if(DataGridPesquisa.Items.Count != 0)
        //    {
        //        foreach (var linhas in DataGridPesquisa.SelectedItems)
        //        {
        //            objeto = (T)linhas;
        //        }
        //    }
        //}

        #region Session
        private ISession session;
        protected ISession Session
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

        #region Repositório
        private RepositorioFormaPagamento _repositorio;
        public RepositorioFormaPagamento Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioFormaPagamento(Session);
                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        private void CarregaDados()
        {
            var predicado = Repositorio.CriarPredicado();
            predicado = predicado.And(x => x.Situacao == Situacao.Ativo);
            DataGridPesquisa.ItemsSource = Repositorio.ObterPorParametros(predicado);
        }

        public JanelaPesquisas()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaDados();
        }
    }
}
