using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;
using EntityEngine.Components.Sprites;

namespace EntityEngine.Components.TileComponents
{
    public class TerrainComponent : UpdateableComponent
    {
        HexComponent hex;
        public HexComponent GetHex()
        {
            return hex;
        }
        public void SetHex(HexComponent myHex)
        {
            hex.RemoveTerrain(this);
            myHex.AddTerrain(this);
            hex = myHex;
            
        }

        Visibility visibility;
        public void SetVisbility(Visibility myVis)
        {
            visibility = myVis;
        }

        Boolean impassable;
        public Boolean getImpassable()
        {
            return impassable;
        }

        float movementRestriction;
        public float getMovementRestriction()
        {
            return movementRestriction;
        }
        public void setMovementRestriction(float myMoveRes)
        {
            movementRestriction = myMoveRes;
        }

        public TerrainComponent(Entity myParent, HexComponent myHex, bool myImpassable)
            : base(myParent)
        {
            this.name = "TerrainComponent";
            hex = myHex;
            impassable = myImpassable;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateVisibility();
            base.Update(gameTime);
        }

        public void UpdateVisibility()
        {  
            visibility = hex.GetVisibility();
            SpriteComponent sprite = _parent.getDrawable("SpriteComponent") as SpriteComponent;

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

        


    }
}
