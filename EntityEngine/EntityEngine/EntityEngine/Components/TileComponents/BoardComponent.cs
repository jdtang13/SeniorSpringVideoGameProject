﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.Sprites;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Input;
using EntityEngine.Stat_Attribute_Classes;

namespace EntityEngine.Components.TileComponents
{
    public class BoardComponent : Component
    {
        bool fogOfWarToggle = true;
        List<HexComponent> oldVisible;
        List<HexComponent> newVisible;

        Vector2 gridSize;
        public Vector2 GetDimenions()
        {
            return gridSize;
        }

        //If the maps works correctly why would we need this?
        Texture2D gridTexture;
        SpriteFont gridFont;

        Vector2 mouseCurrentHex;
        List<HexComponent> adjacentList = new List<HexComponent>();

        Entity[,] hexEntityGrid;

        public List<UnitComponent> alliedUnitList = new List<UnitComponent>();
        List<UnitComponent> nonAlliedUnitList = new List<UnitComponent>();

        //
        List<Vector2> alliedSpawnPoints = new List<Vector2>();
        public void AddAlliedSpawnPoint(Vector2 myVector)
        {
            alliedSpawnPoints.Add(myVector);
        }
        public Vector2 GetOneAlliedSpawnPoint(Random rand)
        {
            int chosenIndex = rand.Next(0, alliedSpawnPoints.Count);
            Vector2 spawn = alliedSpawnPoints[chosenIndex];
            alliedSpawnPoints.RemoveAt(chosenIndex);

            return spawn;
        }

        Dictionary<int,List<Vector2>> enemySpawnPoints = new Dictionary<int,List<Vector2>>();
        public Vector2 GetEnemySpawnPointForType(int myUnitNumber, Random myRand)
        {
            List<Vector2> listOfAvailableSpawnsForUnitType = enemySpawnPoints[myUnitNumber];
            int randomlySelectedSpawn = myRand.Next(0, listOfAvailableSpawnsForUnitType.Count);

            Vector2 spawnPoint = listOfAvailableSpawnsForUnitType[randomlySelectedSpawn];
            listOfAvailableSpawnsForUnitType.RemoveAt(randomlySelectedSpawn);

            return spawnPoint;
        }
        public void AddEnemySpawnPoint(int myUnitNumber, Vector2 myVector2)
        {
            if (!enemySpawnPoints.ContainsKey(myUnitNumber))
            {
                enemySpawnPoints.Add(myUnitNumber, new List<Vector2>());
            }
            enemySpawnPoints[myUnitNumber].Add(myVector2);
        }

        //You must handle nulls for this dictionary
        Dictionary<Vector2, HexComponent> hexDictionary = new Dictionary<Vector2, HexComponent>();
        public HexComponent GetHex(Vector2 myVec)
        {
            if (this.hexDictionary.ContainsKey(myVec))
            {
                return hexDictionary[myVec];
            }
            else
            {
                return null;
            }
        }
        public HexComponent GetHex(int x, int y)
        {
            return GetHex(new Vector2(x, y));
        }

        Dictionary<Vector2, Entity> HexEntityDictionary = new Dictionary<Vector2, Entity>();
        public Entity GetHexEntity(Vector2 myVec)
        {
            if (this.HexEntityDictionary.ContainsKey(myVec))
            {
                return HexEntityDictionary[myVec];
            }
            else
            {
                return null;
            }
        }

        public BoardComponent(Vector2 mySize,Texture2D myGridTexture,SpriteFont myFont)
        {
            this.name = "BoardComponent";
            gridTexture = myGridTexture;
            gridSize = mySize;
            gridFont = myFont;
        }

        public override void Initialize()
        {
            hexEntityGrid = new Entity[(int)gridSize.X, (int)gridSize.Y];
            createGrid();

            oldVisible = new List<HexComponent>();
            newVisible = new List<HexComponent>();

            base.Initialize();
        }

