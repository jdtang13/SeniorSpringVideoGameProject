using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using EntityEngine;
using EntityEngine.Components.TileComponents;
using EntityEngine.Components.Sprites;
using EntityEngine.Components.Component_Parents;
using EntityEngine.Input;
using EntityEngine.Components.World_Map;

namespace SeniorProjectGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        int screenWidth = 1280;
        int screenHeight = 680;
        SpriteBatch spriteBatch;

        //FileStream worldMapTxt, worldMapBin;

        //Dictionary<string, FileStream> hexMapTxts = new Dictionary<string, FileStream>();
        //Dictionary<string, FileStream> hexMapBinaries = new Dictionary<string, FileStream>();
        //public FileStream GetLevelBinary(string myLevelID)
        //{
        //    return new FileStream();
        //}
        //public FileStream GetLevelTxt(string myLevelID)
        //{
        //    return new FileStream();
        //}

        Dictionary<string, BoardComponent> hexMapDictionary = new Dictionary<string, BoardComponent>();
        public void AddHexMap(string myID, BoardComponent myBoard)
        {
            hexMapDictionary.Add(myID, myBoard);
        }
        public BoardComponent GetHexMap(string myID)
        {
            if(hexMapDictionary.ContainsKey(myID))
            {
                return hexMapDictionary[myID];
            }
            else
            {
                throw new Exception("That map doesn't exist");
            }
        }

        Dictionary<string, TerrainComponent> terrainDictionary = new Dictionary<string, TerrainComponent>();
        public void AddTerrainComponent(string myKey, TerrainComponent myTerrainComponent)
        {
            terrainDictionary.Add(myKey, myTerrainComponent);
        }
        public TerrainComponent GetTerrainFromChar(string myKey)
        {
            if (terrainDictionary.ContainsKey(myKey))
            {
                return terrainDictionary[myKey];
            }
            else
            {
                throw new Exception("That character doesn't exist in the terrain dictionary");
            }
        }

        Random rand = new Random();

        Texture2D worldMapTexture, nodeTexture, pointerTexture;

        Texture2D hexBaseTexture, hexDirtTexture, hexGrassTexture;

        Texture2D unitTexture;

        SoundEffect selectSound;

        SpriteFont font;

        InputAction singleLeftClick, singleRightClick, singleMiddleClick;
        InputAction wClick, aClick, sClick, dClick, escapeClick;

        WorldMapComponent worldMapComponent;
        BoardComponent boardComp;

        float framesPerSecond = 60;
        float numberOfFrames;
        TimeSpan elapsedTime;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //1280x720
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            LoadContent();

            PopulateTerrainDictionary();

            escapeClick = new InputAction(new Keys[] { Keys.Escape }, true);
            singleLeftClick = new InputAction(MouseButton.left, true);
            singleRightClick = new InputAction(MouseButton.right, true);
            singleMiddleClick = new InputAction(MouseButton.middle, true);

            InitializeState();
            //ConvertTxtToBinSave("C:\\Users\\Oliver\\Desktop\\WorldMap.txt");
            ReadWorldMapBin();

            InitializeHexMap();

            base.Initialize();
        }

        void InitializeState()
        {
            State.Initialize();

            Dictionary<String, Role> classes = new Dictionary<String, Role>();
            Role lordClass = new Role(3,2,4,4,2,0,3,
                .1f,.2f,.2f,.3f,.4f,.1f,.3f,
                4,2,3,5,3,2,4,
                true, false, false, false, false, false, false, 3);

            classes["lord"] = lordClass;

            UnitData myUnitData = new UnitData(5,5,4,3,7,2,5,
                .6f,.5f,.4f,.5f,.7f,.3f,.6f,
                40,40,40,35,45,35,45, "myUnit",
                Alignment.PLAYER, classes["lord"], 1, 0);

            Entity unitEntity = new Entity(5, State.ScreenState.SKIRMISH);
            UnitComponent myUnitComponent = new UnitComponent(true, 4,
                boardComp.getHex(new Vector2(3, 3)), true, myUnitData);

            //unitEntity.AddComponent(new AnimatedSpriteComponent(unitEntity, false, 
            //    new Vector2(3,3), unitTexture, 4, 50, 50));

            Texture2D debugTexture = Content.Load<Texture2D>("Graphics\\UnitTextures\\debugUnitOneFrame");

            unitEntity.AddComponent(new SpriteComponent(false, boardComp.getHex(new Vector2(3,3))
                ._parent.GetDrawable("SpriteComponent").position, debugTexture));

            unitEntity.AddComponent(myUnitComponent);
            unitEntity.AddComponent(new CameraComponent(new Vector2(3, 3)));

            boardComp.getHex(3, 3).SetUnit(myUnitComponent);
            //boardComp.CreateUnit();
            
            EntityManager.AddEntity(unitEntity);
        }

        void PopulateTerrainDictionary()
        {
            //AddTerrainChar("G", hexGrassTexture);
            //AddTerrainChar("D", hexDirtTexture);
        }

        void InitializeHexMap()
        {
            CreateBoard(new Vector2(27, 12));

            //boardComp.CreateUnit(true, 2, new Vector2(5, 5), unitTexture, 50, 50);
            //boardComp.CreateUnit(true, 2, new Vector2(16, 13), unitTexture, 50, 50);
            //boardComp.CreateUnit(true, 2, new Vector2(10, 9), unitTexture, 50, 50);
        }

        //void SaveMap(string myMapName, Entity myEntity)//THIS OVERWRITES WHAT'S THERE
        //{
        //    FileStream file = new FileStream("Content/" + myMapName + ".bin", System.IO.FileMode.Create);

        //    using (BinaryWriter bin = new BinaryWriter(file))
        //    {
        //        //TODO: SAVE THE BOARD'S STATE W/O UNITS

        //        //How to read this file:
        //        //Read for first x dimension then y to build the map. Then the next two will be the coordinates of a terrain piece and 
        //        //identification for the terrain graphic and another point for whether its impassable

        //        //bin.Write(boardComp.GetDimenions().X);
        //        //bin.Write(boardComp.GetDimenions().Y);




        //    }
        //}
        //Entity LoadMap(string myMapName)
        //{
        //    Entity tempBoardEntity = null;

        //    if (File.Exists("Content/" + myMapName + ".bin"))
        //    {
        //        FileStream file = new FileStream("Content/" + myMapName + ".bin", System.IO.FileMode.Open);

        //        using (BinaryReader bin = new BinaryReader(file))
        //        {
        //            //TODO: REBUILD THE BOARD
        //            //bin.ReadByte();

        //            return tempBoardEntity;
        //        }
        //    }

        //    return tempBoardEntity;
        //}



        void ReadWorldMapBin()
        {
            if (File.Exists("Content/WorldMap.bin"))
            {
                FileStream worldMapBinFile = new FileStream("Content/WorldMap.bin", System.IO.FileMode.Open);
                BinaryReader worldMapBinReader = new BinaryReader(worldMapBinFile);

                List<string> worldMapLines = new List<string>();

                while (worldMapBinReader.PeekChar() != -1)
                {
                    worldMapLines.Add(worldMapBinReader.ReadString());
                }

                //numberOfMaps = worldMapLines.Count;

                Entity worldMapEntity = new Entity(0, State.ScreenState.WORLD_MAP);
                worldMapEntity.AddComponent(new SpriteComponent(true, new Vector2(screenWidth / 2, screenHeight / 2), worldMapTexture));
                worldMapEntity.AddComponent(new CameraComponent(new Vector2(screenWidth / 2, screenHeight / 2)));
                worldMapComponent = new WorldMapComponent();
                worldMapEntity.AddComponent(worldMapComponent);
                EntityManager.AddEntity(worldMapEntity);

                //Now we are to process the lines of the world map
                for (int l = 0; l < worldMapLines.Count; l++)
                {
                    string[] line = worldMapLines[l].Split(new string[] { " ; " }, StringSplitOptions.None);
                    string title = line[0].Split(':')[1];
                    string id = line[1].Split(':')[1];

                    ReadHexMapBin(id);

                    Boolean isSideQuest = Convert.ToBoolean(line[2].Split(':')[1]);

                    string coords = line[3].Split(':')[1];
                    Vector2 position = new Vector2(float.Parse(coords.Split(',')[0]), float.Parse(coords.Split(',')[1]));

                    string listOfConnect = line[4].Split(':')[1];
                    List<string> connectedTo = listOfConnect.Split(',').OfType<string>().ToList();;

                    NodeState state = (NodeState)Enum.Parse(typeof(NodeState), line[5].Split(':')[1]);


                    worldMapComponent.CreateNode(title,id,position,connectedTo,state,nodeTexture);
                }

                worldMapComponent.CreatePointer(worldMapComponent.GetNodeEntity(0), new Vector2(0, -20), pointerTexture);
            }
            else
            {
                throw new Exception("Either you named the worldmap wrong or it doesnt exist.");
            }
        }

        void ReadHexMapBin(string myID)
        {
            if (File.Exists("Content/" + myID + ".bin"))
            {
                FileStream hexMapBinFile = new FileStream("Content/" + myID + ".bin", System.IO.FileMode.Open);
                BinaryReader hexMapBinReader = new BinaryReader(hexMapBinFile);

                List<string> hexMapLines = new List<string>();

                while (hexMapBinReader.PeekChar() != -1)
                {
                    hexMapLines.Add(hexMapBinReader.ReadString());
                }
            }
            else
            {
                throw new Exception("This exception doesn't exist.");
            }
        }

        void CreateBoard(Vector2 myDimensions)
        {
            Entity board = new Entity(0, State.ScreenState.SKIRMISH);
            boardComp = new BoardComponent(board, hexBaseTexture, font, myDimensions);
            board.AddComponent(boardComp);
            EntityManager.AddEntity(board);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Graphics\\Fonts\\Debug");
            Globals.font = font;

            hexBaseTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\hexBase");
            hexGrassTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\hexWater0");
            hexDirtTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\hexDirt");

            unitTexture = Content.Load<Texture2D>("Graphics\\UnitTextures\\unitSample");

            selectSound = Content.Load<SoundEffect>("Audio\\Sounds\\Powerup27");

            worldMapTexture = Content.Load<Texture2D>("Graphics\\Backgrounds\\island");
            pointerTexture = Content.Load<Texture2D>("Graphics\\Other\\pointer");
            nodeTexture = Content.Load<Texture2D>("Graphics\\Other\\node");
        }

        void MoveUnit(HexComponent original, HexComponent final)
        {
            UnitComponent unit = original.GetUnit();
            Entity unitEntity = unit._parent;

            ((SpriteComponent) unitEntity.GetDrawable("SpriteComponent")).
                setPosition(final._parent.GetDrawable("SpriteComponent").position);

            unit.SetHex(final);
            final.SetUnit(unit);

            original.SetUnit(null); //todo: removeunit()
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                framesPerSecond = numberOfFrames;
                numberOfFrames = 0;
            }

            if (escapeClick.Evaluate())
            {
                this.Exit();
            }

            EntityManager.Update(gameTime);

            switch (State.screenState)
            {
                #region MainPage
                case State.ScreenState.MAIN_PAGE:
                    break;
                #endregion

                #region Setting
                case State.ScreenState.SETTINGS_MENU:
                    break;
                #endregion

                #region WorldMap
                case State.ScreenState.WORLD_MAP:

                    if (singleLeftClick.Evaluate())
                    {
                        for (int p = 0; p < worldMapComponent.GetNodeEntityList().Count; p++)
                        {
                            ClickableComponent click = worldMapComponent.GetNodeEntityList()[p].GetComponent("ClickableComponent") as ClickableComponent;
                            if (click.isColliding(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)))
                            {
                                selectSound.Play();
                                NodeComponent node = click._parent.GetComponent("NodeComponent") as NodeComponent;
                                worldMapComponent.SetSelectedNode(node);
                                //LoadMap(node.GetLevelName());
                            }
                        }
                    }
                    if (singleMiddleClick.Evaluate())
                    {
                        State.screenState = State.ScreenState.SKIRMISH;
                    }

                    break;
                #endregion

                #region Shop
                case State.ScreenState.SHOP:
                    break;
                #endregion

                #region Dialogue
                case State.ScreenState.DIALOGUE:
                    if (State.firstDialogueWord == "")
                    {
                        string line = string.Empty;
                        using (StreamReader sr = new StreamReader("dialogue1.txt"))
                        {
                            while ((line = sr.ReadLine()) != null)
                            {
                                State.currentDialogueMessage.Add(line);

                                if (State.firstDialogueWord == "")
                                {
                                    State.firstDialogueWord = line.Split(' ')[0];
                                    State.dialogueWordPosition = 1;
                                }
                            }
                        }
                    }
                    else if (gameTime.TotalGameTime.TotalMilliseconds - State.lastTimeDialogueChecked > Globals.dialogueDisplayRate)
                    {
                        string line = State.currentDialogueMessage[State.dialogueLinePosition];
                        string[] words = line.Split(' ');

                        string curWord = words[State.dialogueWordPosition];
                        char curChar = curWord[State.dialogueCharacterPosition];

                        State.dialogueCharacterPosition++;

                        if (State.messageBegin)
                        {
                            if (curWord == "]")
                            {
                                State.messageBegin = false;
                                State.displayedDialogueMessage += "\n"; // newlines for new messages

                                State.dialogueLinePosition++;
                                State.dialogueWordPosition = 0;
                            }
                            else
                            {
                                State.displayedDialogueMessage += curChar;
                                //  add chars blipping onto the screen
                            }

                        }
                        else
                        {
                            State.messageBegin = (curWord == "[");
                        }

                        if (State.dialogueCharacterPosition == curWord.Count())
                        {
                            if (State.dialogueWordPosition != 0)
                            {
                                State.displayedDialogueMessage += " ";
                            }
                            State.dialogueCharacterPosition = 0;
                            State.dialogueWordPosition++;
                        }

                        State.lastTimeDialogueChecked = (int)gameTime.TotalGameTime.TotalMilliseconds;
                    }

                    break;
