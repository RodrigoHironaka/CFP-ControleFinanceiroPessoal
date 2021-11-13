using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
using Dominio.Dominio;
using Dominio.ObejtoValor;
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

namespace CFP.App.Formularios.Financeiros
{
    /// <summary>
    /// Interação lógica para UserControlCofre.xam
    /// </summary>
    public partial class UserControlCofre : UserControl
    {
        ISession Session;
        Cofre cofre;

        #region Repositorio
        private RepositorioCofre _repositorio;
        public RepositorioCofre Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioCofre(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        #region Preenche DataGrid
        private void PreencheDataGrid()
        {
            DataGridCofre.ItemsSource = Repositorio.ObterPorParametros(x => x.UsuarioCriacao == MainWindow.UsuarioLogado).ToList();
            //foreach (Cofre item in DataGridCofre.ItemsSource)
            //{
            //    if (item.Situacao == SituacaoCofre.Transferido)
                    
            //        Foreground = Brushes.Red;
            //}
        }
        #endregion

      
        public UserControlCofre( ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void btAdicionar_Click(object sender, RoutedEventArgs e)
        {
            ValoresCofre janela = new ValoresCofre(Session);
            janela.ShowDialog();
            PreencheDataGrid();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            var selecoes = DataGridCofre.SelectedItems;
            foreach (Cofre item in selecoes)
            {
                switch (item.Situacao)
                {
                    case SituacaoCofre.Cancelado:
                        MessageBox.Show("Você não pode Cancelar este registro!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case SituacaoCofre.Sacado:
                        MessageBox.Show("Você não pode Cancelar este registro!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case SituacaoCofre.RetiradoCaixa:
                        MessageBox.Show("Você não pode Cancelar este registro!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case SituacaoCofre.Transferido:
                    case SituacaoCofre.Depositado:
                    case SituacaoCofre.RecebimentoCaixa:
                        item.Situacao = SituacaoCofre.Cancelado;
                        item.DataAlteracao = DateTime.Now;
                        Repositorio.Alterar(item);
                        break;
                }
            }
           
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PreencheDataGrid();
        }

        private void DataGridCofre_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Cofre selecao = (Cofre)DataGridCofre.SelectedItem;
            cofre = Repositorio.ObterPorCodigo(selecao.Codigo);
            ValoresCofre janela = new ValoresCofre(cofre, Session);
            janela.ShowDialog();
            PreencheDataGrid();
        }

        private void DataGridCofre_LoadingRow(object sender, DataGridRowEventArgs e)
        {
          
        }
    }
}
