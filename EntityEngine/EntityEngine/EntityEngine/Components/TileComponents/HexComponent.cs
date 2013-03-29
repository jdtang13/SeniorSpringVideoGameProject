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
    public class HexComponent : UpdateableComponent
    {
        Vector2 coordPosition;
        public Vector2 getCoordPosition()
        {
            return coordPosition;
        }
        public Boolean checkCoords(Vector2 myVector)
        {
            if (myVector == coordPosition)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public HexComponent n , ne, se, s , sw, nw;

        //List of counters on top of the hex.
        List<PlaceableComponent> placeableList = new List<PlaceableComponent>();
        public void addPlaceable(PlaceableComponent myPlaceable)
        {
            placeableList.Add(myPlaceable);
        }
        public void removePlaceable(PlaceableComponent myPlaceable)
        {
            placeableList.Remove(myPlaceable);
        }

    
        public HexComponent(Entity myParent, Vector2 myCoordPosition) : base(myParent)
        {
            this.name = "HexComponent";
            coordPosition = myCoordPosition;
        }

        public void setAdjacent(HexComponent N, HexComponent NE, HexComponent SE , HexComponent S, HexComponent SW, HexComponent NW)
        {
            n = N; ne = NE; se = SE; s = S; sw = SW; nw = NW;
        }

        //Returns the hex component of the hex entity in a certain direction
        public HexComponent getAdjacent(Orientation myOar)
        {
            switch (myOar)
            {
                case Orientation.n:
                    return n;
                case Orientation.ne:
                    return ne;                   
                case Orientation.se:
                    return se;                    
                case Orientation.s:
                    return s;                    
                case Orientation.sw:
                    return sw;                   
                case Orientation.nw:
                    return nw;                    
                default:
                    return this;
           
            }
        }
    }
}
