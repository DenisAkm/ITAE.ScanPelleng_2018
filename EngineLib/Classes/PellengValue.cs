using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public class PellengValue
    {
        PellengValueElement[] pellengElement;
        public PellengValueElement this[int index]
        {
            get
            {
                return pellengElement[index];
            }
            set
            {
                pellengElement[index] = value;
            }
        }
        public PellengValue(int count)
        {
            pellengElement = new PellengValueElement[count];
        }
        public int Count
        {
            get { return pellengElement.Length; }
        }
    }
    public class PellengValueElement
    {
        public double Theta { get; set; }
        public double Phi { get; set; }

        public double ThetaLocal { get; set; }
        public double PhiLocal { get; set; }
        public double Value { get; set; }

        public PellengValueElement(double value, double theta, double phi, double thetaLoc, double phiLoc)
        {
            Value = value;
            Theta = theta;
            Phi = phi;
            ThetaLocal = thetaLoc;
            PhiLocal = phiLoc;
        }
    }
}
