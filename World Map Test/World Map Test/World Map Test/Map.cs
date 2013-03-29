using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace World_Map_Test
{
    class Map
    {
        public Vector2 position;
        public Texture2D sprite;

        public Map(Vector2 myPosition) //setter function
        {
            position = myPosition;
        }

        public Vector2 Position() { return position; }
        public Texture2D Sprite() { return sprite; }
        public void SetSprite(Texture2D mySprite)
        {
            sprite = mySprite;
        }
    }
}
