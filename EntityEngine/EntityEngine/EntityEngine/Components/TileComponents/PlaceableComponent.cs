using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;

namespace EntityEngine.Components.TileComponents
{
    public class PlaceableComponent : UpdateableComponent
    {
        HexComponent hex;

        public void SetHex(HexComponent myHex)
        {
            hex = myHex;
            hex.removePlaceable(this);
            hex.addPlaceable(this);
        }
        public HexComponent GetHex()
        {
            return hex;
        }

        public PlaceableComponent(Entity myParent, HexComponent myHex)
            : base(myParent)
        {
            this.name = "PlaceableComponent";
            this.hex = myHex;

        }
    }
}
