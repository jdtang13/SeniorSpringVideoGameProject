using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;
using EntityEngine.Components.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngine.Components.TileComponents
{
    public class TerrainComponent : Component
    {
        HexComponent hex;
        public HexComponent GetHex()
        {
            return hex;
        }
        public void SetHex(HexComponent myHex)
        {
            hex = myHex;
        }

        Visibility visibility = Visibility.Unexplored;
        public void SetVisbility(Visibility myVis)
        {
            visibility = myVis;

            SpriteComponent sprite = _parent.GetDrawable("SpriteComponent") as SpriteComponent;

            if (visibility == Visibility.Visible)
            {
                sprite.setColor(Color.White);
                sprite._visible = true;
            }

            if (visibility == Visibility.Explored)
            {
                sprite.setColor(Color.SlateGray);
                sprite._visible = true;
            }

            if (visibility == Visibility.Unexplored)
            {
                sprite._visible = false;
            }
        }

        Boolean impassable;
        public Boolean GetImpassable()
        {
            return impassable;
        }

        float movementRestriction;
        public float GetMovementRestriction()
        {
            return movementRestriction;
        }
        public void SetMovementRestriction(float myMoveRes)
        {
            movementRestriction = myMoveRes;
        }

        Texture2D texture;
        public Texture2D GetTexture()
        {
            return texture;
        }

        public TerrainComponent(Texture2D myTex, bool myImpassable)
        {
            this.name = "TerrainComponent";
            impassable = myImpassable;
            texture = myTex;
        }

        public void UpdateVisibility()
        {
            visibility = hex.GetVisibility();
            SpriteComponent sprite = _parent.GetDrawable("SpriteComponent") as SpriteComponent;

            if (visibility == Visibility.Visible)
            {
                sprite.setColor(Color.White);
                sprite._visible = true;
            }

            else if (visibility == Visibility.Explored)
            {
                sprite.setColor(Color.SlateGray);
                sprite._visible = true;
            }

            else if (visibility == Visibility.Unexplored)
            {

                sprite._visible = false;
            }
        }

        public override void Initialize()
        {
            SetVisbility(hex.GetVisibility());
            base.Initialize();
        }
    }
}
