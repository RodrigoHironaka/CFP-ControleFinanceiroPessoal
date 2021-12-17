using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Ferramentas;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObjetoValor;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using NHibernate;
using Repositorio.Repositorios;
using SGE.Repositorio.Configuracao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CFP.App.Formularios.Principais
{
    /// <summary>
    /// Interação lógica para UserControlConfiguracoes.xam
    /// </summary>
    public partial class UserControlConfiguracoes : UserControl
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

        Configuracao configuracao = new Configuracao();

        #region Repositorio
        private RepositorioConfiguracao _repositorio;
        public RepositorioConfiguracao Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioConfiguracao(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                configuracao.CaminhoArquivos = txtCaminhoArquivos.Text;
                configuracao.CaminhoBackup = txtCaminhoBackup.Text;
                configuracao.FormaPagamentoPadraoConta = (FormaPagamento)cmbFormaPagamentoPadrão.SelectedItem;
                configuracao.TransacaoBancariaPadrao = (FormaPagamento)cmbTransacaoBancariaPadrao.SelectedItem;
                configuracao.DiasAlertaVencimento = txtQtdDiasAlertaVencimento.Text != string.Empty ? Int32.Parse(txtQtdDiasAlertaVencimento.Text) : 0;
                configuracao.UsuarioCriacao = MainWindow.UsuarioLogado;
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

        #region Preenche campos no user control
        private void PreencheCampos()
        {
            List<Configuracao> todasConfiguracoes = Repositorio.ObterTodos().Where(x => x.UsuarioCriacao.Id == MainWindow.UsuarioLogado.Id).ToList();
            foreach (var item in todasConfiguracoes)
                configuracao = Repositorio.ObterPorId(item.Id);

            if (configuracao != null)
            {
                txtCodigo.Text = configuracao.Id.ToString();
                txtCaminhoArquivos.Text = configuracao.CaminhoArquivos;
                txtCaminhoBackup.Text = configuracao.CaminhoBackup;
                cmbFormaPagamentoPadrão.SelectedItem = configuracao.FormaPagamentoPadraoConta;
                cmbTransacaoBancariaPadrao.SelectedItem = configuracao.TransacaoBancariaPadrao;
                txtQtdDiasAlertaVencimento.Text = configuracao.DiasAlertaVencimento.ToString() != string.Empty ? configuracao.DiasAlertaVencimento.ToString() : string.Empty;

            }
        }
        #endregion

        #region Escolher caminhos de pastas
        private string EscolherCaminho()
        {
            string retornando = string.Empty;
            using (var caminho = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult res = caminho.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                    retornando = caminho.SelectedPath.ToString();
            }
            return retornando;
        }
        #endregion

        #region DataAtualizacao
        private void UltimaAltualizacao()
        {
            if (configuracao != null && configuracao.DataAlteracao != DateTime.MinValue)
                lblAtualizadoEm.Text = string.Format("Atualizado em: {0}", configuracao.DataAlteracao);
            else
                lblAtualizadoEm.Text = configuracao != null && configuracao.DataGeracao != DateTime.MinValue ? string.Format("Atualizado em: {0}", configuracao.DataGeracao) : string.Empty;
        }
        #endregion

        #region Carrega Combo
        private void CarregaCombo()
        {
            cmbFormaPagamentoPadrão.ItemsSource = new RepositorioFormaPagamento(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.TransacoesBancarias == SimNao.Não)
                .OrderBy(x => x.Nome)
                .ToList();

            cmbTransacaoBancariaPadrao.ItemsSource = new RepositorioFormaPagamento(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.TransacoesBancarias == SimNao.Sim)
                .OrderBy(x => x.Nome)
                .ToList();
        }
        #endregion

        public UserControlConfiguracoes()
        {
            InitializeComponent();
            PreencheCampos();
            UltimaAltualizacao();
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);

        }
        private void btPesquisaCaminhoArquivos_Click(object sender, RoutedEventArgs e)
        {
            txtCaminhoArquivos.Text = EscolherCaminho();
        }

        private void btPesquisaCaminhoBackup_Click(object sender, RoutedEventArgs e)
        {
            txtCaminhoBackup.Text = EscolherCaminho();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtCaminhoArquivos.Text))
            {
                MessageBox.Show("Caminho dos arquivos é um campo obrigatório!");
                return;
            }

            if (PreencheObjeto())
            {
                if (configuracao != null && configuracao.Id == 0)
                {
                    configuracao.DataGeracao = DateTime.Now;
                    Repositorio.Salvar(configuracao);
                    txtCodigo.Text = configuracao.Id.ToString();
                    UltimaAltualizacao();
                }
                else
                {
                    configuracao.DataAlteracao = DateTime.Now;
                    Repositorio.Alterar(configuracao);
                    UltimaAltualizacao();
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombo();
        }

        private void btRestaurarBancoDados_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult d = MessageBox.Show(" Deseja realmente Restaurar o Banco de Dados? ", " ATENÇÃO ", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (d == MessageBoxResult.Yes)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "SQL (*.sql;)|*.sql;" };
                bool? response = openFileDialog.ShowDialog();
                if (response == true)
                {
                    using (MySqlConnection conn = new MySqlConnection(ArquivosXML.StringConexao()))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup bk = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                Mouse.OverrideCursor = Cursors.Wait;
                                bk.ImportFromFile(openFileDialog.FileName);
                                Mouse.OverrideCursor = null;
                                conn.Close();
                                MessageBox.Show("Restauração bem Sucedida. O sistema será fechado.");
                                Application.Current.Shutdown();
                            }
                        }
                    }
                }

            }
        }

        private void btConfigBancoDados_Click(object sender, RoutedEventArgs e)
        {
            ConfiguracaoBanco config = new ConfiguracaoBanco();
            config.ShowDialog();
            if (config.Sucesso == true)
                MessageBox.Show("Configuração Realizada com Sucesso, por favor abra novamente o sistema!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Houve algum erro na configuração! Configure corretamente pois pode haver erros ao conectar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        private void txtQtdDiasAlertaVencimento_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtQtdDiasAlertaVencimento_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
