using CFP.App.Formularios.Principais;
using CFP.Ferramentas;
using SGE.Repositorio.Configuracao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CFP.App
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                if (!File.Exists(ArquivosXML.CaminhoArquivoXML()))
                {
                    ConfiguracaoBanco config = new ConfiguracaoBanco();
                    config.ShowDialog();
                    if (config.Sucesso == true)
                        MessageBox.Show("Configuração Realizada com Sucesso, por favor abra novamente o sistema!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Houve algum erro na configuração! Configure corretamente pois pode haver erros ao conectar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var session = NHibernateHelper.GetSession(true);
                }
            }
            catch (Exception)
            {
                ConfiguracaoBanco config = new ConfiguracaoBanco();
                config.ShowDialog();
                if (config.Sucesso == true)
                    MessageBox.Show("Configuração Realizada com Sucesso, por favor abra novamente o sistema!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Houve algum erro na configuração! Configure corretamente pois pode haver erros ao conectar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
      
    }
}