        // TODO: broken. have lionel or oliver fix "screen coordinates of hex"
        public Vector2 screenCoordinatesOfHex(Vector2 pos) {
            return screenCoordinatesOfHex((int)pos.X, (int)pos.Y);
        }
        public Vector2 screenCoordinatesOfHex(int x, int y)
        {
            SpriteComponent sprite = GetHex(new Vector2(x,y))._parent.GetDrawable("SpriteComponent") as SpriteComponent;

            return sprite.getCenterPosition();

            /*Vector2 screenPosition;

            if (x % 2 == 0)
            {
                screenPosition.Y = y * gridTexture.Height + gridTexture.Height / 2f;
            }
            else
            {
                screenPosition.Y = y * gridTexture.Height + gridTexture.Height / 2f + gridTexture.Height / 2f;
            }
            screenPosition.X = x * (gridTexture.Width / 4f * 3f) + gridTexture.Width / 2f;

            return screenPosition;*/
        }

        void createGrid()
        {
            for (int x = 0; x < gridSize.X; x++)
            {
                for (int y = 0; y < gridSize.Y; y++)
                {
                    //Each hex will be an entity
                    Entity hexEntity = new Entity(0, State.ScreenState.SKIRMISH);
                    hexEntityGrid[x, y] = hexEntity;


                    //Creating the hex comp for the hex entity
                    Vector2 coordPosition = new Vector2(x, y);
                    if (coordPosition.X % 2 == 0)
                    {
                        coordPosition.Y = coordPosition.X / 2f + y;
                    }
                    else
                    {
                        coordPosition.Y = (coordPosition.X + 1f) / 2f + y;
                    }

                    HexComponent hexComp = new HexComponent(coordPosition);
                    hexEntity.AddComponent(hexComp);
                    hexDictionary.Add(coordPosition, hexComp);
                    HexEntityDictionary.Add(coordPosition, hexEntity);

                    //Creating the sprite for the hex entity
                    Vector2 screenPosition;
                    if (x % 2 == 0)
                    {
                        screenPosition.Y = y * gridTexture.Height + gridTexture.Height / 2f;
                    }
                    else
                    {
                        screenPosition.Y = y * gridTexture.Height + gridTexture.Height / 2f + gridTexture.Height / 2f;
                    }
                    screenPosition.X = x * (gridTexture.Width / 4f * 3f) + gridTexture.Width / 2f;

                    SpriteComponent hexSprite = new SpriteComponent(true, screenPosition, gridTexture);
                    hexEntity.AddComponent(hexSprite);

                    EntityManager.AddEntity(hexEntity);

                    GetHex(coordPosition).SetVisibility(Visibility.Unexplored);
                }
            }

            //Giving all the hex's their adjacents
            foreach (KeyValuePair<Vector2, HexComponent> entry in hexDictionary)
            {
                HexComponent hex = entry.Value;
                Vector2 coords = hex.getCoordPosition();

                //Setting up everyones adjacent, if it's null, it doesnt exist
                HexComponent n, ne, se, sw, s, nw;
                n = null; ne = null; se = null; s = null; sw = null; nw = null;

                if (GetHex(new Vector2(coords.X, coords.Y - 1)) != null)
                    n = GetHex(new Vector2(coords.X, coords.Y - 1));

                if (GetHex(new Vector2(coords.X + 1, coords.Y)) != null)
                    ne = GetHex(new Vector2(coords.X + 1, coords.Y));

                if (GetHex(new Vector2(coords.X + 1, coords.Y + 1)) != null)
                    se = GetHex(new Vector2(coords.X + 1, coords.Y + 1));

                if (GetHex(new Vector2(coords.X, coords.Y + 1)) != null)
                    s = GetHex(new Vector2(coords.X, coords.Y + 1));

                if (GetHex(new Vector2(coords.X - 1, coords.Y)) != null)
                    sw = GetHex(new Vector2(coords.X - 1, coords.Y));

                if (GetHex(new Vector2(coords.X - 1, coords.Y - 1)) != null)
                    nw = GetHex(new Vector2(coords.X - 1, coords.Y - 1));

                hex.SetAdjacent(n, ne, se, s, sw, nw);
            }
        }

        public List<HexComponent> GetAdjacentList(HexComponent hex)
        {
            adjacentList = new List<HexComponent>();

            adjacentList.Add(hex.n);
            adjacentList.Add(hex.ne);
            adjacentList.Add(hex.nw);
            adjacentList.Add(hex.s);
            adjacentList.Add(hex.sw);
            adjacentList.Add(hex.se);

            return adjacentList;
        }

