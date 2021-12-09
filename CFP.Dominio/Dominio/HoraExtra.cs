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
        public virtual TimeSpan HoraInicioNoite { get; set; }
        public virtual TimeSpan HoraFinalNoite { get; set; }
        public virtual TimeSpan TotalManha
        {
            get
            {
                if (HoraInicioManha != TimeSpan.Zero && HoraFinalManha != TimeSpan.Zero)
                    return (HoraFinalManha.Subtract(HoraInicioManha));
                else
                    return TimeSpan.Zero;
            }
        }
        public virtual TimeSpan TotalTarde 
        {
            get
            {
                if (HoraInicioTarde!= TimeSpan.Zero && HoraFinalTarde != TimeSpan.Zero)
                    return (HoraFinalTarde.Subtract(HoraInicioTarde));
                else
                    return TimeSpan.Zero;
            }
        }

        public virtual TimeSpan TotalNoite
        {
            get
            {
                if (HoraInicioNoite != TimeSpan.Zero && HoraFinalNoite != TimeSpan.Zero)
                    return (HoraFinalNoite.Subtract(HoraInicioNoite));
                else
                    return TimeSpan.Zero;
            }
        }
        public virtual TimeSpan HoraFinalDia
        {
            get
            {
                return (TotalManha + TotalTarde + TotalNoite) - TimeSpan.Parse("08:00:00");

            }
        }
    }
}
