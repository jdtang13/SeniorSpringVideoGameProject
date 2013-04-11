using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.Sprites;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Input;

namespace EntityEngine.Components.TileComponents
{
    public class BoardComponent : UpdateableComponent
    {
        //Use this component to make an entity into a board. This component handles the creation of the other hex entities.

        List<HexComponent> oldVisible;
        List<HexComponent> newVisible;

        Vector2 gridSize;
        public Vector2 GetDimenions()
        {
            return gridSize;
        }

        Texture2D gridTexture;
        SpriteFont gridFont;

        Vector2 mouseCurrentHex;

        Entity[,] hexEntityGrid;

        UnitComponent selectedUnit;
        public void SetSelectedUnit(UnitComponent myUnit)
        {
            selectedUnit = myUnit;
        }

        public List<UnitComponent> alliedUnitList = new List<UnitComponent>();
        List<UnitComponent> nonAlliedUnitList = new List<UnitComponent>();

        //You must handle nulls for this dictionary
        Dictionary<Vector2, HexComponent> HexDictionary = new Dictionary<Vector2, HexComponent>();
        public HexComponent getHex(Vector2 myVec)
        {
            if (this.HexDictionary.ContainsKey(myVec))
            {
                return HexDictionary[myVec];
            }
            else
            {
                return null;
            }
        }

        Dictionary<Vector2, Entity> HexEntityDictionary = new Dictionary<Vector2, Entity>();
        public Entity getHexEntity(Vector2 myVec)
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

        //You must pass in the texture of the grids, the font for debugging, and the dimensions of the grid
        public BoardComponent(Entity myParent, Texture2D myTexture, SpriteFont myFont, Vector2 mySize)
            : base(myParent)
        {
            this.name = "BoardComponent";

            gridSize = mySize;
            gridTexture = myTexture;
            gridFont = myFont;


        }

        public BoardComponent(Texture2D myTexture, SpriteFont myFont, Vector2 mySize)
        {
            this.name = "BoardComponent";

            gridSize = mySize;
            gridTexture = myTexture;
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
                    HexDictionary.Add(coordPosition, hexComp);
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

                    SpriteComponent hexSprite = new SpriteComponent( true, screenPosition, gridTexture);
                    hexEntity.AddComponent(hexSprite);
                    hexEntity.AddComponent(new CameraComponent(screenPosition));


                    EntityManager.AddEntity(hexEntity);

                    getHex(coordPosition).SetVisibility(Visibility.Unexplored);


                    //Adding text to label the coordinates of the hex entity
                    Vector2 debugTextPosition = new Vector2(hexSprite.getCenterPosition().X, hexSprite.getCenterPosition().Y);
                    hexEntity.AddComponent(new TextSpriteComponent(false, coordPosition.X.ToString() + "," + coordPosition.Y.ToString(), Color.Black, debugTextPosition, gridFont));
                }
            }

