using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class FluxoCaixa : Base
    {
        public override string ToString()
        {
            return Nome;
        }
        public virtual Caixa Caixa { get; set; }
        public virtual Usuario UsuarioLogado { get; set; }
    }
}
