using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Components.Component_Parents;


namespace EntityEngine.Components.Sprites
{
    public class SpriteSheetComponent : DrawableComponent
    {
        //Use this component if you want to have a sprite that uses only a portion of the texture. Think of a grid of a bunch
        //of sprites and only one sprite gets used at a time.

        Vector2 currentFrame;
        public void setCurrentFrame(Vector2 myFrame)
        {
            currentFrame = myFrame;
        }
        public void setCurrentFrameX(int x)
        {
            currentFrame.X = x;
        }
        public void setCurrentFrameY(int y)
        {
            currentFrame.X = y;
        }

        public int spriteWidth, spriteHeight;

        Color color;
        public void setColor(Color myColor)
        {
            color = myColor;
        }

        public SpriteSheetComponent( bool myMain, Vector2 myPosition, Texture2D myTex, int mySpriteWidth, int mySpriteHeight)
            : base(myMain)
        {
            this.name = "SpriteSheetComponent";
            this.position = myPosition;
            this.texture = myTex;

            spriteHeight = mySpriteHeight;
            spriteWidth = mySpriteWidth;
        }

        public override void Initialize()
        {
            color = Color.White;
            this.offset = new Vector2(spriteWidth / 2, spriteHeight / 2);
            this._updateOrder = 1;
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, position-offset,
                new Rectangle(spriteWidth * (int)currentFrame.X, spriteHeight * (int)currentFrame.Y, spriteWidth, spriteHeight), 
                color);

        }

    }
}
