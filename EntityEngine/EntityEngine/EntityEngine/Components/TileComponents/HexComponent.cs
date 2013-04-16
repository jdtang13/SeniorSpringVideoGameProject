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
    public class HexComponent : Component
    {
        Vector2 coordPosition;
        public Vector2 getCoordPosition()
        {
            return coordPosition;
        }

        public HexComponent n, ne, se, s, sw, nw;
        public HexComponent GetAdjacent(Orient myOar)
        {
            switch (myOar)
            {
                case Orient.n:
                    return n;
                case Orient.ne:
                    return ne;
                case Orient.se:
                    return se;
                case Orient.s:
                    return s;
                case Orient.sw:
                    return sw;
                case Orient.nw:
                    return nw;

                default:
                    return this;

            }
        }

        //As there can only be one unit per tile, there is but one unit var
        UnitComponent unit;
        public UnitComponent GetUnit()
        {
            return unit;
        }
        public void SetUnit(UnitComponent myUnit)
        {
            unit = myUnit;   
        }
        public void RemoveUnit()
        {
            unit = null;
        }
        public bool HasUnit()
        {
            return (unit!=null);
        }

        Visibility visibility;
        public Visibility GetVisibility()
        {
            return visibility;
        }
        public void SetVisibility(Visibility myVis)
        {
            visibility = myVis;

            Entity hexEntity = _parent;
            SpriteComponent sprite = hexEntity.GetDrawable("SpriteComponent") as SpriteComponent;

            if (HasUnit())
            {
                unit.SetVisbility(visibility);
            }

            for (int p = 0; p < terrainList.Count; p++)
            {
                terrainList[p].SetVisbility(visibility);
            }

            if (myVis == Visibility.Visible)
            {
                sprite.setColor(Color.White);
                sprite._visible = true;
            }

            if (myVis == Visibility.Explored)
            {
                sprite.setColor(Color.SlateGray);
                sprite._visible = true;
            }

            if (myVis == Visibility.Unexplored)
            {
                sprite.setColor(Color.White);
                sprite._visible = false;
            }
        }

        bool inQueue;
        public bool GetInQueue()
        {
            return inQueue;
        }
        public void SetInQueue(bool myTruth)
        {
            inQueue = myTruth;

            Entity hexEntity = _parent;
            SpriteComponent sprite = hexEntity.GetDrawable("SpriteComponent") as SpriteComponent;

            foreach (TerrainComponent terrain in terrainList)
            {
                terrain.SetInQueue(myTruth);
            }

            if (myTruth == true)
            {
                sprite.setColor(Color.Green);
            }
            if (myTruth == false)
            {
                sprite.setColor(Color.White);
            }
        }

        List<TerrainComponent> terrainList = new List<TerrainComponent>();
        public void AddTerrain(TerrainComponent myTerrain)
        {
            terrainList.Add(myTerrain);
        }
        public void RemoveTerrain(TerrainComponent myTerrain)
        {
            terrainList.Remove(myTerrain);
        }
        public List<TerrainComponent> GetTerrain()
        {
            return terrainList;
        }

        public Boolean ContainsImpassable()
        {
            for (int p = 0; p < terrainList.Count ; p++)
            {
                if (terrainList[p].GetImpassable() == true)
                {
                    return true;
                }
            }
            return false;
        }

        public HexComponent(Vector2 myCoordPosition) 
        {
            this.name = "HexComponent";
            coordPosition = myCoordPosition;
        }

        public void SetAdjacent(HexComponent N, HexComponent NE, HexComponent SE, HexComponent S, HexComponent SW, HexComponent NW)
        {
            n = N; ne = NE; se = SE; s = S; sw = SW; nw = NW;
        }
    }
}
