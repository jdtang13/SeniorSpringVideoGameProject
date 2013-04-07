using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;

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
            impassable = myImpassable;
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        


    }
}