        //public void CreateUnitWithData(Vector2 myCoordinate,Texture2D myTexture, int

        //public void CreateUnit(Role myRole,UnitData myUnitData, Texture2DFramed myFramedTexture, Vector2 myCoordinate)
        //{
        //    HexComponent hexComp = GetHex(myCoordinate);

        //    if (hexComp.GetUnit() == null)
        //    {
        //        SpriteComponent hexSprite = GetHex(myCoordinate)._parent.GetDrawable("SpriteComponent") as SpriteComponent;

        //        Entity unitEntity = new Entity(15, State.ScreenState.SKIRMISH);           

        //        AnimatedSpriteComponent unitSprite = new AnimatedSpriteComponent(true, hexSprite.getCenterPosition(), myFramedTexture);
        //        unitEntity.AddComponent(unitSprite);

        //        UnitComponent unitComp = new UnitComponent(isAlly, mySightRadius, GetHex(myCoordinate), true, null);
        //        unitEntity.AddComponent(unitComp);

        //        UnitDataCom

        //        GetHex(myCoordinate).SetUnit(unitComp);
        //        EntityManager.AddEntity(unitEntity);

        //        if (isAlly)
        //        {
        //            alliedUnitList.Add(unitComp);
        //        }
        //        else
        //        {
        //            nonAlliedUnitList.Add(unitComp);
        //        }

        //        hexComp.SetUnit(unitComp);
        //        UpdateVisibilityAllies();
        //    }
        //    else
        //    {
        //        throw new Exception("There is already a unit where you are trying to create one.");
        //    }
        //}
        
        //TODO: Add row layer support
        public void AddTerrain(Vector2 myCoordinate,int myLayer, TerrainPackage myTerrain)
        {
            HexComponent hexComponent = GetHex(myCoordinate);
            SpriteComponent hexSprite = hexComponent._parent.GetDrawable("SpriteComponent") as SpriteComponent;

            Entity terrainEntity = new Entity(4+myLayer, State.ScreenState.SKIRMISH);
            terrainEntity.AddComponent(new SpriteComponent(true, hexSprite.getCenterPosition(), myTerrain.GetTexture()));

            TerrainComponent terrComp = new TerrainComponent(hexComponent, myTerrain.GetTexture(), myTerrain.GetImpassable());
            terrainEntity.AddComponent(terrComp);
            hexComponent.AddTerrain(terrComp);

            EntityManager.AddEntity(terrainEntity);
        }

