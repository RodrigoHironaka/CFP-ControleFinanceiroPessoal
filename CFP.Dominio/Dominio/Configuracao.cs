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

        #region Sistema
        public virtual String CaminhoArquivos { get; set; }
        public virtual String CaminhoBackup { get; set; }
        #endregion

        #region Banco de Dados
        public virtual String ServidorBD { get; set; }
        public virtual String BaseBD { get; set; }
        public virtual String UsuarioBD { get; set; }
        public virtual String SenhaBD { get; set; }
        public virtual String PortaBD { get; set; }
        #endregion

    }
}
