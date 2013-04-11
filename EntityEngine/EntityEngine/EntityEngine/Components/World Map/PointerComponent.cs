using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;
using EntityEngine.Components.Sprites;

namespace EntityEngine.Components.World_Map
{
    public class PointerComponent : Component
    {
        Vector2 pointerOffset;
        public Vector2 GetOffset()
        {
            return pointerOffset;
        }

        public PointerComponent(Vector2 myVector)
        {
            this.name = "PointerComponent";
            pointerOffset = myVector;
        }
    }
}
