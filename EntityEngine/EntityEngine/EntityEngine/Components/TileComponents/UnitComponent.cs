using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;
using EntityEngine.Components.Sprites;
using EntityEngine;
using EntityEngine.Stat_Attribute_Classes;

namespace EntityEngine.Components.TileComponents
{
    public class UnitComponent : Component
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
        public bool GetSelected()
        {
            return selected;
        }
        public void SetSelected(bool myTruth)
        {
            selected = myTruth;
            
            foreach(TerrainComponent terrain in hex.GetTerrain())
            {
                SpriteComponent sprite = terrain._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                if (myTruth)
                {
                    sprite.setColor(Color.Red);
                }
                if (!myTruth)
                {
                    sprite.setColor(Color.White);
                } 
            }    
        }

        bool selectable = false;
        public bool GetSelectable()
        {
            return selectable;
        }

        Orient orientation = Orient.s;
        public void changeOrientation(Orient myOar)
        {
            orientation = myOar;
        }

        //TODO: Somehow set this
        Visibility visibility;
        public void SetVisbility(Visibility myVis)
        {
            visibility = myVis;
            AnimatedSpriteComponent sprite = _parent.GetDrawable("AnimatedSpriteComponent") as AnimatedSpriteComponent;

            UnitDataComponent unitDataComp = _parent.GetComponent("UnitDataComponent") as UnitDataComponent;

            if (unitDataComp.GetAlignment() == Alignment.PLAYER)
            {
                sprite.SetColor(Color.White);
                sprite._visible = true;
            }
            else if (visibility == Visibility.Visible)
            {
                sprite.SetColor(Color.White);
                sprite._visible = true;
            }

            else if (visibility == Visibility.Explored)
            {
                sprite.SetColor(Color.SlateGray);
                sprite._visible = true;
            }

            else if (visibility == Visibility.Unexplored)
            {
                sprite._visible = false;
            }
        }

        CommandState commandState;
        public void SetCommandState(CommandState myState)
        {
            commandState = myState;
        }

        //UnitData unitData;
        //public UnitData GetUnitData()
        //{
        //    return unitData;
        //}
        //public void SetUnitData(UnitData u) { unitData = u; }

        public UnitComponent(HexComponent myHex, bool mySelectable)//, UnitData unitData)
        {
            hex = myHex;
            this.name = "UnitComponent";
            selectable = mySelectable;
        }

        public void MoveDirection(Orient myOar)
        {
            //Move one hexEntity in a direction
            switch (myOar)
            {
                case Orient.n:
                    if (hex.n != null)
                    {
                        if (!hex.n.HasUnit())
                        {
                            hex.n.SetUnit(this);
                            SetHex(hex.n);
                            hex.RemoveUnit();
                        }
                    }
                    break;
                case Orient.ne:
                    if (hex.ne != null)
                    {
                        if (!hex.ne.HasUnit())
                        {
                            hex.ne.SetUnit(this);
                            SetHex(hex.ne);
                            hex.RemoveUnit();
                        }
                    }
                    break;
                case Orient.se:
                    if (hex.se != null)
                    {
                        if (!hex.se.HasUnit())
                        {
                            hex.se.SetUnit(this);
                            SetHex(hex.se);
                            hex.RemoveUnit();
                        }
                    }
                    break;
                case Orient.s:
                    if (hex.s != null)
                    {
                        if (!hex.s.HasUnit())
                        {
                            hex.s.SetUnit(this);
                            SetHex(hex.s);
                            hex.RemoveUnit();
                        }
                    }
                    break;
                case Orient.sw:
                    if (hex.sw != null)
                    {
                        if (!hex.sw.HasUnit())
                        {
                            hex.sw.SetUnit(this);
                            SetHex(hex.sw);
                            hex.RemoveUnit();
                        }
                    }
                    break;
                case Orient.nw:
                    if (hex.nw != null)
                    {
                        if (!hex.nw.HasUnit())
                        {
                            hex.nw.SetUnit(this);
                            SetHex(hex.nw);
                            hex.RemoveUnit();
                        }
                    }
                    break;

                default:
                    //This should never happen
                    break;
            }
            AnimatedSpriteComponent sprite = _parent.GetDrawable("AnimatedSpriteComponent") as AnimatedSpriteComponent;
            SpriteComponent hexSprite = hex._parent.GetDrawable("SpriteComponent") as SpriteComponent;
            sprite.position = hexSprite.position;

        }

        //public void PathInput()
        //{
        //    List <HexComponent> pathCue = new List<HexComponent>();
        //    while shift is held down
        //    {
        //        ghostHex = startingHex
        //        if (GetHex(mouseCurrentHex) is not in pathCue && is next to ghostHex)
        //        {
        //            add hex to list
        //            ghostHex = mouseCurrentHex
        //        }
        //        if (GetHex(mouseCurrentHex) is in pathCue)
        //        {
        //            remove hex from cue and all others added after it
        //            ghostHex = hex last in cue
        //        }
        //    }

        //    for (loop through pathCue)
        //    {
        //        move unit to hex in cue
        //        updateallyvisibility
        //        wait miliseconds
        //    }
        //}
    }
}
