using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Components.Component_Parents;


namespace EntityEngine.Components.Sprites
{
    public class TextSpriteComponent : DrawableComponent
    {
        //Use this sprite if you want the sprite to be only text. You have to tell them which color to use and which font to use

        SpriteFont font;

        Color color;
        
        string text;
        public string getText()
        {
            return text;
        }
        public void setText(String myText)
        {
            text = myText;
            offset = font.MeasureString(text) / 2;
        }

        public TextSpriteComponent( bool myMain,string myText,Color myColor,  Vector2 myPosition, SpriteFont myFont)
            : base(myMain)
        {
            this.name = "TextSpriteComponent";
            this.position = myPosition;
            color = myColor;
            font = myFont;
            text = myText;
        }

        public override void Initialize()
        {
            offset = font.MeasureString(text)/2;
            this._updateOrder = 1;
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CameraComponent cam = _parent.GetComponent("CameraComponent") as CameraComponent;

            screenPosition = cam.GetDrawPosition(position) - offset;

            spriteBatch.DrawString(font, text, screenPosition, color);
        }
    }
}
