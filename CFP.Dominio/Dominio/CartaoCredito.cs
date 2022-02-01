﻿using CFP.Dominio.ObjetoValor;
using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class CartaoCredito : Base
    {
     
        public virtual Int32 MesReferencia { get; set; }
        public virtual Int32 AnoReferencia { get; set; }
        public virtual Decimal ValorFatura { get; set; }
        public virtual SituacaoFatura SituacaoFatura { get; set; }
        public virtual FormaPagamento Cartao { get; set; }
        public virtual IList<CartaoCreditoItens> CartaoCreditos { get; set; }

        public virtual string DescricaoCompleta
        {
            get
            {
                return string.Format("{0} - {1:00}/{2}", Cartao, MesReferencia, AnoReferencia);
            }
        }
        public override string ToString()
        {
            return string.Format("{0} - {1:00}/{2}", Cartao, MesReferencia, AnoReferencia);
        }
    }
}
