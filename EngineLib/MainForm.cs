using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Numerics;


namespace Integral
{
    public partial class MainForm : Form
    {
 
        public double wire = 0;        // в мм
        DVector appertureI;
        DVector appertureM;
        public List<double> Frequencies = new List<double>();

        public List<WireMesh> WireGeometry = new List<WireMesh>();
        public string loadCurrentsAdress = "";
        public string saveCurrentsAdress = "";
        public Apperture apperture;
        double dimF = 1e9;
        public bool useCurrents = false;
        bool loading = false;
        int ThreadsNumber = 4;

        public List<FarFieldRequestTemplate> RequestsF = new List<FarFieldRequestTemplate>();
        

        public MainForm()
        {
            InitializeComponent();            
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderControl1.shutDown();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            renderControl1.init();

            DebugConfigure();
            ThinWireAprox.DegreeOfParallelism = ThreadsNumber;  
        }
        private void DebugConfigure()
        {
            loading = true;
            Frequencies.Add(6);
            treeViewConfiguration.Nodes[0].Text = "Частота [6 ГГц]";
             
            loading = false;

            
        }                       
        
        private void buttonGoButton_Click(object sender, EventArgs e)
        {   
            Stopwatch sw = Stopwatch.StartNew();
            DateTime date = DateTime.Now;
            textBoxInfo.AppendText("Начало расчёта в " + date + "." + Environment.NewLine);
            //ReSortWireGeometryIndex();

            if (treeViewConfiguration.Nodes[3].Nodes.Find("FarFieldRequest", false) != null)
            {
                FarFieldRequest();
            }

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            date = DateTime.Now;
            textBoxInfo.AppendText("Расчет завершён в " + date + ". ");
            textBoxInfo.AppendText("(Время расчёта " + elapsedTime + ".)" + Environment.NewLine);            
        }

