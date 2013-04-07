using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Components.Component_Parents;


namespace EntityEngine.Components.Sprites
{
    public class AnimatedSpriteComponent : DrawableComponent
    {
        //Use this component when you want to have a sprite that is animated

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

        float timer = 0;
        float interval;
        int numberFrames;

        bool animating;//Turns the animating on and off
        public void SetAnimated(bool myTruth)
        {
            animating = myTruth;
        }

        //You have to pass in the size of the frame of the spite within the spritesheet

        public AnimatedSpriteComponent(Entity myParent, bool myMain, Vector2 myPosition, Texture2D myTex,
                float myInterval, int mySpriteWidth, int mySpriteHeight)
            : base(myParent,myMain)
        {
            this.name = "AnimatedSpriteComponent";
            this.position = myPosition;

            this.texture = myTex;
            spriteWidth = mySpriteWidth;
            spriteHeight = mySpriteHeight;

            numberFrames = this.texture.Width / spriteWidth;
            interval = myInterval;
            animating = true;
        }
        public AnimatedSpriteComponent(Entity myParent, bool myMain, Vector2 myPosition, Texture2D myTex,
                float myInterval, int mySpriteWidth, int mySpriteHeight, int myXOffset, int myYOffset)
            : base(myParent, myMain)
        {
            this.name = "AnimatedSpriteComponent";
            this.position = myPosition;

            this.texture = myTex;
            spriteWidth = mySpriteWidth;
            spriteHeight = mySpriteHeight;

            numberFrames = this.texture.Width / spriteWidth;
            interval = myInterval;
            animating = true;
        }

        public override void Initialize()
        {
            this.offset = new Vector2(spriteWidth / 2, spriteHeight / 2);
            this._updateOrder = 1;
            base.Initialize();
        }

        public override void Update(GameTime myTime)
        {
            timer += myTime.ElapsedGameTime.Milliseconds;

            if (animating)
            {
                if (timer > interval)
                {
                    timer = timer - interval;
                    if (currentFrame.X != numberFrames)
                    {
                        currentFrame.X++;
                    }
                    else
                    {
                        currentFrame.X -= numberFrames;
                    }
                }
            }

            base.Update(myTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CameraComponent cam = _parent.getUpdateable("CameraComponent") as CameraComponent;

            screenPosition = cam.getDrawPosition(position) - offset;

            spriteBatch.Draw(texture, screenPosition,
                new Rectangle(spriteWidth * (int)currentFrame.X, spriteHeight * (int)currentFrame.Y, spriteWidth, spriteHeight), Color.White);
        }
    }
}
