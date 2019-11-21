using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Resource = SlimDX.Direct3D11.Resource;
using Device = SlimDX.Direct3D11.Device;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Collections.ObjectModel;


namespace Integral
{
    public class DeviceManager
    {

        #region Singleton Pattern
        private static DeviceManager instance = null;
        public static DeviceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DeviceManager();
                }
                return instance;
            }
        }
        #endregion

        #region Constructor
        private DeviceManager() { }
        #endregion
        
        public Device device;
        public SwapChain swapChain;
        public Viewport viewport;
        public RenderTargetView renderTarget;
        public DeviceContext context;

        public void getCaps()
        {
            Factory1 fac = new Factory1();
            int numAdapters = fac.GetAdapterCount1();
            List<Adapter1> adapts = new List<Adapter1>();
            for (int i = 0; i < numAdapters; i++)
            {
                adapts.Add(fac.GetAdapter1(i));
            }

            Output outp = adapts[0].GetOutput(0);

            List<Format> formats = new List<Format>();
            foreach (Format format in Enum.GetValues(typeof(Format)))
            {
                formats.Add(format);
            }

            List<ReadOnlyCollection<ModeDescription>> ll = new List<ReadOnlyCollection<ModeDescription>>();

            for (int i = 0; i < formats.Count - 1; i++)
            {
                ReadOnlyCollection<ModeDescription> mdl;
                mdl = outp.GetDisplayModeList(formats[i], DisplayModeEnumerationFlags.Interlaced);
                ll.Add(mdl);
                if (mdl != null)
                {
                    Console.WriteLine(formats[i].ToString());
                }
            }
        }

        public void createDeviceAndSwapChain(System.Windows.Forms.Control form)
        {
            var description = new SwapChainDescription()
            {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out device, out swapChain);

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(device, resource);

            // setting a viewport is required if you want to actually see anything
            context = device.ImmediateContext;
            viewport = new Viewport(0.0f, 0.0f, form.ClientSize.Width, form.ClientSize.Height);
            context.OutputMerger.SetTargets(renderTarget);
            context.Rasterizer.SetViewports(viewport);

            



            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAltEnter);

            // handle alt+enter ourselves
            form.KeyDown += (o, e) =>
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                {
                    swapChain.IsFullScreen = !swapChain.IsFullScreen;
                }
            };
        }

        public void shutDown()
        {
            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }
    }
}
