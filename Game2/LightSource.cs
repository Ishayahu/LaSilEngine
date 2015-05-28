using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // for GameTime
using Microsoft.Xna.Framework.Graphics; // for SpriteBatch
using Microsoft.Xna.Framework.Input;

namespace Game2
{
    class LightSource
    {
        #region fields
        // сила света
        private float strength;
        #endregion

        #region constructor

        public LightSource(float strength)
        {
            if (strength >= 0)
            {
                this.strength = strength;
            }
            else
            {
                this.strength = 1;
            }
        }
        #endregion

        #region properties
        public float Strength
        {
            get { return strength; }
        }
        #endregion
    }
}
