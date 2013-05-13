using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Components.Component_Parents;
using EntityEngine.Components.TileComponents;
using EntityEngine.Stat_Attribute_Classes;

namespace EntityEngine.Components.Sprites
{
    public class UnitSpriteComponent : AnimatedSpriteComponent
    {
        public UnitSpriteComponent(bool isMain, Vector2 position, Texture2DFramed tex)
            : base (isMain, position, tex)
        {
            this.name = "UnitSpriteComponent";
            /*
            this.isMainSprite = isMain;
            this.position = position;
            this.texture = tex.texture;

            frameWidth = tex.frameWidth;
            frameHeight = tex.frameHeight;

            numberFrames = this.texture.Width / frameWidth -1;
            interval = tex.animationSpeed;
            animating = true;*/

            //this = new AnimatedSpriteComponent(isMain, position, texture);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Entity e = _parent;
            HexComponent hex = (_parent.GetComponent("UnitComponent") as UnitComponent).GetHex();
            Vector2 pos = hex.GetCenterPosition();
            UnitData u = (e.GetComponent("UnitComponent") as UnitComponent).GetUnitData();

            Color unitColor = new Color(140, 140, 255); // light blue

            if (u.GetAlignment() == Alignment.ENEMY)
            {
                //  draw the AnimatedSpriteComponent section of the sprite
                unitColor = new Color(255, 140, 140); // light red
            }
            else if (u.GetAlignment() == Alignment.NEUTRAL)
            {
                unitColor = new Color(140, 255, 140); // light green
            }
            //  draw the AnimatedSpriteComponent section of the sprite
            base.Draw(spriteBatch, unitColor);

            float healthPercentage = u.GetCurrentHealth() / (float)u.GetMaxHealth();

            int hexHeight = (hex._parent.GetDrawable("SpriteComponent") as SpriteComponent).spriteHeight;

            //  draw a health bar. blue = ally health, red = enemy health
            int healthBarThickness = 5;
            Color allyHealthColor = Color.CornflowerBlue;
            Color enemyHealthColor = Color.Red;

            Color emptyBarColor = Color.Black;
            int healthBarHeightOffset = +3;

            Color drawColor = allyHealthColor;
            if (u.GetAlignment() == Alignment.ENEMY)
            {
                drawColor = enemyHealthColor;
            }

            int healthBarWidth = frameWidth - 5; // make the health bar slightly smaller than the frame

            //  draw two aspects of the health bar: the "colored" section which represents current health,
            //  and the "black" section which represents health lost
            spriteBatch.Draw(State.dot, new Rectangle((int)pos.X - healthBarWidth / 2, (int)pos.Y - hexHeight + healthBarHeightOffset, (int)(healthBarWidth * healthPercentage), healthBarThickness), drawColor);
            spriteBatch.Draw(State.dot, new Rectangle((int)pos.X - healthBarWidth / 2 + (int)(healthBarWidth * healthPercentage), (int)pos.Y - hexHeight + healthBarHeightOffset, (int)(healthBarWidth * (1 - healthPercentage)), healthBarThickness), emptyBarColor);

            spriteBatch.DrawString(State.font, "Lvl " + u.GetCurrentLevel(), new Vector2(pos.X - frameWidth/2, pos.Y - hexHeight - healthBarHeightOffset - healthBarThickness - 10), Color.White);
        }
    }
}
