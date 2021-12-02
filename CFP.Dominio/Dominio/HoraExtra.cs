using CFP.Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class HoraExtra : Base
    {
        public virtual Pessoa Pessoa { get; set; }
        public virtual DateTime DataHoraExtra { get; set; }
        public virtual TimeSpan HoraInicioManha { get; set; }
        public virtual TimeSpan HoraFinalManha { get; set; }
        public virtual TimeSpan HoraInicioTarde { get; set; }
        public virtual TimeSpan HoraFinalTarde { get; set; }
        public virtual TimeSpan TotalManha
        {
            get
            {
                if (HoraInicioManha != TimeSpan.Zero && HoraFinalManha != TimeSpan.Zero)
                    return (HoraFinalManha.Subtract(HoraInicioManha)) - TimeSpan.Parse("04:00:00");
                else
                    return TimeSpan.Zero;
            }
        }
        public virtual TimeSpan TotalTarde 
        {
            get
            {
                if (HoraInicioTarde!= TimeSpan.Zero && HoraFinalTarde != TimeSpan.Zero)
                    return (HoraFinalTarde.Subtract(HoraInicioTarde)) - TimeSpan.Parse("04:00:00");
                else
                    return TimeSpan.Zero;
            }
        }
        public virtual TimeSpan HoraFinalDia 
        {
            get
            {
                return TotalManha + TotalTarde;
            }
        }


    }
}
