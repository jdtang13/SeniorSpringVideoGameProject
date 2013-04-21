using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngine.Components.Sprites
{
    public class Texture2DFramed
    {
        //A class that holds the important stuff of a animation so that it can be set earlier in loadcontent process

        public Texture2D texture;
        public int frameWidth, frameHeight;
        public float animationSpeed;

        public Texture2DFramed(Texture2D myTexture,float myAnimationSpeed, int myWidth, int myHeight)
        {
            animationSpeed = myAnimationSpeed;
            texture = myTexture;
            frameWidth = myWidth;
            frameHeight = myHeight;
        }
    }
}
