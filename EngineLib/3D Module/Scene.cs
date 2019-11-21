using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integral
{
    public class Scene
    {
        #region Singleton Pattern
        private static Scene instance = null;
        public static Scene Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Scene();
                }
                return instance;
            }
        }
        #endregion

        #region Constructor
        private Scene() { }
        #endregion

        List<Renderable> RenderObjects = new List<Renderable>();

        public void addRenderObject(Renderable renderObject)
        {
            lock (RenderObjects)
            {
                RenderObjects.Add(renderObject);
            }
        }

        public void removeRenderObject(Renderable renderObject)
        {
            lock (RenderObjects)
            {
                if( RenderObjects.Contains( renderObject ))
                {
                    RenderObjects.Remove( renderObject );
                }
            }
        }

        public void render()
        {
            lock (RenderObjects)
            {
                foreach (Renderable renderable in RenderObjects)
                {
                    renderable.render();
                }
            }
        }
    }
}
