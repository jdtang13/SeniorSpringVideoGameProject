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
            ConvertTxtToBinSave("C:\\Users\\Oliver\\Desktop\\WorldMap.txt");
            LoadBinaryFiles();

            //InitializeHexMap();

            base.Initialize();
        }

        void InitializeState()
        {
            State.screenState = State.ScreenState.WORLD_MAP;
            State.selectionState = State.SelectionState.NoSelection;
            State.dialoguePosition = 0;
            State.dialogueChoicePosition = 0;
            State.displayedDialogueMessage = "";

            State.dialogueLinePosition = 0;
            State.dialogueWordPosition = 0;
            State.dialogueCharacterPosition = 0;

            State.firstDialogueWord = "";
            State.lastTimeDialogueChecked = 0;
            State.messageBegin = false;
            State.currentDialogueMessage = new List<string>();
        }

        void PopulateTerrainDictionary()
        {
            //AddTerrainChar("G", hexGrassTexture);
            //AddTerrainChar("D", hexDirtTexture);
        }

        void InitializeHexMap()
        {
            CreateBoard(new Vector2(27, 12));

            boardComp.CreateUnit(true, 2, new Vector2(5, 5), unitTexture, 50, 50);
            boardComp.CreateUnit(true, 2, new Vector2(16, 13), unitTexture, 50, 50);
            boardComp.CreateUnit(true, 2, new Vector2(10, 9), unitTexture, 50, 50);
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

        void LoadBinaryFiles()
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

        void ConvertTxtToBinSave(string myFilePath)
        {
            if (File.Exists(myFilePath))
            {
                FileStream txtFile = new FileStream(myFilePath, System.IO.FileMode.Open);

                StreamReader titleReader = new StreamReader(txtFile);
                string fileName = titleReader.ReadLine();

                //Create a new bin file with read name or overwrite it if it already exists
                FileStream binFile = new FileStream("Content/" + fileName + ".bin", System.IO.FileMode.Create);

                using (BinaryWriter binWriter = new BinaryWriter(binFile))
                {
                    //We have to create a new streamreader to start at the top again
                    //StreamReader txtReader = new StreamReader(txtFile);

                    while (titleReader.EndOfStream == false)
                    {
                        binWriter.Write(titleReader.ReadLine());
                    }
                }
            }
            else
            {
                throw new Exception("No file exists at " + myFilePath + ".");
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
                        boardComp.UpdateVisibilityAllies();
                    }
                    //if (singleLeftClick.Evaluate())
                    //{
                    //HexComponent hexComp = boardComp.GetCurrentHexAtMouse();
                    //Entity hexEntity = hexComp._parent;

                    //// PSEUDO-CODE OUTLINE BELOW. DO NOT ERASE! t.Jon


                    ////if (hexComp.HasUnit() && State.selectionState == State.SelectionState.NoSelection)
                    ////{
                    ////    UnitComponent unit = hexComp.GetUnit();
                    ////    State.selectionState = State.SelectionState.SelectingUnit;

                    ////    State.originalHexClicked = hexComp;
                    ////}



                    ////else if (State.selectionState == State.SelectionState.SelectingUnit)
                    ////{
                    ////    State.selectionState = State.SelectionState.SelectingOptionsForSkirmishUnits;
                    ////}
                    ////else if (State.selectionState == State.SelectionState.SelectingOptionsForSkirmishUnits) {

                    ////    if (ButtonPressed == "Back")
                    ////    {
                    ////        State.selectionState = State.SelectionState.NoSelection;
                    ////    }
                    ////    else if (OptionSelected == "Wait" || OptionSelected == "Item")
                    ////    {
                    ////        UnitComponent unit = State.originalHexClicked.GetUnit();
                    ////        State.originalHexClicked.RemoveUnit();

                    ////        hexComp.SetUnit(unit);

                    ////        State.SelectionState.NoSelection;
                    ////    }

                    ////}
                    ////*/

                    //SpriteComponent sprite = hexEntity.GetDrawable("SpriteComponent") as SpriteComponent;

                    //sprite.setColor(Color.BurlyWood);

                    //List<HexComponent> hexRing = boardComp.GetAllRings(3);
                    //for (int p = 0; p < hexRing.Count; p++)
                    //{
                    //    Entity hexParent = hexRing[p]._parent;
                    //    SpriteComponent spriteParent = hexParent.GetDrawable("SpriteComponent") as SpriteComponent;
                    //    spriteParent.setColor(Color.CadetBlue);
                    //}
                    //}
                    if (singleRightClick.Evaluate())
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

            //if (State.screenState == State.ScreenState.DIALOGUE)
            //{
            //    spriteBatch.DrawString(Globals.font, State.displayedDialogueMessage, new Vector2(0, 0), Color.White);
            //}
            //else if (State.screenState == State.ScreenState.SKIRMISH)
            //{

            //}

            string fps = string.Format("fps: {0}", framesPerSecond);
            spriteBatch.DrawString(font, fps, Vector2.Zero, Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
