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

        public Vector2 centerScreenPosition;

        public Vector2 getCenterPosition()
        {
            return centerScreenPosition;
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
        public SpriteComponent(bool myMain,Vector2 myPosition,Texture2D myTex, float myRot)
                               : base(myMain)
        {
            this.name = "SpriteComponent";
            this.position = myPosition;
            this.texture = myTex;
            this.rotation = myRot;
        }

        //If the sprite isnt going to rotate
        public SpriteComponent( bool myMain, Vector2 myPosition, Texture2D myTex)
                                : base(myMain)
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
            
            centerScreenPosition = this.position;

            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CameraComponent cam = _parent.GetComponent("CameraComponent") as CameraComponent;

            screenPosition = cam.GetDrawPosition(position) - offset;

            centerScreenPosition = position;

            spriteBatch.Draw(texture, screenPosition , null, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

            
        }

    }
}