        private void PellengRequest()
        {
            FarField scatteredFarFieldAzimut, scatteredFarFieldUglomest;
            FarField antennaFarFieldAzimut, antennaFarFieldUglomest;
            FarField transmittedFieldAzimut, transmittedFieldUglomest;            

            for (int i = 0; i < Frequencies.Count; i++)
            {
                double freq = Frequencies[i] * dimF;
                string name = "pelleng_diagram_" + freq / 1000000000 + "GHz";
                string directory = "C:/Users/Denis/Documents/Documents 2018/Расчет рассеянного поля по ИУ/Apperture";
                WireCurrent I;
                


                double scanPhiStart = 0;
                double scanPhiFinish = 0;
                double scanThetaStart = 0;
                double scanThetaFinish = 0;
                double scanDelta = 1;
                int scanCoordinatSystem = 0;

                if (CreateAppertureForm.Scanning)
                {
                    scanPhiStart = CreateAppertureForm.scanPhiStart;
                    scanPhiFinish = CreateAppertureForm.scanPhiFinish;
                    scanThetaStart = CreateAppertureForm.scanThetaStart;
                    scanThetaFinish = CreateAppertureForm.scanThetaFinish;
                    scanDelta = CreateAppertureForm.scanDelta;
                    scanCoordinatSystem = CreateAppertureForm.SysOfCoord;
                }

                int scanPoints_theta = Convert.ToInt32(Math.Abs(scanThetaFinish - scanThetaStart) / scanDelta) + 1;
                int scanPoints_phi = Convert.ToInt32(Math.Abs(scanPhiFinish - scanPhiStart) / scanDelta) + 1;
                PellengValue PellengAzimut = new PellengValue(scanPoints_phi * scanPoints_theta);
                PellengValue PellengUglomest = new PellengValue(scanPoints_phi * scanPoints_theta);
                int s = 0;
                for (int i_scanPhi = 0; i_scanPhi < scanPoints_phi; i_scanPhi++)
                {
                    double phi_out = scanPhiStart + i_scanPhi * scanDelta;
                    for (int i_scanTheta = 0; i_scanTheta < scanPoints_theta; i_scanTheta++)
                    {
                        //Создание аппертуры
                        double theta_out = scanThetaStart + i_scanTheta * scanDelta;

                        double theta_global = GetThetaGlobal(phi_out, theta_out, scanCoordinatSystem);
                        double phi_global = GetPhiGlobal(phi_out, theta_out, scanCoordinatSystem);
                        Apperture AppertureNew = new Apperture(apperture);
                        AppertureNew.ApplyScanning(freq, theta_global, phi_global);
                        
                        //
                        NearField nearField = new NearField(AppertureNew, WireGeometry, freq);
                        ThinWireAprox Calculation = new ThinWireAprox(WireGeometry, nearField, freq, wire, textBoxInfo);
                        I = Calculation.GetCurrent();


                        //Вычисление траектории сканирование по азимуту и по углу места

                        double thetaStartAzimut = 0; double thetaFinishAzimut = 0; double thetaStartUglomest = 0; double thetaFinishUglomest = 0;
                                                
                        double step = BearingErrorsRequestForm.Step;
                        int SysOfCoordAzimut = 0;
                        int SysOfCoordUglomest = 0;
                        double phiAzimut = 0;
                        double phiUglomest = 0;


                        if (AppertureNew.workPlane == "XY")
                        {
                            if (CreateAppertureForm.Axis == "Плоскость YZ")
                            {
                                SysOfCoordAzimut = 2;                                    
                                SysOfCoordUglomest = 1;
                            }
                            else if (CreateAppertureForm.Axis == "Плоскость XZ")
                            {
                                SysOfCoordAzimut = 1;
                                SysOfCoordUglomest = 2;
                            }
                        }
                        else if (AppertureNew.workPlane == "XZ")
                        {
                            if (CreateAppertureForm.Axis == "Плоскость XY")
                            {
                                SysOfCoordAzimut = 0;
                                SysOfCoordUglomest = 2;
                            }
                            else if (CreateAppertureForm.Axis == "Плоскость YZ")
                            {
                                SysOfCoordAzimut = 2;
                                SysOfCoordUglomest = 0;
                            }
                        }
                        else if (AppertureNew.workPlane == "YZ")
                        {
                            if (CreateAppertureForm.Axis == "Плоскость XY")
                            {
                                SysOfCoordAzimut = 0;
                                SysOfCoordUglomest = 1;
                            }
                            else if (CreateAppertureForm.Axis == "Плоскость XZ")
                            {
                                SysOfCoordAzimut = 1;
                                SysOfCoordUglomest = 0;
                            }
                        }

                        phiAzimut = GetPhiLocal(phi_global, theta_global, SysOfCoordAzimut);
                        phiUglomest = GetPhiLocal(phi_global, theta_global, SysOfCoordUglomest);



                        if (BearingErrorsRequestForm.ScanningDirection)
                        {
                            thetaStartAzimut = GetThetaLocal(phi_global, theta_global, SysOfCoordAzimut) - BearingErrorsRequestForm.AlthaStart;
                            thetaFinishAzimut = GetThetaLocal(phi_global, theta_global, SysOfCoordAzimut) + BearingErrorsRequestForm.AlthaStart;

                            thetaStartUglomest = GetThetaLocal(phi_global, theta_global, SysOfCoordUglomest) - BearingErrorsRequestForm.AlthaStart;
                            thetaFinishUglomest = GetThetaLocal(phi_global, theta_global, SysOfCoordUglomest) + BearingErrorsRequestForm.AlthaStart;
                        }
                        else
                        {
                            thetaStartAzimut = BearingErrorsRequestForm.AlthaStart;
                            thetaFinishAzimut = BearingErrorsRequestForm.AlthaFinish;

                            thetaStartUglomest = BearingErrorsRequestForm.AlthaStart;
                            thetaFinishUglomest = BearingErrorsRequestForm.AlthaFinish;
                        }


                        scatteredFarFieldAzimut = new FarField(I, freq, phiAzimut, phiAzimut, thetaStartAzimut, thetaFinishAzimut, step, SysOfCoordAzimut, true);
                        scatteredFarFieldUglomest = new FarField(I, freq, phiUglomest, phiUglomest, thetaStartUglomest, thetaFinishUglomest, step, SysOfCoordUglomest, true);

                        antennaFarFieldAzimut = new FarField(AppertureNew, freq, phiAzimut, phiAzimut, thetaStartAzimut, thetaFinishAzimut, step, SysOfCoordAzimut, true);
                        antennaFarFieldUglomest = new FarField(AppertureNew, freq, phiUglomest, phiUglomest, thetaStartUglomest, thetaFinishUglomest, step, SysOfCoordUglomest, true);

                        transmittedFieldAzimut = antennaFarFieldAzimut + scatteredFarFieldAzimut;
                        transmittedFieldUglomest = antennaFarFieldUglomest + scatteredFarFieldUglomest;

                        double pellengAzimut = DeterminePelleng(antennaFarFieldAzimut, transmittedFieldAzimut);
                        double pellengUglomest = DeterminePelleng(antennaFarFieldUglomest, transmittedFieldUglomest);

                        PellengAzimut[s] = new PellengValueElement(pellengAzimut, theta_global, phi_global, theta_out, phi_out);
                        PellengUglomest[s] = new PellengValueElement(pellengUglomest, theta_global, phi_global, theta_out, phi_out);
                        s++;
                        //string filename = Path.Combine(directory, name + "_T" + theta_global + "_P" + phi_global + ".txt");
                        //WriteAngularDiagram(filename, antennaFarFieldAzimut, scatteredFarFieldAzimut, transmittedFieldAzimut);
                    }
                }
                string filenameA = Path.Combine(directory, name + "A.txt");
                string filenameU = Path.Combine(directory, name + "U.txt");
                //string filename = Path.Combine("C:/Users/Denis/Documents/Documents 2018/Расчет рассеянного поля по ИУ/Apperture", name + ".txt");
                WritePellengDiagram(filenameA, PellengAzimut);
                WritePellengDiagram(filenameU, PellengUglomest);   
            }
        }