            //Giving all the hex's their adjacents
            foreach (KeyValuePair<Vector2, HexComponent> entry in HexDictionary)
            {
                HexComponent hex = entry.Value;
                Vector2 coords = hex.getCoordPosition();

                //Setting up everyones adjacent, if it's null, it doesnt exist
                HexComponent n, ne, se, sw, s, nw;
                n = null; ne = null; se = null; s = null; sw = null; nw = null;

                if (getHex(new Vector2(coords.X, coords.Y - 1)) != null)
                    n = getHex(new Vector2(coords.X, coords.Y - 1));

                if (getHex(new Vector2(coords.X + 1, coords.Y)) != null)
                    ne = getHex(new Vector2(coords.X + 1, coords.Y));

                if (getHex(new Vector2(coords.X + 1, coords.Y + 1)) != null)
                    se = getHex(new Vector2(coords.X + 1, coords.Y + 1));

                if (getHex(new Vector2(coords.X, coords.Y + 1)) != null)
                    s = getHex(new Vector2(coords.X, coords.Y + 1));

                if (getHex(new Vector2(coords.X - 1, coords.Y)) != null)
                    sw = getHex(new Vector2(coords.X - 1, coords.Y));

                if (getHex(new Vector2(coords.X - 1, coords.Y - 1)) != null)
                    nw = getHex(new Vector2(coords.X - 1, coords.Y - 1));

                hex.SetAdjacent(n, ne, se, s, sw, nw);
            }
        }
        public void CreateUnit(bool myIsAlly, int mySightRadius, Vector2 myCoordinate, Texture2D myTexture, int mySpriteFrameWidth, int mySpriteFrameHeight)
        {
            HexComponent hexComp = getHex(myCoordinate);

            if (hexComp.GetUnit() == null)
            {
                Entity unitEntity = new Entity(5, State.ScreenState.SKIRMISH);

                SpriteComponent hexSprite = getHex(myCoordinate)._parent.GetDrawable("SpriteComponent") as SpriteComponent;
                unitEntity.AddComponent(new AnimatedSpriteComponent(true, hexSprite.getCenterPosition(), myTexture, 75f, mySpriteFrameWidth, mySpriteFrameHeight));
                unitEntity.AddComponent(new CameraComponent( hexSprite.getCenterPosition()));

                UnitComponent unitComp = new UnitComponent( myIsAlly, mySightRadius, getHex(myCoordinate), true);
                unitEntity.AddComponent(unitComp);

                getHex(myCoordinate).SetUnit(unitComp);

                EntityManager.AddEntity(unitEntity);

                if (myIsAlly)
                {
                    alliedUnitList.Add(unitComp);
                }
                else
                {
                    nonAlliedUnitList.Add(unitComp);
                }

                hexComp.SetUnit(unitComp);
            }
            else
            {
                throw new Exception("There is already a unit where you are trying to create one.");
            }
        }
        public void CreateTerrain(Vector2 myCoordinate, Texture2D myTexture, bool myImpassable)
        {
            HexComponent hexComp = getHex(myCoordinate);

            Entity terrainEntity = new Entity(4, State.ScreenState.SKIRMISH);

            SpriteComponent hexSprite = getHex(myCoordinate)._parent.GetDrawable("SpriteComponent") as SpriteComponent;
            terrainEntity.AddComponent(new SpriteComponent(true, hexSprite.getCenterPosition(), myTexture));
            terrainEntity.AddComponent(new CameraComponent(hexSprite.getCenterPosition()));

            TerrainComponent terrainComp = new TerrainComponent(hexComp, myImpassable);
            terrainEntity.AddComponent(terrainComp);

            EntityManager.AddEntity(terrainEntity);

            hexComp.AddTerrain(terrainComp);
        }

        //Returns the hex component of the hex entity that is under the mouse
        public HexComponent GetCurrentHexAtMouse()
        {
            float distance = 0;
            Vector2 mousePosition = InputState.getMousePosition();

            Vector2 mouseHexCoordinate;
            mouseHexCoordinate.X = mousePosition.X / (gridTexture.Width * 3f / 4f);
            mouseHexCoordinate.Y = roundDown(mousePosition.Y / gridTexture.Height) + roundDown(mouseHexCoordinate.X) / 2f;

            Vector2 mouseRoundedHexCoordinate = new Vector2(roundDown(mouseHexCoordinate.X), roundDown(mouseHexCoordinate.Y));

            if (getHex(mouseRoundedHexCoordinate) != null)
            {
                SpriteComponent centerSprite = getHex(mouseRoundedHexCoordinate)._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                distance = Vector2.Distance(mousePosition, centerSprite.getCenterPosition());

                if (Vector2.Distance(mousePosition, centerSprite.getCenterPosition()) < gridTexture.Height / 2f)
                {
                    mouseCurrentHex = getHex(mouseRoundedHexCoordinate).getCoordPosition();
                }
                else
                {
                    if (getHex(mouseRoundedHexCoordinate).n != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).n._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).n.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).ne != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).ne._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).ne.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).se != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).se._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).se.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).s != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).s._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).s.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).sw != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).sw._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).sw.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).nw != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).nw._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).nw.getCoordPosition();
                        }
                    }
                }
            }
            return getHex(mouseCurrentHex);
        }

        //Some rounding functions, nothing to see here
        public float roundUp(float myNum)
        {
            float rounded = (int)myNum + 1;

            return rounded;
        }
        public float roundDown(float myNum)
        {
            float rounded = (int)myNum;

            return rounded;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HexComponent currentHex = GetCurrentHexAtMouse();

        }

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

                    if (getHex(ghostCoord) != null)
                    {
                        ring.Add(getHex(ghostCoord));
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

            for (int r = 0; r <= myUnit.GetSightRadius(); r++)
            {
                allRings.AddRange(GetRing(myUnit, r));
            }

            allRings.Add(getHex(myUnit.GetHex().getCoordPosition()));

            return allRings;
        }


        //IMPORTANT: Call this function every time anyone on your team moves
        public void UpdateVisibilityAllies()
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
}
