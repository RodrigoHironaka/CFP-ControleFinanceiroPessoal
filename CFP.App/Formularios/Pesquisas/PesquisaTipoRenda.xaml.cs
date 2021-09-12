﻿using Dominio.Dominio;
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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CFP.App.Formularios.Pesquisas
{
    /// <summary>
    /// Lógica interna para PesquisaTipoRenda.xaml
    /// </summary>
    public partial class PesquisaTipoRenda : Window
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
        private RepositorioTipoRenda _repositorio;
        public RepositorioTipoRenda Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioTipoRenda(Session);
                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        public TipoRenda objeto;

        private void SelecionaeFecha()
        {
            if (DataGridPesquisa.Items.Count != 0)
            {
                objeto = DataGridPesquisa.SelectedItem as TipoRenda;
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
                //senão filtra o objeto (neste caso 'FormaPagamento') por Nome e Id
                cv.Filter = o =>
                {
                    TipoRenda p = o as TipoRenda;
                    return p.Nome.Contains(txtPesquisa.Text)
                            || p.Id.ToString().Contains(txtPesquisa.Text);
                };
            }
        }

        public PesquisaTipoRenda()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaDados();
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