        private void FarFieldRequest()
        {
            FarField scatteredFarField, antennaFarField, transmittedField;

            for (int i = 0; i < Frequencies.Count; i++)
            {
                double freq = Frequencies[i] * dimF;
                WireCurrent I;

                double scanPhiStart = 0;
                double scanPhiFinish = 0;
                double scanThetaStart = 0;
                double scanThetaFinish = 0;
                double scanDelta = 1;
                int scanCoordinatSystem = 0;

                if (CreateAppertureForm.Scanning)
                {
                    scanPhiStart = CreateAppertureForm.scanPhiStart;
                    scanPhiFinish = CreateAppertureForm.scanPhiFinish;
                    scanThetaStart = CreateAppertureForm.scanThetaStart;
                    scanThetaFinish = CreateAppertureForm.scanThetaFinish;
                    scanDelta = CreateAppertureForm.scanDelta;
                    scanCoordinatSystem = CreateAppertureForm.SysOfCoord;
                }

                int scanPoints_theta = Convert.ToInt32(Math.Abs(scanThetaFinish - scanThetaStart) / scanDelta) + 1;
                int scanPoints_phi = Convert.ToInt32(Math.Abs(scanPhiFinish - scanPhiStart) / scanDelta) + 1;

                for (int i_scanPhi = 0; i_scanPhi < scanPoints_phi; i_scanPhi++)
                {
                    double phi_out = scanPhiStart + i_scanPhi * scanDelta;
                    for (int i_scanTheta = 0; i_scanTheta < scanPoints_theta; i_scanTheta++)
                    {
                        double theta_out = scanThetaStart + i_scanTheta * scanDelta;

                        double theta_global = GetThetaGlobal(phi_out, theta_out, scanCoordinatSystem);
                        double phi_global = GetPhiGlobal(phi_out, theta_out, scanCoordinatSystem);
                        Apperture AppertureNew = new Apperture(apperture);
                        AppertureNew.ApplyScanning(freq, theta_global, phi_global);
                        NearField nearField = new NearField(AppertureNew, WireGeometry, freq, ThreadsNumber, textBoxInfo);
                        ThinWireAprox Calculation = new ThinWireAprox(WireGeometry, nearField, freq, wire, textBoxInfo);
                        I = Calculation.GetCurrent();

                        textBoxInfo.AppendText("Расчет поля в дальней зоне" + Environment.NewLine);
                        for (int f = 0; f < RequestsF.Count; f++)
                        {
                            double phiStart = RequestsF[f].PhiStart;
                            double phiFinish = RequestsF[f].PhiFinish;
                            double thetaStart = RequestsF[f].ThetaStart;
                            double thetaFinish = RequestsF[f].ThetaFinish;
                            double step = RequestsF[f].Delta;
                            int systemfarfield = RequestsF[f].SystemOfCoordinates;

                            scatteredFarField = new FarField(4, I, freq, phiStart, phiFinish, thetaStart, thetaFinish, step, systemfarfield, true);                            
                            antennaFarField = new FarField(4, AppertureNew, freq, phiStart, phiFinish, thetaStart, thetaFinish, step, systemfarfield, true);
                            transmittedField = antennaFarField + scatteredFarField;

                            string name = RequestsF[f].Title + "_" + freq / 1000000000 + "GHz" + "_T" + theta_global + "_P" + phi_global + "_parallel.txt";
                            string filename = Path.Combine("C:/Users/Denis/Documents/Documents 2018/Расчет рассеянного поля по ИУ/Apperture", name);
                            WriteAngularDiagramSimple(filename, antennaFarField, transmittedField, scatteredFarField);
                        }
                    }
                }
            }
        }
               
        public static double GetPhiGlobal(double phi, double theta, int system)
        {
            double a = 0; double b = 0; double c = 0;
            
            if (system == 0)
            {
                a = Math.Sin(theta * Math.PI / 180) * Math.Cos(phi * Math.PI / 180);
                b = Math.Sin(theta * Math.PI / 180) * Math.Sin(phi * Math.PI / 180);
                c = Math.Cos(theta * Math.PI / 180);                
            }
            else if (system == 1)
            {
                c = Math.Sin(theta * Math.PI / 180) * Math.Cos(phi * Math.PI / 180);
                a = Math.Sin(theta * Math.PI / 180) * Math.Sin(phi * Math.PI / 180);
                b = Math.Cos(theta * Math.PI / 180);                
            }
            else if (system == 2)
            {
                b = Math.Sin(theta * Math.PI / 180) * Math.Cos(phi * Math.PI / 180);
                c = Math.Sin(theta * Math.PI / 180) * Math.Sin(phi * Math.PI / 180);
                a = Math.Cos(theta * Math.PI / 180);
            }
            double m = c / Math.Sqrt(a * a + b * b + c * c);
            if (m < -1)
            {
                m = -1;
            }
            else if(m > 1)
            {
                m = 1;
            }

            double thetaGlobal = Math.Acos(m) / Math.PI * 180;
            if (thetaGlobal == 0)
            {
                return 0;
            }
            else
            {
                double v = a / Math.Sin(thetaGlobal * Math.PI / 180);
                if (v > 1)
                {
                    v = 1;
                }
                else if (v < -1)
                {
                    v = -1;
                }

                double phiGlobal = Math.Acos(v) / Math.PI * 180;
                //if (theta > 90 && theta < 270)
                //{
                //    return 360 - phiGlobal;
                //}
                //else
                //{
                //    return phiGlobal;
                //}

                if (b < 0)
                {                    
                    return 360 - phiGlobal;
                }
                else
                {
                    return phiGlobal;
                }                
            }
            
        }

