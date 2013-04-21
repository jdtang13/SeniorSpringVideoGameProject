using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngine.Components.Sprites
{
    class Texture2DFramed
    {
        public Texture2D texture;
        public int width, height;

        public Texture2DFramed(Texture2D myTexture, int myWidth, int myHeight)
        {
            texture = myTexture;
            width = myWidth;
            height = myHeight;
        }
    }
}
