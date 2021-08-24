using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class HoraExtra
    {
        public virtual Int64 ID { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual TimeSpan HoraInicioManha { get; set; }
        public virtual TimeSpan HoraFinalManha { get; set; }
        public virtual TimeSpan TotalManha { get; set; }
        public virtual TimeSpan HoraInicioTarde { get; set; }
        public virtual TimeSpan HoraFinalTarde { get; set; }
        public virtual TimeSpan TotalTarde { get; set; }
        public virtual TimeSpan HoraFinalDia { get; set; }


    }
}
