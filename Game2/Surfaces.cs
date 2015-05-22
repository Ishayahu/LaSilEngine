using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game2
{
    public class Surface
    {
        
        #region constructor
        /// <summary>
        /// 
        /// </summary>
        public Surface() { }
        public Surface(Random rand)
        {
            this.rand = rand;
        }
        #endregion

        #region fields
        protected float visibility;
        int surfaceCount = 4;
        Random rand;
        protected string surfaceCode;
        #endregion

        #region methods
        public Surface GetRandormSurface()
        {
            int surfaceTypeCode = rand.Next(surfaceCount);
            Surface res;
            switch (surfaceTypeCode)
            {
                case 0:
                    {
                        res = new Grass();
                        break;
                    }
                case 1:
                    {
                        res = new Water();
                        break;
                    }
                case 2:
                    {
                        res = new Fog();
                        break;
                    }
                case 3:
                    {
                        res = new Mountine();
                        break;
                    }
                default:
                    {
                        res = new Grass();
                        break;
                    }

            }
            return res;
        }
        virtual public int IsTraversal(Camera camera) 
        {
            throw new NotImplementedException();
            //return -1;
        }
        #endregion

        #region properties
        public float Visibility
        {
            get { return visibility; }
        }
        public string SurfaceCode
        {
            get { return surfaceCode; }
        }
        #endregion
    }

    public class Grass: Surface
    {
        
        public Grass()
        {
            surfaceCode = "G";
            visibility = 0;
        }
    #region methods
        override public int IsTraversal(Camera camera)
        {
            return 1;
        }
    #endregion
    }

    public class Water : Surface
    { 
        public Water()
        {
            surfaceCode = "W";
            visibility = 0;
        }
        #region methods
        override public int IsTraversal(Camera camera)
        {
            return 0;
        }
        #endregion
    }

    public class Fog : Surface
    { 
        public Fog() 
        {
            surfaceCode = "F";
            visibility = 0.5F;
        }
        #region methods
        override public int IsTraversal(Camera camera)
        {
            return 2;
        }
        #endregion
    }

    public class Mountine : Surface
    {
        public Mountine()
        {
            surfaceCode = "M";
            visibility = 1F;
        }
        #region methods
        override public int IsTraversal(Camera camera)
        {
            return -1;
        }
        #endregion
    }
}
