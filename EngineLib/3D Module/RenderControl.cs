using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SlimDX;
using Integral;
using System.Threading;

namespace Integral
{
    public partial class RenderControl : UserControl
    {
        List<Arc3d> pelleng;
        List<Polyline> wireObject = new List<Polyline>();
        IncArrow2 kvector;
        Ball Target;
        List<Arc3d> farfield = new List<Arc3d>();
        TriangleSurface triangleObject;
        TriangleSurface apperture;
        Arc3d scanRegion;

        public RenderControl()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(RenderControl_MouseWheel);
            FrameCounter.Instance.FPSCalculatedEvent += new FrameCounter.FPSCalculatedHandler(Instance_FPSCalculatedEvent);            
        }
        
        delegate void setFPS(string fps);
        void Instance_FPSCalculatedEvent(string fps)
        {
            if (this.InvokeRequired)
            {
                setFPS d = new setFPS(Instance_FPSCalculatedEvent);
                this.Invoke(d, new object[] { fps });
            }
            else
            {
                this.DebugTextLabel.Text = fps;
            }
        }

        public void DeletePelleng()
        {
            try
            {
                for (int i = 0; i < pelleng.Count; i++)
                {
                    Scene.Instance.removeRenderObject(pelleng[i]);
                }
            }
            catch (Exception)
            {
                
             
            }            
        }
        public void DrawPelleng(double thetaStart, double thetaFinish, double phiStart, double step, int system, double delta, string workPlane, string axis, bool azimut)
        {
            DeletePelleng();
            pelleng = new List<Arc3d>();
            int thetaScan_count = Convert.ToInt32(Math.Abs(thetaFinish - thetaStart) / step) + 1;
            for (int i = 0; i < thetaScan_count; i++)
            {
                int SysOfCoordAzimut = 0; int SysOfCoordUglomest = 0;                

                double thetaGlobal = MainForm.GetThetaGlobal(phiStart, thetaStart + i * step, system);
                double phiGlobal = MainForm.GetPhiGlobal(phiStart, thetaStart + i * step, system);

                if (workPlane == "XY")
                {
                    if (axis == "Плоскость YZ")
                    {
                        SysOfCoordAzimut = 2;                                    //SysOfCoordAzimut = 2;                                    
                        SysOfCoordUglomest = 1;
                    }
                    else if (CreateAppertureForm.Axis == "Плоскость XZ")
                    {
                        SysOfCoordAzimut = 1;
                        SysOfCoordUglomest = 2;
                    }
                }
                else if (workPlane == "XZ")
                {
                    if (axis == "Плоскость XY")
                    {
                        SysOfCoordAzimut = 0;
                        SysOfCoordUglomest = 2;
                    }
                    else if (axis == "Плоскость YZ")
                    {
                        SysOfCoordAzimut = 2;
                        SysOfCoordUglomest = 0;
                    }
                }
                else if (workPlane == "YZ")
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

                double phiAzimut = MainForm.GetPhiLocal(phiGlobal, thetaGlobal, SysOfCoordAzimut);
                double phiUglomest = MainForm.GetPhiLocal(phiGlobal, thetaGlobal, SysOfCoordUglomest);

                double thetaStartAzimut = MainForm.GetThetaLocal(phiGlobal, thetaGlobal, SysOfCoordAzimut) - delta;
                double thetaFinishAzimut = MainForm.GetThetaLocal(phiGlobal, thetaGlobal, SysOfCoordAzimut) + delta;

                double thetaStartUglomest = MainForm.GetThetaLocal(phiGlobal, thetaGlobal, SysOfCoordUglomest) - delta;
                double thetaFinishUglomest = MainForm.GetThetaLocal(phiGlobal, thetaGlobal, SysOfCoordUglomest) + delta;

                if (azimut)
                {
                    pelleng.Add(new Arc3d(10, thetaStartAzimut, thetaFinishAzimut, phiAzimut, phiAzimut, 1, SysOfCoordAzimut, Color.Cyan.ToArgb()));
                }
                else
                {
                    pelleng.Add(new Arc3d(10, thetaStartUglomest, thetaFinishUglomest, phiUglomest, phiUglomest, 1, SysOfCoordUglomest, Color.Cyan.ToArgb()));
                }                
            }

            for (int i = 0; i < pelleng.Count; i++)
            {
                Scene.Instance.addRenderObject(pelleng[i]);
            }
        }

