using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;
using EntityEngine.Components.Sprites;
using EntityEngine;

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


        //TODO: Somehow set this
        Visibility visibility;
        public void SetVisbility(Visibility myVis)
        {
            visibility = myVis;
            AnimatedSpriteComponent sprite = _parent.GetDrawable("AnimatedSpriteComponent") as AnimatedSpriteComponent;

            if (isAlly)
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

        bool isAlly;
        public void SetAlly(bool myTruth)
        {
            isAlly = myTruth;
        }
        public bool GetAlly()
        {
            return isAlly;
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

        UnitData unitData;
        public UnitData GetUnitData()
        {
            return unitData;
        }
        public void SetUnitData(UnitData u) { unitData = u; }

        public UnitComponent(bool myIsAlly, int mySightRadius, HexComponent myHex, bool mySelectable, UnitData unitData)
        {
            hex = myHex;
            this.name = "UnitComponent";
            this.unitData = unitData;
            sightRadius = mySightRadius;
            isAlly = myIsAlly;
        }

        //public override void Initialize()
        //{
        //    AnimatedSpriteComponent sprite = _parent.GetDrawable("AnimatedSpriteComponent") as AnimatedSpriteComponent;
        //    sprite._visible = false;
        //    base.Initialize();
        //}

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
                        if (hex.ne != null)
                        {
                            if (!hex.ne.HasUnit())
                            {
                                hex.ne.SetUnit(this);
                                SetHex(hex.ne);
                                hex.RemoveUnit();
                            }
                        }
                    }
                    break;
                case Orient.se:
                    if (hex.se != null)
                    {
                        if (hex.se != null)
                        {
                            if (!hex.se.HasUnit())
                            {
                                hex.se.SetUnit(this);
                                SetHex(hex.se);
                                hex.RemoveUnit();
                            }
                        }
                    }
                    break;
                case Orient.s:
                    if (hex.s != null)
                    {
                        if (hex.s != null)
                        {
                            if (!hex.s.HasUnit())
                            {
                                hex.s.SetUnit(this);
                                SetHex(hex.s);
                                hex.RemoveUnit();
                            }
                        }
                    }
                    break;
                case Orient.sw:
                    if (hex.sw != null)
                    {
                        if (hex.sw != null)
                        {
                            if (!hex.sw.HasUnit())
                            {
                                hex.sw.SetUnit(this);
                                SetHex(hex.sw);
                                hex.RemoveUnit();
                            }
                        }
                    }
                    break;
                case Orient.nw:
                    if (hex.nw != null)
                    {
                        if (hex.nw != null)
                        {
                            if (!hex.nw.HasUnit())
                            {
                                hex.nw.SetUnit(this);
                                SetHex(hex.nw);
                                hex.RemoveUnit();
                            }
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
    }
}
