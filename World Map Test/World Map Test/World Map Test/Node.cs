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
    class Node
    {
        private int index;
        private Vector2 position; //position, sprite, and the node name
        private Texture2D sprite;
        private Rectangle hitbox;
        private string name;
        private List<int> adjacency;

        public Node(int myIndex, Vector2 myPosition, string myName) // setter function
        {
            index = myIndex;
            position = myPosition;
            name = myName;
        }

        public Vector2 Position() { return position; } // return values from class
        public Texture2D Sprite() { return sprite; }
        public Rectangle Hitbox() { return hitbox; }

        public void SetSprite(Texture2D mySprite) // sets the sprite
        {
            sprite = mySprite;
            position.X = position.X - (sprite.Width);
            position.Y = position.Y - (sprite.Height);
        }

        public void SetPosition(Vector2 myPosition) { position = myPosition; } // sets position
        public void SetHitbox(int X, int Y) { hitbox = new Rectangle(X, Y, sprite.Width, sprite.Height); } //sets hitbox

        public void Draw(SpriteBatch mySpriteBatch) // draws the node
        {
                mySpriteBatch.Draw(sprite, position, Color.White);
        }

        public void Update()
        {

        }
    }
}
