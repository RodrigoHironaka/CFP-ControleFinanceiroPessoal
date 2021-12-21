using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
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
using System.Windows.Shapes;

namespace CFP.App.Formularios.Financeiros.TelasConfirmacoes
{
    /// <summary>
    /// Lógica interna para TransferenciaCofres.xaml
    /// </summary>
    public partial class TransferenciaCofres : Window
    {
        ISession Session;
        Cofre cofre = new Cofre();
        Configuracao config;

        #region Carrega combo
        private void CarregaCombo()
        {
            cmbBancoEntrada.ItemsSource = new RepositorioBanco(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
            cmbBancoEntrada.SelectedIndex = 0;
        }
        #endregion

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

        private RepositorioConfiguracao _repositorioConfiguracao;
        public RepositorioConfiguracao RepositorioConfiguracao
        {
            get
            {
                if (_repositorioConfiguracao == null)
                    _repositorioConfiguracao = new RepositorioConfiguracao(Session);

                return _repositorioConfiguracao;
            }
            set { _repositorioConfiguracao = value; }
        }
        #endregion

        #region Pegando as Configuracoes
        private void ConfiguracoesSistema()
        {
            Session.Clear();
            config = RepositorioConfiguracao.ObterTodos().Where(x => x.UsuarioCriacao.Id == MainWindow.UsuarioLogado.Id).FirstOrDefault();
            if (config == null || config.TransacaoBancariaPadrao == null)
            {
                MessageBox.Show("Por favor verifique suas configurações!\r\nPode ser que ela não esteja criada ou sua transação bancária padrão não esteja definida!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }
        #endregion

        public TransferenciaCofres(Cofre _cofre, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            cofre = _cofre;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfiguracoesSistema();
            CarregaCombo();
            txtBancoSaida.Text = cofre.Banco.ToString();
            txtValorSaida.Text = cofre.Valor.ToString();
            txtValorEntrada.Text = cofre.Valor.ToString();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBancoEntrada.SelectedIndex == -1)
            {
                MessageBox.Show("Defina o banco de entrada para realizar a transferência", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            #region Retirada do cofre
            Cofre cofreRetirada = new Cofre
            {
                Codigo = Repositorio.RetornaUltimoCodigo() + 1,
                Caixa = null,
                Banco = cofre.Banco,
                Valor = cofre.Valor * -1,
                TransacoesBancarias = config.TransacaoBancariaPadrao,
                Situacao = EntradaSaida.Saída,
                Nome = String.Format("Transferência saída para o banco {0}", cmbBancoEntrada.SelectedItem),
                DataGeracao = DateTime.Now,
                UsuarioCriacao = MainWindow.UsuarioLogado
            };
            Repositorio.Salvar(cofreRetirada);
            #endregion

            #region Entrada no cofre
            Cofre cofreEntrada = new Cofre
            {
                Codigo = Repositorio.RetornaUltimoCodigo() + 1,
                Caixa = null,
                Banco = (Banco)cmbBancoEntrada.SelectedItem,
                Valor = cofre.Valor,
                TransacoesBancarias = config.TransacaoBancariaPadrao,
                Situacao = EntradaSaida.Entrada,
                Nome = String.Format("Transferência entrada do banco {0}", cofre.Banco),
                DataGeracao = DateTime.Now,
                UsuarioCriacao = MainWindow.UsuarioLogado
            };
            Repositorio.Salvar(cofreEntrada);
            #endregion

            DialogResult = true;
        }
    }
}
