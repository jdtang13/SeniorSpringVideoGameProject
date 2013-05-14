using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngine.Dialogue
{
    public class PortraitPackage
    {
        public Dictionary<Emotion, Texture2D[]> textures = new Dictionary<Emotion, Texture2D[]>();
        public void AddEmotionTexture(Emotion myEmotion, Texture2D[] myTextures)
        {
            textures.Add(myEmotion, myTextures);
        }
        public Texture2D GetTexture(Emotion myEmotion, int myLeftOrRight)
        {
            if (textures[myEmotion][myLeftOrRight] != null)
            {
                return textures[myEmotion][myLeftOrRight];
            }
            else
            {
                //Woah! You better handle that null, bro!
                return null;
            }
        }

        public PortraitPackage()
        {
        }
        
    }
}
