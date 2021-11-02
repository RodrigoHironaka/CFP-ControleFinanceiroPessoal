using CFP.Dominio.Dominio;
using CFP.Ferramentas;
using SGE.Repositorio.Configuracao;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Linq;

namespace CFP.App.Formularios.Principais
{
    /// <summary>
    /// Lógica interna para ConfiguracaoBanco.xaml
    /// </summary>
    public partial class ConfiguracaoBanco : Window
    {
        #region Preencher Campos
        public void PreencheCampos()
        {
            string caminho = @"D:\Projetos\ControleFinanceiroPessoal\CFP.App\config.xml";

            if (File.Exists(caminho))
            {
                DadosBD dadosBDXml = ArquivosXML.Deserialize<DadosBD>(caminho);
                txtBase.Text = dadosBDXml.BaseBD;
                txtPorta.Text = dadosBDXml.PortaBD;
                txtServidor.Text = dadosBDXml.ServidorBD;
                if (dadosBDXml.Servidor)
                    rgServidor.IsChecked = true;
                else
                    rgTerminal.IsChecked = true;
            }
            else
                MessageBox.Show("Arquivo de configuração do banco não existe!\nDigite as configurações a seguir.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
           
        }
        #endregion

        #region Verifica se todos os campos foram preenchidos
        public bool VerificaCampos()
        {
            foreach (var item in GridControls.Children)
            {
                if ((item is TextBox && (item as TextBox).Text == string.Empty) || (item is PasswordBox && (item as PasswordBox).Password == string.Empty))
                {
                    MessageBox.Show("Todos os campos são de preenchimento obrigatório!\nPor favor verifique novamente.");
                    return false;
                }
            }
            if(rgServidor.IsChecked == false && rgTerminal.IsChecked == false)
            {
                MessageBox.Show("Defina o tipo de máquina (Servidor ou terminal)!");
                return false;
            }
            return true;
        }
        #endregion

        public bool Sucesso = false;
        public bool Gravado = false;

        public ConfiguracaoBanco()
        {
            InitializeComponent();
            PreencheCampos();
        }

        private void btTestarConexao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VerificaCampos())
                {
                    DadosBD dados = new DadosBD
                    {
                        BaseBD = txtBase.Text,
                        PortaBD = txtPorta.Text,
                        SenhaBD = txtSenha.Password,
                        UsuarioBD = txtUsuario.Password,
                        ServidorBD = txtServidor.Text,
                        Servidor = rgServidor.IsChecked == true ? true : false

                    };

                    var d = ArquivosXML.Serialize<DadosBD>(dados);
                    XDocument doc = XDocument.Parse(d);
                    XElement root = doc.Root;
                    root.Save(@"D:\Projetos\ControleFinanceiroPessoal\CFP.App\config.xml");
                    Gravado = true;
                }
                var session = NHibernateHelper.GetSession(true);
                lblStatusConexao.Text = "Sucesso!";
                Sucesso = true;
            }
            catch (Exception)
            {
                lblStatusConexao.Text = "Falha!";
                Sucesso = false;
                Gravado = false;
            }
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
