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
        //Use this code to make something a piece that goes on top of a hex, such as a mountain, wall, or player

        //The hexagon that this placeable is attached to
        HexComponent hex;

        //Change the placeable to the given hex
        public void setHex(HexComponent myHex)
        {
            hex.removePlaceable(this);
            hex = myHex;
            myHex.addPlaceable(this);
        }
        public HexComponent getHex()
        {
            return hex;
        }

        //For ease of access the placeable will also keep track of his coords
        Vector2 coordPosition;

        //The direction this piece is facing
        Orientation orientation;
        public void changeOrientation(Orientation myOar)
        {
            orientation = myOar;
        }


        public PlaceableComponent(Entity myParent, HexComponent myHex) : base(myParent)
        {
            this.name = "PlaceableComponent";
            hex = myHex;
        }

        public override void Update(GameTime gameTime)
        {
            coordPosition = hex.getCoordPosition();
            base.Update(gameTime);
        }

        public void moveDirection(Orientation myOar)
        {
            //Move one hexEntity in a direction
            switch (myOar)
            {
                case Orientation.n:
                    if (hex.n != null)
                    {
                        setHex(hex.n);
                    }
                    break;
                case Orientation.ne:
                    if (hex.ne != null)
                    {
                        setHex(hex.ne);
                    }
                    break;
                case Orientation.se:
                    if (hex.se != null)
                    {
                        setHex(hex.se);
                    }
                    break;
                case Orientation.s:
                    if (hex.s != null)
                    {
                        setHex(hex.s);
                    }
                    break;
                case Orientation.sw:
                    if (hex.sw != null)
                    {
                        setHex(hex.sw);
                    }
                    break;
                case Orientation.nw:
                    if (hex.nw != null)
                    {
                        setHex(hex.nw);
                    }
                    break;

                default:
                    //This should never happen
                    break;
            }

        }

        
    }
}
