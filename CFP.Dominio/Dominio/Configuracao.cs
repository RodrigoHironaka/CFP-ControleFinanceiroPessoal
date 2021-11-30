using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class Configuracao
    {
        public virtual Int64 Id { get; set; }
        public virtual DateTime DataGeracao { get; set; }
        public virtual DateTime DataAlteracao { get; set; }
        public virtual Usuario UsuarioCriacao { get; set; }

        #region Sistema
        public virtual String CaminhoArquivos { get; set; }
        public virtual String CaminhoBackup { get; set; }
        public virtual FormaPagamento FormaPagamentoPadraoConta { get; set; }
        public virtual Int32 DiasAlertaVencimento { get; set; }
        #endregion

    }
}
