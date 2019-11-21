using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public class FarFieldRequestTemplate
    {
        public double ThetaStart { get; set; }
        public double ThetaFinish { get; set; }
        public double PhiStart { get; set; }
        public double PhiFinish { get; set; }
        public double Delta { get; set; }
        public int SystemOfCoordinates { get; set; }

        public string Title { get; set; }

        public FarFieldRequestTemplate(FarFieldRequestForm form)
        {
            ThetaStart = FarFieldRequestForm.ThetaStart;
            ThetaFinish = FarFieldRequestForm.ThetaFinish;
            PhiStart = FarFieldRequestForm.PhiStart;
            PhiFinish = FarFieldRequestForm.PhiFinish;
            Delta = FarFieldRequestForm.Delta;
            SystemOfCoordinates = FarFieldRequestForm.SystemOfCoordinates;
            Title = form.textBoxTitle.Text;
        }
    
    }
}
