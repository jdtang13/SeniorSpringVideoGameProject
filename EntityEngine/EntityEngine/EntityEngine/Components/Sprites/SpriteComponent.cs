using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Components.Component_Parents;


namespace EntityEngine.Components.Sprites
{
    public class SpriteComponent : DrawableComponent
    {
        //Use this component if you want to add a sprite that uses the whole texture when its drawing

        public Vector2 topLeftCornerScreenPosition, centerScreenPosition;

        public Vector2 getCenterPosition()
        {
            return centerScreenPosition;
        }
        public Vector2 getTopLeftPosition()
        {
            return topLeftCornerScreenPosition;
        }

        float rotation = 0f;
        public void setRotation(float myRot)
        {
            rotation = myRot;
        }

        public int spriteWidth, spriteHeight;

        Color color;
        public void setColor(Color myColor)
        {
            color = myColor;
        }

        //Use this constructor if you want to pass in a rotation of the sprite so that it moves aroudn
        public SpriteComponent(Entity myParent,bool myMain,Vector2 myPosition,Texture2D myTex, float myRot)
                               : base(myParent,myMain)
        {
            this.name = "SpriteComponent";
            this.position = myPosition;
            this.texture = myTex;
            this.rotation = myRot;
        }

        //If the sprite isnt going to rotate
        public SpriteComponent(Entity myParent, bool myMain, Vector2 myPosition, Texture2D myTex)
                                : base(myParent,myMain)
        {
            this.name = "SpriteComponent";
            this.position = myPosition;
            this.texture = myTex;
        }
       

        public override void Initialize()
        {
            this._updateOrder = 1;

            color = Color.White;

            spriteHeight = this.texture.Height;
            spriteWidth = this.texture.Width;
            this.offset = new Vector2(spriteWidth / 2, spriteHeight / 2);
            
            topLeftCornerScreenPosition = this.position - offset;
            centerScreenPosition = this.position;

            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CameraComponent cam = _parent.getComponent("CameraComponent") as CameraComponent;

            screenPosition = cam.getDrawPosition(position) - offset;

            centerScreenPosition = position;
            topLeftCornerScreenPosition = screenPosition;

            spriteBatch.Draw(texture, screenPosition , null, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

            
        }

    }
}
