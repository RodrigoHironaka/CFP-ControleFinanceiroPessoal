using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CFP.App.Formularios.Pesquisas
{
    /// <summary>
    /// Lógica interna para PesquisaCartoesCredito.xaml
    /// </summary>
    public partial class PesquisaCartoesCredito : Window
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
        private RepositorioCartaoCredito _repositorio;
        public RepositorioCartaoCredito Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioCartaoCredito(Session);
                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        #region Carrega Combos
        private void CarregaCombos()
        {
            cmbCartao.ItemsSource = new RepositorioFormaPagamento(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.UsadoParaCompras == SimNao.Sim)
                .OrderBy(x => x.Nome)
                .ToList<FormaPagamento>();

        }
        #endregion

        public CartaoCredito objeto;

        private void SelecionaeFecha()
        {
            if (DataGridPesquisa.Items.Count != 0)
            {
                objeto = DataGridPesquisa.SelectedItem as CartaoCredito;
                Close();
            }
        }

        private void CarregaDados()
        {
            var predicado = Repositorio.CriarPredicado();
            if (cmbCartao.SelectedIndex != -1)
                predicado = predicado.And(x => x.Cartao == cmbCartao.SelectedItem);

            if (chkAbertos.IsChecked == true)
                predicado = predicado.And(x => x.SituacaoFatura == SituacaoFatura.Aberta);

            DataGridPesquisa.ItemsSource = Repositorio.ObterPorParametros(predicado);
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
                cv.Filter = o =>
                {
                    CartaoCredito p = o as CartaoCredito;
                    return p.Id.ToString().Contains(txtPesquisa.Text)
                            || p.DescricaoCompleta.ToString().Contains(txtPesquisa.Text);
                };
            }
        }

        public PesquisaCartoesCredito()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombos();
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

        private void chkAbertos_Click(object sender, RoutedEventArgs e)
        {
            CarregaDados();
        }

        private void cmbCartao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregaDados();
        }
    }
}