        public static double GetThetaGlobal(double phi, double theta, int system)
        {
            double a = 0; double b = 0; double c = 0;

            if (system == 0)
            {
                a = Math.Sin(theta * Math.PI / 180) * Math.Cos(phi * Math.PI / 180);
                b = Math.Sin(theta * Math.PI / 180) * Math.Sin(phi * Math.PI / 180);
                c = Math.Cos(theta * Math.PI / 180);
            }
            else if (system == 1)
            {
                c = Math.Sin(theta * Math.PI / 180) * Math.Cos(phi * Math.PI / 180);
                a = Math.Sin(theta * Math.PI / 180) * Math.Sin(phi * Math.PI / 180);
                b = Math.Cos(theta * Math.PI / 180);
            }
            else if (system == 2)
            {
                b = Math.Sin(theta * Math.PI / 180) * Math.Cos(phi * Math.PI / 180);
                c = Math.Sin(theta * Math.PI / 180) * Math.Sin(phi * Math.PI / 180);
                a = Math.Cos(theta * Math.PI / 180);
            }

            double m = c / Math.Sqrt(a * a + b * b + c * c);
            if (m < -1)
            {
                m = -1;
            }
            else if (m > 1)
            {
                m = 1;
            }

            return Math.Acos(m) / Math.PI * 180;
        }

        public static double GetPhiLocal(double phi, double theta, int system)
        {
            double a = Math.Sin(theta * Math.PI / 180) * Math.Cos(phi * Math.PI / 180);
            double b = Math.Sin(theta * Math.PI / 180) * Math.Sin(phi * Math.PI / 180);
            double c = Math.Cos(theta * Math.PI / 180);

            if (system == 0)
            {
                double v = c / Math.Sqrt(a * a + b * b + c * c);
                if (v < -1)
                {
                    v = -1;
                }
                else if (v > 1)
                {
                    v = 1;
                }
                double thetaLocal = Math.Acos(v) / Math.PI * 180;
                if (thetaLocal == 0)
                {
                    return 0;
                }
                else
                {
                    double m = a / Math.Sin(thetaLocal * Math.PI / 180);
                    if (m < -1)
                    {
                        m = -1;
                    }
                    else if (m > 1)
                    {
                        m = 1;
                    }
                    if (b < 0)
                    {
                        return -Math.Acos(m) / Math.PI * 180;    
                    }
                    else
                    {
                        return Math.Acos(m) / Math.PI * 180;
                    }                    
                }
            }
            else if (system == 1)
            {
                double v = b / Math.Sqrt(a * a + b * b + c * c);
                if (v < -1)
                {
                    v = -1;
                }
                else if (v > 1)
                {
                    v = 1;
                }
                double thetaLocal =  Math.Acos(v) / Math.PI * 180;
                if (thetaLocal == 0)
                {
                    return 0;
                }
                else
                {
                    double m = c / Math.Sin(thetaLocal * Math.PI / 180);
                    if (m < -1)
                    {
                        m = -1;
                    }
                    else if (m > 1)
                    {
                        m = 1;
                    }
                    if (a < 0)
                    {
                        return -Math.Acos(m) / Math.PI * 180;
                    }
                    else
                    {
                        return Math.Acos(m) / Math.PI * 180;
                    }
                }
            }
            else if (system == 2)
            {
                double v = a / Math.Sqrt(a * a + b * b + c * c);
                if (v < -1)
                {
                    v = -1;
                }
                else if (v > 1)
                {
                    v = 1;
                }
                double thetaLocal =  Math.Acos(v) / Math.PI * 180;
                if (thetaLocal == 0)
                {
                    return 0;
                }
                else
                {
                    double m = b / Math.Sin(thetaLocal * Math.PI / 180);
                    if (m < -1)
                    {
                        m = -1;
                    }
                    else if (m > 1)
                    {
                        m = 1;
                    }
                    if (c < 0)
                    {
                        return -Math.Acos(m) / Math.PI * 180;    
                    }
                    else
                    {
                        return Math.Acos(m) / Math.PI * 180;
                    }                    
                }
            }
            else
            {
                return 0;
            }
        }

        public static double GetThetaLocal(double phi, double theta, int system)
        {
            double a = Math.Sin(theta * Math.PI / 180) * Math.Cos(phi * Math.PI / 180);
            double b = Math.Sin(theta * Math.PI / 180) * Math.Sin(phi * Math.PI / 180);
            double c = Math.Cos(theta * Math.PI / 180);

            if (system == 0)
            {
                double v = c / Math.Sqrt(a * a + b * b + c * c);
                if (v > 1)
                {
                    v = 1;
                }
                else if (v < -1)
                {
                    v = -1;
                }
                return Math.Acos(v) / Math.PI * 180;
            }
            else if (system == 1)
            {
                double v = b / Math.Sqrt(a * a + b * b + c * c);
                if (v > 1)
                {
                    v = 1;
                }
                else if (v < -1)
                {
                    v = -1;
                }
                return Math.Acos(v) / Math.PI * 180;
            }
            else if (system == 2)
            {
                double v = a / Math.Sqrt(a * a + b * b + c * c);
                if (v > 1)
                {
                    v = 1;
                }
                else if (v < -1)
                {
                    v = -1;
                }
                return Math.Acos(v) / Math.PI * 180;
            }            
            else
            {
                return 0;
            }
        }
        private double DeterminePelleng(FarField antennaFarField, FarField transmittedField)
        {            
            try
            {
                double min_antenna = FindMinimum(antennaFarField);
                double min_transmit = FindMinimum(transmittedField);
                return min_antenna - min_transmit;
            }
            catch (Exception)
            {
                return Double.NaN;                
            }           
            
        }

