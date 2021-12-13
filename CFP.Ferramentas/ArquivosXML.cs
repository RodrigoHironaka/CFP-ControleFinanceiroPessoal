using CFP.Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CFP.Ferramentas
{
    public class ArquivosXML
    {
        #region Exemplo Serializar
        //public static void GerarXML(List<DadosBD> dados)
        //{
        //    const string caminho = @"D:\Projetos\ControleFinanceiroPessoal\CFP.App\config.xml";
        //    try
        //    {
        //        XmlSerializer xsSubmit = new XmlSerializer(typeof(List<DadosBD>));
        //        var subReq = dados;
        //        using (var sww = new StringWriter())
        //        {
        //            using (var writer = XmlWriter.Create(sww))
        //            {
        //                xsSubmit.Serialize(writer, subReq);
        //                var xml = sww.ToString();
        //                XDocument doc = XDocument.Parse(xml);
        //                XElement root = doc.Root;
        //                root.Save(caminho);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //}



        //public void PreencheCampos()
        //{

        //    var configuracoes = Ferramentas.LerXML();
        //    if (configuracoes.Count > 0)
        //    {
        //        txtServidor.Text = configuracoes[0].ToString();
        //        txtPorta.Text = configuracoes[1].ToString();
        //        txtBanco.Text = configuracoes[2].ToString();
        //        txtUsuario.Text = configuracoes[3].ToString();
        //        txtSenha.Text = configuracoes[4].ToString();
        //        rdgServidorTerminal.SelectedIndex = Convert.ToInt32(configuracoes[5]);
        //        txtCaminhoBackupPadrao.Text = configuracoes[6].ToString();
        //        txtAlertaAtraso.Text = configuracoes[7].ToString();
        //        txtAtualizaEm.Text = configuracoes[8].ToString();
        //    }

        //}
        #endregion

        public static string StringConexao()
        {
            var dadosXml = ArquivosXML.Deserialize<DadosBD>(ArquivosXML.CaminhoArquivoXML());
            return "Server=" + dadosXml.ServidorBD + ";Port=" + dadosXml.PortaBD + ";Database=" + dadosXml.BaseBD + ";Uid=" + dadosXml.UsuarioBD + ";Pwd=" + dadosXml.SenhaBD + ";";
        }

        public static string CaminhoArquivoXML()
        {
            return String.Format(@"{0}\config.xml", Directory.GetCurrentDirectory());
        }

        #region Serializar e Deserializar para XML
        public static T Deserialize<T>(string input) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StreamReader sr = new StreamReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new
               XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
        #endregion
    }
}
