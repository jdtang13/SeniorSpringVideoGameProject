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
    class Pointer
    {
        private Texture2D sprite;//variables for the pointer class--only needs sprite & location
        private Vector2 position;
        private bool enabled;

        public Pointer()//constructor
        {
            enabled = false;
        }

        public Vector2 Position() { return position; } //getters
        public Texture2D Sprite() { return sprite; }
        public bool Enabled() { return enabled; }

        public void SetPosition(Vector2 myPosition) { position = myPosition; } //setters
        public void SetSprite(Texture2D mySprite) { sprite = mySprite; }
        public void SetEnabled(bool myEnabled) { enabled = myEnabled; }

        public void Draw(SpriteBatch mySpriteBatch) // draws the pointer
        {
            mySpriteBatch.Draw(sprite, position, Color.White);
        }
    }
}
