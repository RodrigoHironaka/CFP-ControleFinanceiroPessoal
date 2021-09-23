using Dominio.Interfaces;
using Dominio.ObejtoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class PessoaTipoRendas
    {
        public override string ToString()
        {
            return string.Format("{0} - Renda Bruta: {1} - Renda Liquida: {2}", TipoRenda.Nome, RendaBruta.ToString("N2"), RendaLiquida.ToString("N2"));
        }
        public virtual Int64 ID { get; set; }

        //criar um botao na frente de renda bruta onde apareca uma calculadora para calcular inss / desc sindicato(%),
        //add horaextra(precisa ser algo automatico, ao virar o mÊs ele puxa o total da tabela horaextra e calcula junto do salario 
        public virtual Decimal RendaBruta { get; set; }

        //Pode ser 0 se o campo estiver vazio
        public virtual Decimal RendaLiquida { get; set; }
        public virtual TipoRenda TipoRenda { get; set; }
        public virtual Pessoa Pessoa { get; set; }
    }
}