        public void init()
        {            
            int size_coeff = 10;
            DeviceManager.Instance.createDeviceAndSwapChain(this);
            RenderManager.Instance.init();

            Grid ground = new Grid(20 * size_coeff, 10.0f * size_coeff);
            Arrows arr = new Arrows(size_coeff);
            

            Scene.Instance.addRenderObject(ground);
            Scene.Instance.addRenderObject(arr);
            
        }

        public void shutDown()
        {
            RenderManager.Instance.shutDown();
            DeviceManager.Instance.shutDown();
        }



        public void DrawKVector(double theta, double phi, double polariz)
        {            
            try
            {
                Scene.Instance.removeRenderObject(kvector);
                kvector = new IncArrow2(10, theta, phi, polariz);
                Scene.Instance.addRenderObject(kvector);
            }
            catch (Exception)
            {
                kvector = new IncArrow2(10, theta, phi, polariz);
                Scene.Instance.addRenderObject(kvector);
            }
            
        }
        
        
        public void ChangeCamera(int c)
        {
            Vector3 eye  = new Vector3();
            Vector3 target = new Vector3();
            Vector3 up = new Vector3();
            if (c == 0)
            {
                CameraManager.Instance.CameraAt(c);
            }
            else
	        {
                if (c == 1)
                {
                    CameraManager.Instance.CameraAt(c);
                    eye = new Vector3(500, 0, 0);
                    target = new Vector3(0, 0, 0);
                    up = new Vector3(0, 1, 0);
                    CameraManager.Instance.returnCamera(c).setView(eye, target, up);
                }
                else if (c == 2)
                {
                    CameraManager.Instance.CameraAt(c);
                    eye = new Vector3(0, 0, 500);
                    target = new Vector3(0, 0, 0);
                    up = new Vector3(1, 0, 0);
                    CameraManager.Instance.returnCamera(c).setView(eye, target, up);
                }
                else if (c == 3)
                {
                    CameraManager.Instance.CameraAt(c);
                    eye = new Vector3(0, 500, 0);
                    target = new Vector3(0, 0, 0);
                    up = new Vector3(0, 0, 1);
                }
                CameraManager.Instance.returnCamera(c).eye = eye;
                CameraManager.Instance.returnCamera(c).target = target;
                CameraManager.Instance.returnCamera(c).up = up;
                CameraManager.Instance.returnCamera(c).setView(eye, target, up);
            }
            

        }

        private void RenderControl_MouseUp(object sender, MouseEventArgs e)
        {
            CameraManager.Instance.currentCamera.MouseUp(sender, e);
            Scene.Instance.removeRenderObject(Target);
        }

        private void RenderControl_MouseDown(object sender, MouseEventArgs e)
        {
            CameraManager.Instance.currentCamera.MouseDown(sender, e);
            Vector3 target = CameraManager.Instance.currentCamera.target;
            Target = new Ball(10, target.X, target.Y, target.Z);
            Scene.Instance.addRenderObject(Target);
        }

        private void RenderControl_MouseMove(object sender, MouseEventArgs e)
        {
            CameraManager.Instance.currentCamera.MouseMove(sender, e);
        }

        void RenderControl_MouseWheel(object sender, MouseEventArgs e)
        {
            CameraManager.Instance.currentCamera.MouseWheel(sender, e);
        }

