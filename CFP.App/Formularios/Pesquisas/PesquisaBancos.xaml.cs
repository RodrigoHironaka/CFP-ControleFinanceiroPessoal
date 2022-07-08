using CFP.Ferramentas;
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
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CFP.App.Formularios.Pesquisas
{
    /// <summary>
    /// Lógica interna para PesquisaBancos.xaml
    /// </summary>
    public partial class PesquisaBancos : Window
    {
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
        private RepositorioBanco _repositorio;
        public RepositorioBanco Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioBanco(Session);
                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        public Banco objeto;

        private void SelecionaeFecha()
        {
            if (DataGridPesquisa.Items.Count != 0)
            {
                objeto = DataGridPesquisa.SelectedItem as Banco;
                Close();
            }
        }

        private void CarregaDados()
        {
            DataGridPesquisa.ItemsSource = Repositorio.ObterTodos();
        }

        private void PesquisarDados()
        {
            //recebe todos os dados do dataGrid
            ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridPesquisa.ItemsSource);
            //verifica se campo de pesquisa é esta vazio, se sim, filtro é nulo
            if (txtPesquisa.Text == string.Empty)
                cv.Filter = null;
            else
            {
                //senão filtra o objeto (neste caso 'Banco') por Nome e Id
                String pesq = txtPesquisa.Text.ToLower();
                cv.Filter = o =>
                {
                    Banco p = o as Banco;
                    return Texto.RemoveAcento(p.Nome).Contains(pesq)
                            || p.Id.ToString().ToLower().Contains(pesq);
                };
            }
        }

        public PesquisaBancos()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaDados();
            txtPesquisa.Focus();
        }

        private void txtPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            PesquisarDados();
        }

        private void DataGridPesquisa_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelecionaeFecha();
        }
    }
}
