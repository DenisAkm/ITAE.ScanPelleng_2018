using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SlimDX;
using SlimDX.DXGI;
using System.Diagnostics;

namespace Integral
{
    public class RenderManager
    {
        #region Singleton Pattern
        private static RenderManager instance = null;
        public static RenderManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RenderManager();
                }
                return instance;
            }
        }
        #endregion

        #region Constructor
        private RenderManager() { }
        #endregion
        
        Thread renderThread;
       
        int syncInterval = 1;

        public void SwitchSyncInterval()
        {
                if (syncInterval == 0)
                {
                    syncInterval = 1;
                }
                else if (syncInterval == 1)
                {
                    syncInterval = 0;
                }
        }

        FrameCounter fc = FrameCounter.Instance;

        public void renderScene()
        {
            while (true)
            {
                fc.Count();
               
                DeviceManager dm = DeviceManager.Instance;
                dm.context.ClearRenderTargetView(dm.renderTarget, new Color4(0.0f, 0.0f, 0.0f));

                Scene.Instance.render();

                dm.swapChain.Present(syncInterval, PresentFlags.None);
            }
        }

        public void init()
        {
            renderThread = new Thread(new ThreadStart(renderScene));
            renderThread.Start();
        }

        public void shutDown()
        {
            renderThread.Abort();
        }
    }
}