        private void RenderControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                CameraManager.Instance.CycleCameras();
            }
            else if (e.KeyCode == Keys.F2)
            {
                RenderManager.Instance.SwitchSyncInterval();
            }

            CameraManager.Instance.currentCamera.KeyUp(sender, e);
        }

        private void RenderControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            CameraManager.Instance.currentCamera.KeyPress(sender, e);
        }

        private void RenderControl_KeyDown(object sender, KeyEventArgs e)
        {
            CameraManager.Instance.currentCamera.KeyDown(sender, e);
        }


        internal void Add(WireMesh geometry)
        {
            Polyline newWire = new Polyline(geometry);
            wireObject.Add(newWire);
            Scene.Instance.addRenderObject(newWire);
        }

        internal void DrawFarFieldRequest(string title, double thetaStart, double thetaFinish, double phiStart, double phiFinish, double step, int sys, int color)
        {
            double delta_render = step;
            try
            {
                
                if (delta_render < 1)
                {
                    delta_render = 1;
                }

                Scene.Instance.removeRenderObject(farfield.Find(f => f.Title == title));
                Arc3d newDraw = new Arc3d(10, thetaStart, thetaFinish, phiStart, phiFinish, delta_render, sys, color, title);
                farfield.Add(newDraw);
                Scene.Instance.addRenderObject(newDraw);
            }
            catch (Exception)
            {
                Arc3d newDraw = new Arc3d(10, thetaStart, thetaFinish, phiStart, phiFinish, delta_render, sys, color, title);
                farfield.Add(newDraw);
                Scene.Instance.addRenderObject(newDraw);
            }
        }
        internal void ReDrawFarFieldRequest(string formerName, string newName, double ThetaStart, double ThetaFinish, double PhiStart, double PhiFinish, double Delta, int sys, int color)
        {            
            Arc3d req = farfield.Find(f => f.Title == formerName);
            Scene.Instance.removeRenderObject(req);
            farfield.Remove(req);

            if (Delta < 1)
            {
                Delta = 1;
            }
            Arc3d newDraw = new Arc3d(10, ThetaStart, ThetaFinish, PhiStart, PhiFinish, Delta, sys, color, newName);
            farfield.Add(newDraw);
            Scene.Instance.addRenderObject(newDraw);      
        }


        internal void Add(TriangleMesh surfaceGeometry)
        {
            List<double> vertexX = surfaceGeometry.ListX;
            List<double> vertexY = surfaceGeometry.ListY;
            List<double> vertexZ = surfaceGeometry.ListZ;

            List<Int32> int1 = surfaceGeometry.ListI1;
            List<Int32> int2 = surfaceGeometry.ListI2;
            List<Int32> int3 = surfaceGeometry.ListI3;
            int color = System.Drawing.Color.FromArgb(225, 225, 225).ToArgb();
            triangleObject = new TriangleSurface(vertexX, vertexY, vertexZ, int1, int2, int3, color);
            Scene.Instance.addRenderObject(triangleObject);
        }

        internal void Add(Apperture app)
        {
            if (apperture != null)
            {
                Remove(app);
            }
            List<double> vertexX = app.ListX;
            List<double> vertexY = app.ListY;
            List<double> vertexZ = app.ListZ;

            List<Int32> int1 = app.ListI1;
            List<Int32> int2 = app.ListI2;
            List<Int32> int3 = app.ListI3;

            int color = System.Drawing.Color.Yellow.ToArgb();
            apperture = new TriangleSurface(vertexX, vertexY, vertexZ, int1, int2, int3, color);
            Scene.Instance.addRenderObject(apperture);
        }


        internal void Remove(Apperture app)
        {
            Scene.Instance.removeRenderObject(apperture);
        }
        internal void Remove(WireMesh wire)
        {
            Polyline removeWire = wireObject.Find(w => w.Name == wire.Name);
            wireObject.Remove(removeWire);
            Scene.Instance.removeRenderObject(removeWire);
        }

        internal void Remove(FarFieldRequestTemplate farfieldTemplate)
        {
            Arc3d farfieldRemove = farfield.Find(f => f.Title == farfieldTemplate.Title);
            farfield.Remove(farfieldRemove);
            Scene.Instance.removeRenderObject(farfieldRemove);
        }
        internal void DrawScanRegion(double thetaStart, double thetaFinish, double phiStart, double phiFinish, double step, int workPlane)
        {
            try
            {
                Scene.Instance.removeRenderObject(scanRegion);
                scanRegion = new Arc3d(10, thetaStart, thetaFinish, phiStart, phiFinish, step, workPlane, Color.Cyan.ToArgb());
                Scene.Instance.addRenderObject(scanRegion);
            }
            catch (Exception)
            {
                scanRegion = new Arc3d(10, thetaStart, thetaFinish, phiStart, phiFinish, step, workPlane, Color.Cyan.ToArgb());
                Scene.Instance.addRenderObject(scanRegion);
            }
        }

        
    }
}
