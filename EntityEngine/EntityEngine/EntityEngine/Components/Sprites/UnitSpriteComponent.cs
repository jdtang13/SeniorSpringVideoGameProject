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
            base.Draw(spriteBatch);

            Entity e = _parent;
            Vector2 pos = GetPosition();
            UnitData u = (e.GetComponent("UnitComponent") as UnitComponent).GetUnitData();

            float healthPercentage = u.GetCurrentHealth() / (float)u.GetMaxHealth();

            int healthBarThickness = 5;
            Color allyHealthColor = Color.CornflowerBlue;
            Color enemyHealthColor = Color.Red;
            Color emptyBarColor = Color.DarkGray;

            spriteBatch.Draw(State.dot, new Rectangle((int)pos.X, (int)pos.Y, (int)(frameWidth * healthPercentage), healthBarThickness), allyHealthColor);
            spriteBatch.Draw(State.dot, new Rectangle((int)pos.X + (int)(frameWidth * healthPercentage), (int)pos.Y, (int)(frameWidth * (1 - healthPercentage)), healthBarThickness), emptyBarColor);

        }
    }
}
