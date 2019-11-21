using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Integral
{
    public class CameraManager
    {
        #region Singleton Pattern
        private static CameraManager instance = null;
        public static CameraManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CameraManager();
                }
                return instance;
            }
        }
        #endregion

        #region Constructor
        private CameraManager() 
        {
            OrbitPanCamera ocp = new OrbitPanCamera();
            
            cameras.Add(ocp);
            
            currentIndex = 0;
            currentCamera = cameras[currentIndex];
        }
        #endregion

        List<Camera> cameras = new List<Camera>();

        public Camera currentCamera;
        int currentIndex;

        public Matrix ViewPerspective
        {
            get
            {
                if (currentCamera is EgoCamera)
                {
                    return ((EgoCamera)currentCamera).ViewPerspective;
                }
                else
                {
                    return currentCamera.ViewPerspective;
                }            
            }
        }

        public string CycleCameras()
        {
            int numCameras = cameras.Count;
            currentIndex = currentIndex + 1;
            if (currentIndex == numCameras)
                currentIndex = 0;
            currentCamera = cameras[currentIndex];
            return currentCamera.ToString();
        }
        public string CameraAt(int i)
        {
            currentIndex = i;
            currentCamera = cameras[currentIndex];
            return currentCamera.ToString();
        }
        public Camera returnCamera(int i)
        {
            return cameras[i];
        }
    }
}