#endregion
                #region Skirmish
                case State.ScreenState.SKIRMISH:

                    if (singleLeftClick.Evaluate())
                    {
                        HexComponent hexComp = boardComp.GetCurrentHexAtMouse();
                        Entity hexEntity = hexComp._parent;

                        boardComp.UpdateVisibilityAllies();

                        if (hexComp.HasUnit() && State.selectionState == State.SelectionState.NoSelection)
                        {
                            UnitComponent unit = hexComp.GetUnit();
                            State.selectionState = State.SelectionState.SelectingUnit;

                            State.originalHexClicked = hexComp;
                        }

                        else if (State.selectionState == State.SelectionState.SelectingUnit)
                        {
                            State.selectionState = State.SelectionState.SelectingOptionsForSkirmishUnits;
                        }
                        else if (State.selectionState == State.SelectionState.SelectingOptionsForSkirmishUnits)
                        {
                            MoveUnit(State.originalHexClicked, boardComp.GetCurrentHexAtMouse());
                            State.selectionState = State.SelectionState.NoSelection;
                            State.originalHexClicked = null;

                        }
                    }
                    else if (singleRightClick.Evaluate())
                    {
                        HexComponent hexComp = boardComp.GetCurrentHexAtMouse();
                        Entity hexEntity = hexComp._parent;
                        SpriteComponent sprite = hexEntity.GetDrawable("SpriteComponent") as SpriteComponent;

                        boardComp.CreateTerrain(hexComp.getCoordPosition(), hexGrassTexture, false);
                    }
                    if (singleMiddleClick.Evaluate())
                    {
                        boardComp.alliedUnitList[0].MoveDirection(Orient.se);
                        boardComp.UpdateVisibilityAllies();
                    }
                    break;
                #endregion

                #region ThirdLayer

                //As of now, we will not be using the states below
                case State.ScreenState.BATTLE_FORECAST:
                    break;
                case State.ScreenState.BATTLING:
                    break;
                case State.ScreenState.BATTLE_RESOLUTION:
                    break;
                #endregion

                default:
                    //This should nevr happen
                    break;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            numberOfFrames++;

            EntityManager.Draw(spriteBatch);

            spriteBatch.Begin();
            if (State.screenState == State.ScreenState.DIALOGUE)
            {
                spriteBatch.DrawString(Globals.font, State.displayedDialogueMessage, new Vector2(0, 0), Color.White);
            }
            else if (State.screenState == State.ScreenState.SKIRMISH)
            {
                EntityManager.Draw(spriteBatch);

                if (State.originalHexClicked != null)
                {
                    spriteBatch.Draw(hexDirtTexture, boardComp.screenCoordinatesOfHex(State.originalHexClicked.getCoordPosition())
                        , new Color(250,250,210,100));
                }
            }
            string fps = string.Format("fps: {0}", framesPerSecond);

            //if (State.screenState == State.ScreenState.DIALOGUE)
            //{
            //    spriteBatch.DrawString(Globals.font, State.displayedDialogueMessage, new Vector2(0, 0), Color.White);
            //}
            //else if (State.screenState == State.ScreenState.SKIRMISH)
            //{

            //}

            spriteBatch.DrawString(font, fps, Vector2.Zero, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