        private double FindMinimum(FarField farField)
        {
            int min_index = 0;
            double e_min = farField[0].Etotal;
            for (int i = 1; i < farField.Count; i++)
            {
                double value = farField[i].Etotal;
                if (e_min > value)
                {
                    e_min = value;
                    min_index = i;
                }                
            }

            double min_angle = 0;
            double delta = 0;

            min_angle = farField[min_index].Theta;
            delta = farField[min_index + 1].Theta - min_angle;
            

            double e_1 = farField[min_index - 1].Etotal;
            double e_2 = farField[min_index - 1].Etotal;

            return min_angle - delta / 2 * (1 - (e_1 - e_min) / (e_2 - e_min));
        }
        
        private void WriteAngularDiagram(string fileName, FarField ff1, FarField ff2, FarField ff3)
        {
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine("Theta" + "\t" + "Phi" + "\t" + "Etotal, dBV" + "\t" + "Ephi, dBV" + "\t" + "Etheta, dBV" + "\t" + "Etotal, dBV" + "\t" + "Ephi, dBV" + "\t" + "Etheta, dBV" + "\t" + "Etotal, dBV" + "\t" + "Ephi, dBV" + "\t" + "Etheta, dBV");
            for (int i = 0; i < ff1.Count; i++)
            {
                sw.WriteLine(((ff1[i].LocalTheta + "\t" + ff1[i].LocalPhi + "\t" + ff1[i].Etotal + "\t" + ff1[i].Ephi + "\t" + ff1[i].Etheta + "\t" + ff2[i].Etotal + "\t" + ff2[i].Ephi + "\t" + ff2[i].Etheta + "\t" + ff3[i].Etotal + "\t" + ff3[i].Ephi + "\t" + ff3[i].Etheta).Replace(",", ".")));
            }
            sw.Close();
        }
        private void WriteAngularDiagramSimple(string fileName, FarField ff1, FarField ff2, FarField ff3)
        {
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine("Theta" + "\t" + "Etotal, dBV" + "\t" + "Etotal, dBV" + "\t" + "Etotal, dBV");
            for (int i = 0; i < ff1.Count; i++)
            {
                sw.WriteLine(((ff1[i].LocalTheta + "\t" + ff1[i].Etotal + "\t" + ff2[i].Etotal + "\t" + ff3[i].Etotal).Replace(",", ".")));
            }
            sw.Close();
        }     
        private void WritePellengDiagram(string filename, PellengValue pelleng)
        {
            StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine("ThetaGlobal" + "\t" + "PhiGlobal" + "\t" + "ThetaLocal" + "\t" + "PhiLocal" + "\t");
            for (int i = 0; i < pelleng.Count; i++)
            {
                sw.WriteLine(((pelleng[i].Theta + "\t" + pelleng[i].Phi + "\t" + pelleng[i].ThetaLocal + "\t" + pelleng[i].PhiLocal + "\t" + pelleng[i].Value).Replace(",", ".")));
            }
            sw.Close();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetParamForm()
        {
            //wire = 0.001;

            //for (int i = 0; i < Frequencies.Count; i++)
            //{
            //    Frequencies.Add(Convert.ToDouble(Frequencies.Items[i].ToString().Replace(".", ",")) * 1e9);    
            //}
            
            //theta = Convert.ToDouble(textBoxPlaneWaveTheta.Text.Replace(".", ","));
            //phi = Convert.ToDouble(textBoxPlaneWavePhi.Text.Replace(".", ","));
            //polar = Convert.ToDouble(textBoxPolariz.Text.Replace(".", ","));
            //thetaStart = Convert.ToDouble(textBoxThetaStart.Text.Replace(".", ","));
            //thetaFinish = Convert.ToDouble(textBoxThetaFinish.Text.Replace(".", ","));    
            //phiStart = Convert.ToDouble(textBoxPhiStart.Text.Replace(".", ","));
            //phiFinish = Convert.ToDouble(textBoxPhiFinish.Text.Replace(".", ","));
            //step = Convert.ToDouble(textBoxStep.Text.Replace(".", ","));
            

        }

        private void ReSortWireGeometryIndex()
        {
            int index = 0;
            for (int i = 0; i < WireGeometry.Count; i++)
            {
                for (int j = 0; j < WireGeometry[i].CountLines; j++)
                {
                    WireGeometry[i].Lines[j].Index = index;
                    index++;
                }
            }
        }

                
        // treeView
        private void treeViewConfiguration_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewConfiguration.SelectedNode != null)
            {
                if (treeViewConfiguration.SelectedNode.Name == "Frequency")
                {
                    OpenFrequencyForm();
                }
                else if (treeViewConfiguration.SelectedNode.Name == "Source")
                {
                    CreateAppertureForm appertureForm = new CreateAppertureForm(this);
                }
                else if (treeViewConfiguration.SelectedNode.Name == "Object")
                {
                    OpenWireGeometry();
                }
                else if (treeViewConfiguration.SelectedNode.Name == "Request")
                {
                    OpenBearingErrorsRequest();
                }
                else if (treeViewConfiguration.SelectedNode.Name == "CreateApperture")
                {
                    CreateAppertureForm form = new CreateAppertureForm(this);
                }                
                //else if (treeViewConfiguration.SelectedNode.Name == "BearingErrors")
                //{
                //    OpenBearingErrorsRequest();
                //}               
                else if (RequestsF.Find(t => t.Title == treeViewConfiguration.SelectedNode.Name) != null)
                {
                    FarFieldRequestTemplate template = RequestsF.Find(t => t.Title == treeViewConfiguration.SelectedNode.Name);
                    var f = new FarFieldRequestForm(this, template);
                }
            }
        }
        private void toolStripFrequency_Click(object sender, EventArgs e)
        {
            OpenFrequencyForm();
        }
        private void toolStripCreateAppertureForm_Click(object sender, EventArgs e)
        {
            CreateAppertureForm appertureForm = new CreateAppertureForm(this);
        }
        private void toolStripMenuItemDeleteWireObject_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem toolstrip = (ToolStripMenuItem)sender;
            DeleteWireGeometry(toolstrip.Tag.ToString());
        }
        private void toolStripMenuItemFarFieldRequest_Click(object sender, EventArgs e)
        {
            OpenFarFieldRequest();
        }
        private void toolStripMenuItemFarFieldRequestDelete_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem toolstrip = (ToolStripMenuItem)sender;
            FarFieldRequestTemplate template = RequestsF.Find(t => t.Title == toolstrip.Tag.ToString());
            RequestsF.Remove(template);
            renderControl1.Remove(template);
            DeleteFarFieldTreeView(template);
        }
        private void toolStripMenuItemFarFieldRequestChange_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem toolstrip = (ToolStripMenuItem)sender;
            FarFieldRequestTemplate template = RequestsF.Find(t => t.Title == toolstrip.Tag.ToString());
            var f = new FarFieldRequestForm(this, template);
        }                
        private void проволочнаяГеометрияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWireGeometry();
        }
        private void ErrorsRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenBearingErrorsRequest();
        }


        public void AddFarFieldTreeView(FarFieldRequestTemplate fftemplate)
        {
            TreeNode newNode = new TreeNode(fftemplate.Title);
            newNode.Name = fftemplate.Title;
            newNode.Tag = "FarFieldRequest";
            ContextMenuStrip localContext = ContectMenuFarFieldRequest2(fftemplate);
            newNode.ContextMenuStrip = localContext;            
            treeViewConfiguration.Nodes[3].Nodes.Add(newNode);       
            
        }


        public void AddObjectTreeView(string title)
        {
            TreeNode newNode = new TreeNode(title);
            newNode.Name = title;
            ContextMenuStrip localContext = new System.Windows.Forms.ContextMenuStrip();

            localContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] 
            {
                new ToolStripMenuItem ()
                {
                    Name = "toolStripMenuItemDelete",
                    Size = new System.Drawing.Size(152, 22),
                    Text = "Удалить",
                    Tag = title
                },
                
            });
            localContext.Items[0].Click += new System.EventHandler(this.toolStripMenuItemDeleteWireObject_Click);            

            localContext.Name = title;
            localContext.Size = new System.Drawing.Size(153, 70);
            newNode.ContextMenuStrip = localContext;
            treeViewConfiguration.Nodes[2].Nodes.Add(newNode);
        }
                
        public void DeleteFarFieldTreeView(FarFieldRequestTemplate fftemplate)
        {            
            treeViewConfiguration.Nodes[3].Nodes.RemoveByKey(fftemplate.Title);
        }
        public void ChangeFarFieldTreeViewName(string formerName, string newName)
        {
            for (int i = 0; i < treeViewConfiguration.Nodes[3].Nodes.Count; i++)
            {
                if (treeViewConfiguration.Nodes[3].Nodes[i].Name == formerName)
                {
                    treeViewConfiguration.Nodes[3].Nodes[i].Text = newName;
                    treeViewConfiguration.Nodes[3].Nodes[i].Name = newName;
                    treeViewConfiguration.Nodes[3].Nodes[i].ContextMenuStrip.Items[0].Tag = newName;
                }
            }
        }
        public void DeleteWireObjectTreeViewNode(string name)
        {            
            treeViewConfiguration.Nodes[2].Nodes.RemoveByKey(name);        
        }

        public ContextMenuStrip ContectMenuFarFieldRequest2(FarFieldRequestTemplate fftemplate)
        {
            ContextMenuStrip localContext = new System.Windows.Forms.ContextMenuStrip();
            localContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] 
            {
                new ToolStripMenuItem ()
                {
                    Name = "toolStripMenuItemChange",
                    Size = new System.Drawing.Size(152, 22),
                    Text = "Изменить",
                    Tag = fftemplate.Title
                },
                new ToolStripMenuItem ()
                {
                    Name = "toolStripMenuItemDelete",
                    Size = new System.Drawing.Size(152, 22),
                    Text = "Удалить",
                    Tag = fftemplate.Title
                }, 
            });
            localContext.Items[0].Click += new System.EventHandler(this.toolStripMenuItemFarFieldRequestChange_Click);
            localContext.Items[1].Click += new System.EventHandler(this.toolStripMenuItemFarFieldRequestDelete_Click);

            localContext.Name = fftemplate.Title;
            localContext.Size = new System.Drawing.Size(153, 70);
            return localContext;
        }
        
        private void OpenFarFieldRequest()
        {
            FarFieldRequestForm ffrequestForm = new FarFieldRequestForm(this);
        }        
        private void OpenFrequencyForm()
        {
            SetFrequencyForm frequencyForm = new SetFrequencyForm(this);
        }
        private void OpenWireGeometry()
        {
            CreateObjectForm form = new CreateObjectForm(this);
        }
        private void DeleteWireGeometry(string name)
        {
            DeleteWireObjectTreeViewNode(name);
            WireMesh wiremesh = WireGeometry.Find(g => g.Name == name);
            WireGeometry.Remove(wiremesh);
            renderControl1.Remove(wiremesh);            
        }
        private void OpenBearingErrorsRequest()
        {
            BearingErrorsRequestForm errorsForm = new BearingErrorsRequestForm(this);
        }


        //unused methods
        private void WriteAngularDiagram(FarField diag)
        {
            StreamWriter sw = new StreamWriter(Path.Combine("C:/Users/Denis/Documents/Documents 2018/Расчет рассеянного поля по ИУ/Feko", "ang_diagram.txt"));
            sw.WriteLine("Theta" + "\t" + "Phi" + "\t" + "Etotal, dBV" + "\t" + "Ephi, dBV" + "\t" + "Etheta, dBV");
            for (int i = 0; i < diag.Count; i++)
            {
                sw.WriteLine(((diag[i].Theta + "\t" + diag[i].Phi + "\t" + diag[i].Etotal + "\t" + diag[i].Ephi + "\t" + diag[i].Etheta).Replace(",", ".")));
            }
            sw.Close();
        }

        private void WriteFreqDiagram(FarField diag)
        {
            StreamWriter sw = new StreamWriter(Path.Combine("C:/Users/Denis/Documents/Документы 2018/Расчет рассеянного поля по ИУ/Feko", "freq_diagram.txt"));
            for (int i = 0; i < diag.Count; i++)
            {
                sw.WriteLine(((diag[i].Frequency + "\t" + diag[i].Etotal + "\t" + diag[i].Ephi + "\t" + diag[i].Etheta).Replace(",", ".")));
            }
            sw.Close();
        }
        private void WriteCurrent(WireCurrent I, WireMesh geo)
        {
            StreamWriter sw = new StreamWriter(Path.Combine("C:/Users/Denis/Documents/Документы 2018/Расчет рассеянного поля по ИУ/Feko", "current.txt"));
            sw.WriteLine("Position" + "\t" + "Magnitude, mA" + "\t" + "Phase, grad" + "\t" + "Real" + "\t" + "Imaginary");
            for (int i = 0; i < I.Count; i++)
            {
                sw.WriteLine((I[i].Position.X.ToString() + "\t" + I[i].Magnitude + "\t" + I[i].Phase + "\t" + I[i].Real + "\t" + I[i].Imaginary).Replace(",", "."));//_I[i].I.X.Phase * 180 / Math.PI + 180
                //sw.WriteLine((((i+1) + "\t" + I[i].Magnitude + "\t" + I[i].Phase).Replace(",", ".")));//_I[i].I.X.Phase * 180 / Math.PI + 180
            }

            sw.Close();
        }
        private void Report(Stopwatch sw)
        {
            sw.Stop();
            string mess = "Закончено. \n Время расчета " + sw.Elapsed.Minutes.ToString() + " мин " + sw.Elapsed.Seconds.ToString() + " сек.";
            MessageBox.Show(mess);
        }

        private void SaveCurrents(WireCurrent currents)
        {
            //StreamWriter sw = new StreamWriter(saveCurrentAdress);
            //sw.WriteLine(currents.Count.ToString());

            //for (int i = 0; i < currents.Count; i++)
            //{
            //    sw.WriteLine(currents[i].Real + "\t" + currents[i].Imaginary + "\t" + currents[i].Direction.X + "\t" + currents[i].Direction.Y + "\t" + currents[i].Direction.Z + "\t" + currents[i].Segment.V1.X + "\t" + currents[i].Segment.V1.Y + "\t" + currents[i].Segment.V1.Z + "\t" + currents[i].Segment.V2.X + "\t" + currents[i].Segment.V2.Y + "\t" + currents[i].Segment.V2.Z);
            //}
            //sw.WriteLine("END");
            //sw.Close();
        }
        private TriangleMesh FekoSurfaceLoad(string p)
        {
            List<double> vertexX = new List<double>();
            List<double> vertexY = new List<double>();
            List<double> vertexZ = new List<double>();

            List<Int32> int1 = new List<Int32>();
            List<Int32> int2 = new List<Int32>();
            List<Int32> int3 = new List<Int32>();
            ReadingNastran(p, vertexX, vertexY, vertexZ, int1, int2, int3);
            return new TriangleMesh(vertexX, vertexY, vertexZ, int1, int2, int3);
        }
        private void ReadingNastran(string var, List<double> vertexX, List<double> vertexY, List<double> vertexZ, List<Int32> indexP1, List<Int32> indexP2, List<Int32> indexP3)
        {
            string line = "";
            string bufflineX = "";
            string bufflineY = "";
            string bufflineZ = "";


            StreamReader sr = new StreamReader(var);

            for (int i = 0; i < 10; i++)
            {
                line = sr.ReadLine();
            }
            while (!(line.Remove(6) == "CTRIA3"))
            {
                bufflineX = line.Substring(40).Remove(16);
                bufflineY = line.Substring(56).Remove(16);
                line = sr.ReadLine();
                bufflineZ = line.Substring(8);
                vertexX.Add(DoubleRecognition(bufflineX));
                vertexY.Add(DoubleRecognition(bufflineY));
                vertexZ.Add(DoubleRecognition(bufflineZ));
                line = sr.ReadLine();
            }

            while (!(sr.EndOfStream))
            {
                indexP1.Add(Convert.ToInt32(line.Substring(24).Remove(8)));
                indexP2.Add(Convert.ToInt32(line.Substring(32).Remove(8)));
                indexP3.Add(Convert.ToInt32(line.Substring(40)));

                line = sr.ReadLine();
            }
            sr.Close();
        }
        double DoubleRecognition(string l)
        {
            double ans;
            double digit = Convert.ToDouble(l.Remove(12).Replace(".", ","));
            double pow = Convert.ToInt32(l.Substring(13));
            ans = digit * Math.Pow(10, pow);
            return ans;
        }
        private void WriteAngularDiagram(FarField antennaFarField, FarField scatteredFarField)
        {
            StreamWriter sw = new StreamWriter(Path.Combine("C:/Users/Denis/Documents/Documents 2018/Расчет рассеянного поля по ИУ/Feko", "ang_diagram2.txt"));
            sw.WriteLine("Theta" + "\t" + "Phi" + "\t" + "Etotal, dBV" + "\t" + "Ephi, dBV" + "\t" + "Etheta, dBV" + "\t" + "Etotal, dBV" + "\t" + "Ephi, dBV" + "\t" + "Etheta, dBV");
            for (int i = 0; i < antennaFarField.Count; i++)
            {
                sw.WriteLine(((antennaFarField[i].Theta + "\t" + antennaFarField[i].Phi + "\t" + antennaFarField[i].Etotal + "\t" + antennaFarField[i].Ephi + "\t" + antennaFarField[i].Etheta + "\t" + scatteredFarField[i].Etotal + "\t" + scatteredFarField[i].Ephi + "\t" + scatteredFarField[i].Etheta).Replace(",", ".")));
            }
            sw.Close();
        }

        private WireCurrent LoadCurrents(string saveCurrentAdress)
        {
            StreamReader sr = new StreamReader(saveCurrentAdress);

            string line = sr.ReadLine();
            int counter = Convert.ToInt32(line);

            Complex[] values = new Complex[counter];
            DVector[] vectors = new DVector[counter];
            Line[] segments = new Line[counter];

            for (int i = 0; i < counter; i++)
            {
                line = sr.ReadLine().Replace(",", ".");
                string[] arr = line.Split(Convert.ToChar("\t"));
                values[i] = new Complex(Convert.ToDouble(arr[0]), Convert.ToDouble(arr[1]));
                vectors[i] = new DVector(Convert.ToDouble(arr[2]), Convert.ToDouble(arr[3]), Convert.ToDouble(arr[4]));
                segments[i] = new Line(new Point3D(Convert.ToDouble(arr[5]), Convert.ToDouble(arr[6]), Convert.ToDouble(arr[7])), new Point3D(Convert.ToDouble(arr[8]), Convert.ToDouble(arr[9]), Convert.ToDouble(arr[10])), i);
            }

            sr.Close();
            return new WireCurrent(values, vectors, segments);
        }
    }
}
 //public void WithdrawlApperture(Apperture App, string name)
 //       {            
 //           string filename = Path.Combine("C:/Users/Denis/Documents/Documents 2018/Расчет рассеянного поля по ИУ/Apperture", name);

 //           StreamWriter sw = new StreamWriter(filename);
 //           for (int i = 0; i < App.Count; i++)
 //           {
 //               sw.WriteLine(App.ElectricCurrent[i].Position.X + ", " + App.ElectricCurrent[i].Position.Y + " \t" + "{" + App.ElectricCurrent[i].Real + "; " + App.ElectricCurrent[i].Imaginary + "}" + " \t {" + App.MagneticCurrent[i].Real + "; " + App.MagneticCurrent[i].Imaginary + "}");
 //           }
 //           sw.Close();
 //       }

 //       public void WithdrawlNearField(NearField nf, string name)
 //       {
 //           string filename = Path.Combine("C:/Users/Denis/Documents/Documents 2018/Расчет рассеянного поля по ИУ/Apperture", name);

 //           StreamWriter sw = new StreamWriter(filename);
 //           for (int i = 0; i < nf.CountElements; i++)
 //           {
 //               //sw.WriteLine( + ", " + nf.ElectricCurrent[i].Position.Y + " \t" + "{" + nf.ElectricCurrent[i].Real + "; " + nf.ElectricCurrent[i].Imaginary + "}" + " \t {" + nf.MagneticCurrent[i].Real + "; " + nf.MagneticCurrent[i].Imaginary + "}");
 //           }
 //           sw.Close();
 //       }