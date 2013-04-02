using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;

namespace EntityEngine.Components.TileComponents
{
    public class UnitComponent : PlaceableComponent
    {
        bool selected = false;
        public bool getSelected()
        {
            return selected;
        }
        public void setSelected(bool myTruth)
        {
            selected = myTruth;
        }

        Orient orientation = Orient.s;
        public void changeOrientation(Orient myOar)
        {
            orientation = myOar;
        }

        CommandState commandState;
        public void SetCommandState(CommandState myState)
        {
            commandState = myState;
        }

        public UnitComponent(Entity myParent, HexComponent myHex, bool mySelectable)
            : base(myParent, myHex)
        {
            this.name = "UnitComponent";
        }

        //public void moveDirection(Orientation myOar)
        //{
        //    //Move one hexEntity in a direction
        //    switch (myOar)
        //    {
        //        case Orientation.n:
        //            if (hex.n != null)
        //            {
        //                setHex(hex.n);
        //            }
        //            break;
        //        case Orientation.ne:
        //            if (hex.ne != null)
        //            {
        //                setHex(hex.ne);
        //            }
        //            break;
        //        case Orientation.se:
        //            if (hex.se != null)
        //            {
        //                setHex(hex.se);
        //            }
        //            break;
        //        case Orientation.s:
        //            if (hex.s != null)
        //            {
        //                setHex(hex.s);
        //            }
        //            break;
        //        case Orientation.sw:
        //            if (hex.sw != null)
        //            {
        //                setHex(hex.sw);
        //            }
        //            break;
        //        case Orientation.nw:
        //            if (hex.nw != null)
        //            {
        //                setHex(hex.nw);
        //            }
        //            break;

        //        default:
        //            //This should never happen
        //            break;
        //    }

        //}
    }
}
