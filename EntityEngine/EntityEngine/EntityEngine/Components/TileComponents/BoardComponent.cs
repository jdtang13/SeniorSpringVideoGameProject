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

        HexComponent oldPlayerPosition;
        HexComponent newPlayerPosition;

        List<HexComponent> oldVisible;
        List<HexComponent> newVisible;

        Vector2 gridSize;
        Texture2D gridTexture;
        SpriteFont gridFont;

        Vector2 mouseCurrentHex;

        Entity[,] hexEntityGrid;

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

        Entity positionEntity;

        //You must pass in the texture of the grids, the font for debugging, and the dimensions of the grid
        public BoardComponent(Entity myParent, Texture2D myTexture, SpriteFont myFont, Vector2 mySize)
            : base(myParent)
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

            oldPlayerPosition = null;
            newPlayerPosition = getHex(mouseCurrentHex);

            oldVisible = new List<HexComponent>();
            newVisible = GetAllRings(3);

            //getHex(new Vector2(5, 5)).SetFog(Visibility.Unexplored);

            base.Initialize();
        }

        void createGrid()
        {
            for (int x = 0; x < gridSize.X; x++)
            {
                for (int y = 0; y < gridSize.Y; y++)
                {
                    //Each hex will be an entity
                    Entity hexEntity = new Entity(0);
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

                    HexComponent hexComp = new HexComponent(hexEntity, coordPosition);
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

                    SpriteComponent hexSprite = new SpriteComponent(hexEntity, true, screenPosition, gridTexture);
                    hexEntity.AddComponent(hexSprite);
                    hexEntity.AddComponent(new CameraComponent(hexEntity, screenPosition));


                    EntityManager.AddEntity(hexEntity);

                    getHex(coordPosition).SetFog(Visibility.Unexplored);


                    //Adding text to label the coordinates of the hex entity
                    Vector2 debugTextPosition = new Vector2(hexSprite.getCenterPosition().X, hexSprite.getCenterPosition().Y);
                    hexEntity.AddComponent(new TextSpriteComponent(hexEntity, false, coordPosition.X.ToString() + "," + coordPosition.Y.ToString(), Color.Black, debugTextPosition, gridFont));
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

                hex.setAdjacent(n, ne, se, s, sw, nw);
            }
        }

        //Returns the hex component of the hex entity that is under the mouse
        public HexComponent getCurrentHexAtMouse()
        {
            float distance = 0;
            Vector2 mousePosition = InputState.getMousePosition();

            Vector2 mouseHexCoordinate;
            mouseHexCoordinate.X = mousePosition.X / (gridTexture.Width * 3f / 4f);
            mouseHexCoordinate.Y = roundDown(mousePosition.Y / gridTexture.Height) + roundDown(mouseHexCoordinate.X) / 2f;

            Vector2 mouseRoundedHexCoordinate = new Vector2(roundDown(mouseHexCoordinate.X), roundDown(mouseHexCoordinate.Y));

            if (getHex(mouseRoundedHexCoordinate) != null)
            {
                SpriteComponent centerSprite = getHex(mouseRoundedHexCoordinate)._parent.getDrawable("SpriteComponent") as SpriteComponent;

                distance = Vector2.Distance(mousePosition, centerSprite.getCenterPosition());

                if (Vector2.Distance(mousePosition, centerSprite.getCenterPosition()) < gridTexture.Height / 2f)
                {
                    mouseCurrentHex = getHex(mouseRoundedHexCoordinate).getCoordPosition();
                }
                else
                {
                    if (getHex(mouseRoundedHexCoordinate).n != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).n._parent.getDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).n.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).ne != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).ne._parent.getDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).ne.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).se != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).se._parent.getDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).se.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).s != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).s._parent.getDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).s.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).sw != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).sw._parent.getDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).sw.getCoordPosition();
                        }
                    }
                    if (getHex(mouseRoundedHexCoordinate).nw != null)
                    {
                        SpriteComponent sprite = getHex(mouseRoundedHexCoordinate).nw._parent.getDrawable("SpriteComponent") as SpriteComponent;

                        if (Vector2.Distance(mousePosition, sprite.getCenterPosition()) < gridTexture.Height / 2f)
                        {
                            mouseCurrentHex = getHex(mouseRoundedHexCoordinate).nw.getCoordPosition();
                        }
                    }
                }
            }
            return getHex(mouseCurrentHex);
        }        
        
        //returns ring of hexes distance radius away from mouseCurrentHex


        public List<HexComponent> GetRing(int radius)
        {
            List<HexComponent> ring = new List<HexComponent>();
            Vector2 startCoord = new Vector2(mouseCurrentHex.X, mouseCurrentHex.Y - radius);
            Vector2 ghostCoord = startCoord;

            //                              N                  NE                 SE                S                  SW                   NW
            Vector2[] directions = { new Vector2(0, -1), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(-1, -1) };

            // 7 % 6 = orientation 1
            for (int o = 2; o <= 7; o++)
            {
                int times = radius;
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
        public List<HexComponent> GetAllRings(int radius)
        {
            List<HexComponent> allRings = new List<HexComponent>();

            for (int r = 0; r <= radius; r++)
            {
                allRings.AddRange(GetRing(r));
            }

            allRings.Add(getHex(mouseCurrentHex));

            return allRings;
        }

        public void CreateUnit(Vector2 myCoordinate, Texture2D myUnitTexture)
        {
            HexComponent hexComp = getHex(myCoordinate);

            if (hexComp.GetUnit() == null)
            {
                Entity unitEntity = new Entity(5);
                UnitComponent unitComp = new UnitComponent(_parent, getHex(myCoordinate), true);
                unitEntity.AddComponent(unitComp);
                SpriteComponent hexSprite = getHex(myCoordinate)._parent.getDrawable("SpriteComponent") as SpriteComponent;
                unitEntity.AddComponent(new AnimatedSpriteComponent(unitEntity, true, hexSprite.getCenterPosition(), myUnitTexture, 75f, 50, 50));
                unitEntity.AddComponent(new CameraComponent(unitEntity, hexSprite.getCenterPosition()));
                EntityManager.AddEntity(unitEntity);

                hexComp.SetUnit(unitComp);

            }
            else
            {
                throw new Exception("There is already a unit where you are trying to create one.");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HexComponent currentHex = getCurrentHexAtMouse();
            UpdateFog();
        }

        public void UpdateFog()
        {
            newPlayerPosition = getHex(mouseCurrentHex);

            newVisible = GetAllRings(3);

            if (new InputAction(MouseButton.left, false).Evaluate())
            {
                //for (int i = 0; i < newVisible.Count; i++)
                //{
                //    newVisible[i].SetFog(Visibility.Visible);
                //}

                //if (oldPlayerPosition != newPlayerPosition)
                //{
                //    for (int i = 0; i < oldVisible.Count; i++)
                //    {
                //        if (newVisible.Contains(oldVisible[i]) == false)
                //        {
                //            oldVisible[i].SetFog(Visibility.Explored);
                //        }
                //    }

                if (oldPlayerPosition != newPlayerPosition)
                {


                    for (int i = 0; i < oldVisible.Count; i++)
                    {
                        oldVisible[i].SetFog(Visibility.Explored);
                    }

                    for (int i = 0; i < newVisible.Count; i++)
                    {
                        newVisible[i].SetFog(Visibility.Visible);
                    }


                }
                oldPlayerPosition = newPlayerPosition;
                oldVisible = newVisible;
            }
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
    }
}
