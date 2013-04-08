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
    public class UnitComponent : UpdateableComponent
    {
        HexComponent hex;
        public HexComponent GetHex()
        {
            return hex;
        }
        public void SetHex(HexComponent myHex)
        {
            hex.SetUnit(null);
            hex = myHex;
            myHex.SetUnit(this);
        }

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

        Visibility visibility;
        public void SetVisbility(Visibility myVis)
        {
            visibility = myVis;
        }

        int sightRadius;
        public int GetSightRadius()
        {
            return sightRadius;
        }

        CommandState commandState;
        public void SetCommandState(CommandState myState)
        {
            commandState = myState;
        }

        public UnitComponent(Entity myParent, int mySightRadius, HexComponent myHex, bool mySelectable)
            : base(myParent)
        {
            this.name = "UnitComponent";
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
