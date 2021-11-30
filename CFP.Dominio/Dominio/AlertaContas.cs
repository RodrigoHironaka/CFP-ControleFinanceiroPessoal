using CFP.Dominio.ObjetoValor;
using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class AlertaContas
    {
        public TipoAlertaContas TipoAlertaContas { get; set; }
        public String Mensagem { get; set; }
        public Int64 CodigoConta { get; set; }
        public String Descricao { get; set; }
        public Int32 NumeroParcela { get; set; }
        public Decimal ValorParcela { get; set; }
        public DateTime VencimentoParcela { get; set; }

        public String DescricaoCompleta
        {
            get
            {
                return String.Format("{0}-{1}- Parcela {2}", CodigoConta, Descricao, NumeroParcela);
            }
        }
    }
}
