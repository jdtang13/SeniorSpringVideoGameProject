using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngine.Components.Sprites
{
    class PortraitPackage
    {

        Texture2D leftFacing, rightFacing;

        public PortraitPackage(int myID, 
            Texture2D myLeftFacingTexture, Texture myLeftSmile, Texture2D myLeftFrown,  Texture2D myLeftScowl,
            Texture2D myRightFacingTexture, Texture2D myRightSmile, Texture2D myRightFrown, Texture2D myRightScowl)
        {
        }
    }
}
