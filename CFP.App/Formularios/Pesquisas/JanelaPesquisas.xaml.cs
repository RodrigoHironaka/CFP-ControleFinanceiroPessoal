using Dominio.Dominio;
using Dominio.ObjetoValor;
using LinqKit;
using NHibernate;
using Repositorio.Repositorios;
using SGE.Repositorio.Configuracao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtPesquisa.Text))
                return true;
            else
            {
                return ((item as FormaPagamento).Nome.IndexOf(txtPesquisa.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                
            }
                
        }
        private void CarregaDados()
        {
            DataGridPesquisa.ItemsSource = Repositorio.ObterTodos();
        }

        public JanelaPesquisas()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaDados();
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //recebe todos os dados do dataGrid
            ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridPesquisa.ItemsSource);
            //verifica se campo de pesquisa é esta vazio, se sim, filtro é nulo
            if (txtPesquisa.Text == string.Empty)
                cv.Filter = null;
            else
            {
                //senão filtro o objeto (neste caso 'FormaPagamento') por Nome e Id
                cv.Filter = o =>
                {
                    FormaPagamento p = o as FormaPagamento;
                    return p.Nome.Contains(txtPesquisa.Text)
                            || p.Id.ToString().Contains(txtPesquisa.Text);
                };
            }
            //TextBox t = (TextBox)sender;
            //string filter = t.Text;
            //ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridPesquisa.ItemsSource);
            //if (filter == "")
            //    cv.Filter = null;
            //else
            //{
            //    cv.Filter = o =>
            //    {
            //        FormaPagamento p = o as FormaPagamento;
            //        if (t.Name == "txtId")
            //            return (p.Id == Convert.ToInt32(filter));
            //        return (p.Nome.ToUpper().StartsWith(filter.ToUpper()));
            //    };
            //}
        }

        private void txtName_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            //var textBox = sender as TextBox;
            //e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtName1_TextChanged(object sender, TextChangedEventArgs e)
        { }

        private void txtPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            //recebe todos os dados do dataGrid
            ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridPesquisa.ItemsSource);
            //verifica se campo de pesquisa é esta vazio, se sim, filtro é nulo
            if (txtPesquisa.Text == string.Empty)
                cv.Filter = null;
            else
            {
                //senão filtro o objeto (neste caso 'FormaPagamento') por Nome e Id
                cv.Filter = o =>
                {
                    FormaPagamento p = o as FormaPagamento;
                    return p.Nome.Contains(txtPesquisa.Text)
                            || p.Id.ToString().Contains(txtPesquisa.Text);
                };
            }
        }
    }
}
