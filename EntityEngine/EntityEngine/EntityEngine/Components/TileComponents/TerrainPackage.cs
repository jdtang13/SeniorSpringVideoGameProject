using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngine.Components.TileComponents
{
    public class TerrainPackage
    {
        Boolean impassable;
        public Boolean GetImpassable()
        {
            return impassable;
        }

        float movementRestriction;
        public float GetMovementRestriction()
        {
            return movementRestriction;
        }

        Texture2D texture;
        public Texture2D GetTexture()
        {
            return texture;
        }

        float visionBlocked;
        public float GetVisionBlocked()
        {
            return visionBlocked;
        }


        public TerrainPackage(Texture2D myTex, bool myImpassable, float myVisionBlockingFraction)
        {
            impassable = myImpassable;
            texture = myTex;
            visionBlocked = myVisionBlockingFraction;
        }
    }
}
