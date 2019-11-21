using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public static class Constant
    {
        public const double Mu_0 = 4 * Math.PI * 1e-7;       // 1.0 / (c * c * E_0) Гн/м     magnetic constant    магнитная постоянная
        public const double E_0 = 8.85418781761e-12; // 8.85e-12 Ф/м        electric constant     электрическая постоянная
        public const double Z_0 = 120 * Math.PI;                    // sqrt(Mu_0/E_0) = 120pi Ом    impedance of free space   волновое сопротивление свободного пространства
        public const double c = 299792458.00017589410202102027464;      //1 / Math.Sqrt(E_0 * Mu_0);       // м/с скорость света 
    }
}