        //Returns the hex component of the hex entity that is under the mouse
        public HexComponent GetMouseHex()
        {
            float distance = 0;
            Vector2 mousePosition = InputState.GetMouseIngamePosition();

            Vector2 mouseHexCoordinate;

            mouseHexCoordinate.X = mousePosition.X / (gridTexture.Width * 3f / 4f);
            mouseHexCoordinate.Y = roundDown(mousePosition.Y / gridTexture.Height) + roundDown(mouseHexCoordinate.X) / 2f;

            Vector2 mouseRoundedHexCoordinate = new Vector2(roundDown(mouseHexCoordinate.X), roundDown(mouseHexCoordinate.Y));

            if (GetHex(mouseRoundedHexCoordinate) != null)
            {
                SpriteComponent centerSprite = GetHex(mouseRoundedHexCoordinate)._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                distance = Vector2.Distance(mousePosition, centerSprite.getCenterPosition());

                if (Vector2.Distance(mousePosition, centerSprite.getCenterPosition()) < gridTexture.Height / 2f)
                {
                    mouseCurrentHex = GetHex(mouseRoundedHexCoordinate).getCoordPosition();
                }
                else
                {
                    if (GetHex(mouseRoundedHexCoordinate).n != null)
                    {
                        SpriteComponent sprite = GetHex(mouseRoundedHexCoordinate).n._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = GetHex(mouseRoundedHexCoordinate).n.getCoordPosition();
                        }
                    }
                    if (GetHex(mouseRoundedHexCoordinate).ne != null)
                    {
                        SpriteComponent sprite = GetHex(mouseRoundedHexCoordinate).ne._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = GetHex(mouseRoundedHexCoordinate).ne.getCoordPosition();
                        }
                    }
                    if (GetHex(mouseRoundedHexCoordinate).se != null)
                    {
                        SpriteComponent sprite = GetHex(mouseRoundedHexCoordinate).se._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = GetHex(mouseRoundedHexCoordinate).se.getCoordPosition();
                        }
                    }
                    if (GetHex(mouseRoundedHexCoordinate).s != null)
                    {
                        SpriteComponent sprite = GetHex(mouseRoundedHexCoordinate).s._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = GetHex(mouseRoundedHexCoordinate).s.getCoordPosition();
                        }
                    }
                    if (GetHex(mouseRoundedHexCoordinate).sw != null)
                    {
                        SpriteComponent sprite = GetHex(mouseRoundedHexCoordinate).sw._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = GetHex(mouseRoundedHexCoordinate).sw.getCoordPosition();
                        }
                    }
                    if (GetHex(mouseRoundedHexCoordinate).nw != null)
                    {
                        SpriteComponent sprite = GetHex(mouseRoundedHexCoordinate).nw._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = GetHex(mouseRoundedHexCoordinate).nw.getCoordPosition();
                        }
                    }
                }
            }
            return GetHex(mouseCurrentHex);
        }

        static float roundUp(float myNum)
        {
            float rounded = (int)myNum + 1;

            return rounded;
        }
        static float roundDown(float myNum)
        {
            float rounded = (int)myNum;

            return rounded;
        }

        //#region Visibility
        //returns ring of hexes distance radius away from mouseCurrentHex
        public List<HexComponent> GetRing(UnitComponent myUnit, int myRadius)
        {
            List<HexComponent> ring = new List<HexComponent>();

            HexComponent unitHex = myUnit.GetHex();

            Vector2 startCoord = new Vector2(unitHex.getCoordPosition().X, unitHex.getCoordPosition().Y - myRadius);
            Vector2 ghostCoord = startCoord;

            //                              N                  NE                 SE                S                  SW                   NW
            Vector2[] directions = { new Vector2(0, -1), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(-1, -1) };

            // 7 % 6 = orientation 1
            for (int o = 2; o <= 7; o++)
            {
                int times = myRadius;
                while (times > 0) // > or >=
                {
                    ghostCoord = ghostCoord + directions[o % 6];

                    if (GetHex(ghostCoord) != null)
                    {
                        ring.Add(GetHex(ghostCoord));
                    }
                    times--;
                }
            }
            return ring;
        }

        //returns all rings of hexes distance radius or less away from mouseCurrentHex
        public List<HexComponent> GetAllRings(UnitComponent myUnit)
        {
            List<HexComponent> allRings = new List<HexComponent>();

            UnitDataComponent unitData = myUnit._parent.GetComponent("UnitDataComponent") as UnitDataComponent;

            for (int r = 0; r <= unitData.GetSightRadius(); r++)
            {
                allRings.AddRange(GetRing(myUnit, r));
            }

            allRings.Add(GetHex(myUnit.GetHex().getCoordPosition()));

            return allRings;
        }

        //IMPORTANT: Call this function every time anyone on your team moves
        public void UpdateVisibilityAllies()
        {
            if (fogOfWarToggle)
            {
                for (int u = 0; u < oldVisible.Count; u++)
                {
                    oldVisible[u].SetVisibility(Visibility.Explored);
                }

                newVisible.Clear();

                for (int p = 0; p < alliedUnitList.Count; p++)
                {
                    newVisible.AddRange(GetAllRings(alliedUnitList[p]));
                }

                for (int i = 0; i < newVisible.Count; i++)
                {
                    newVisible[i].SetVisibility(Visibility.Visible);
                }

                oldVisible = newVisible;
            }
        }

        public void ToggleFogofWar(bool myTruth)
        {
            //True means fog of war is on, false means fog of war is disabled
            if (myTruth == false)
            {
                foreach (KeyValuePair<Vector2, HexComponent> hex in hexDictionary)
                {
                    hex.Value.SetVisibility(Visibility.Visible);
                }
            }

            fogOfWarToggle = myTruth;
        }

    }
}
