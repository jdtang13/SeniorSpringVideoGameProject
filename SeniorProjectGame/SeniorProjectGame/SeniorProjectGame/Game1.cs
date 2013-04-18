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
        #region Variables
        GraphicsDeviceManager graphics;
        int screenWidth = 1280;
        int screenHeight = 680;
        SpriteBatch spriteBatch;

        Dictionary<string, BoardComponent> hexMapDictionary = new Dictionary<string, BoardComponent>();
        public void AddHexMap(string myID, BoardComponent myBoard)
        {
            hexMapDictionary.Add(myID, myBoard);
        }
        public BoardComponent GetHexMap(string myID)
        {
            if (hexMapDictionary.ContainsKey(myID))
            {
                return hexMapDictionary[myID];
            }
            else
            {
                throw new Exception("That map doesn't exist");
            }
        }

        Dictionary<string, TerrainPackage> terrainDictionary = new Dictionary<string, TerrainPackage>();
        public TerrainPackage GetTerrain(string myKey)
        {
            if (terrainDictionary.ContainsKey(myKey))
            {
                return terrainDictionary[myKey];
            }
            else
            {
                throw new Exception(myKey + " doesn't exist in the terrain dictionary");
            }
        }

        Random rand = new Random();

        Texture2D worldMapTexture, nodeTexture, pointerTexture;

        Texture2D hexBaseTexture, hexDirtTexture, hexGrassTexture, hexGravelTexture, hexSandTexture, hexWoodTexture, hexWaterTexture, hexStoneTexture;
        Texture2D hexTreeTexture;
        Texture2D markerTexture;

        Texture2D unitTexture;

        SoundEffect selectSound;

        SpriteFont font;

        InputAction singleLeftClick, singleRightClick, singleMiddleClick;
        InputAction wClick, aClick, sClick, dClick, escapeClick;

        Entity worldMapEntity; WorldMapComponent worldMapComponent;
        Entity boardEntity; BoardComponent boardComponent;

        List<Entity> orderButtonEntityList = new List<Entity>();
        Entity attackOrderEntity, moveOrderEntity, noOrderEntity, spellOrderEntity;
        Texture2D attackOrderTexture, moveOrderTexture, noOrderTexture, spellOrderTexture;

        float framesPerSecond = 60;
        float numberOfFrames;
        TimeSpan elapsedTime;

        #endregion

        #region Initialization

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

            InitializeInput();

            ProcessWorldMapBin();

            InitializeState();

            CreateMenus();

            base.Initialize();
        }

        void CreateMenus()
        {
            //attackOrderEntity = new Entity(6, State.ScreenState.SKIRMISH);
            //attackOrderEntity.AddComponent(new SpriteComponent(true, Vector2.Zero, attackOrderTexture));
            //attackOrderEntity.AddComponent(new CameraComponent(Vector2.Zero));
            //attackOrderEntity.AddComponent(new ClickableComponent(Vector2.Zero, attackOrderTexture.Width, attackOrderTexture.Height));
            //orderButtonEntityList.Add(attackOrderEntity);
            //EntityManager.AddEntity(attackOrderEntity);

            //moveOrderEntity = new Entity(6, State.ScreenState.SKIRMISH);
            //moveOrderEntity.AddComponent(new SpriteComponent(true, Vector2.Zero, moveOrderTexture));
            //moveOrderEntity.AddComponent(new CameraComponent(Vector2.Zero));
            //moveOrderEntity.AddComponent(new ClickableComponent(Vector2.Zero, moveOrderTexture.Width, moveOrderTexture.Height));
            //orderButtonEntityList.Add(moveOrderEntity);
            //EntityManager.AddEntity(moveOrderEntity);

            //noOrderEntity = new Entity(6, State.ScreenState.SKIRMISH);
            //noOrderEntity.AddComponent(new SpriteComponent(true, Vector2.Zero, noOrderTexture));
            //noOrderEntity.AddComponent(new CameraComponent(Vector2.Zero));
            //noOrderEntity.AddComponent(new ClickableComponent(Vector2.Zero, noOrderTexture.Width, noOrderTexture.Height));
            //orderButtonEntityList.Add(noOrderEntity);
            //EntityManager.AddEntity(noOrderEntity);

            //spellOrderEntity = new Entity(6, State.ScreenState.SKIRMISH);
            //spellOrderEntity.AddComponent(new SpriteComponent(true, Vector2.Zero, spellOrderTexture));
            //spellOrderEntity.AddComponent(new CameraComponent(Vector2.Zero));
            //spellOrderEntity.AddComponent(new ClickableComponent(Vector2.Zero, spellOrderTexture.Width, spellOrderTexture.Height));
            //orderButtonEntityList.Add(spellOrderEntity);
            //EntityManager.AddEntity(spellOrderEntity);

        }

        void InitializeState()
        {
            State.Initialize();

            Dictionary<String, Role> classes = new Dictionary<String, Role>();
            Role lordClass = new Role(3, 2, 4, 4, 2, 0, 3,
                .1f, .2f, .2f, .3f, .4f, .1f, .3f,
                4, 2, 3, 5, 3, 2, 4,
                true, false, false, false, false, false, false, 3);

            classes["lord"] = lordClass;

            UnitData myUnitData = new UnitData(5, 5, 4, 3, 7, 2, 5,
                .6f, .5f, .4f, .5f, .7f, .3f, .6f,
                40, 40, 40, 35, 45, 35, 45, "myUnit",
                Alignment.PLAYER, classes["lord"], 1, 0);

            //Entity unitEntity = new Entity(5, State.ScreenState.SKIRMISH);
            //UnitComponent myUnitComponent = new UnitComponent(true, 4, boardComponent.GetHex(new Vector2(3, 3)), true, myUnitData);

            //unitEntity.AddComponent(new AnimatedSpriteComponent(unitEntity, false, 
            //    new Vector2(3,3), unitTexture, 4, 50, 50));

            //Texture2D debugTexture = Content.Load<Texture2D>("Graphics\\UnitTextures\\unitSample");

            //unitEntity.AddComponent(new SpriteComponent(false, boardComponent.GetHex(new Vector2(3,3))
            //    ._parent.GetDrawable("SpriteComponent").position, debugTexture));

            //unitEntity.AddComponent(myUnitComponent);
            //unitEntity.AddComponent(new CameraComponent(new Vector2(3, 3)));

            //boardComponent.GetHex(3, 3).SetUnit(myUnitComponent);
            //loadedBoardComponent.CreateUnit();

            //EntityManager.AddEntity(unitEntity);
        }

        void PopulateTerrainDictionary()
        {
            terrainDictionary["G"] = new TerrainPackage(hexGrassTexture, false);//Grass
            terrainDictionary["D"] = new TerrainPackage(hexDirtTexture, false);//Dirt
            terrainDictionary["L"] = new TerrainPackage(hexWaterTexture, true);//Water
            terrainDictionary["W"] = new TerrainPackage(hexWoodTexture, false);//Wood
            terrainDictionary["S"] = new TerrainPackage(hexStoneTexture, false);//Stone
            terrainDictionary["A"] = new TerrainPackage(hexSandTexture, false);//Sand
            terrainDictionary["R"] = new TerrainPackage(hexGravelTexture, false);//Gravel

            terrainDictionary["T"] = new TerrainPackage(hexTreeTexture, true);//Tree
        }

        void InitializeInput()
        {
            escapeClick = new InputAction(new Keys[] { Keys.Escape }, true);

            wClick = new InputAction(new Keys[] { Keys.W,Keys.Up },     false);
            dClick = new InputAction(new Keys[] { Keys.D, Keys.Right }, false);
            sClick = new InputAction(new Keys[] { Keys.S, Keys.Down },  false);
            aClick = new InputAction(new Keys[] { Keys.A, Keys.Left },  false);

            singleLeftClick = new InputAction(MouseButton.left, true);
            singleRightClick = new InputAction(MouseButton.right, false);
            singleMiddleClick = new InputAction(MouseButton.middle, true);

        }

        #endregion

        #region Loading_Content_and_File_Processing

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Graphics\\Fonts\\Debug");
            Globals.font = font;

            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\WorldMap.txt");
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Tutorial_Level.txt");
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Laboratory.txt");

            hexBaseTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexBase");
            hexGrassTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexGrass");
            hexGravelTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexGravel");
            hexSandTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexSand");
            hexWoodTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexWood0");
            hexWaterTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexWater0");
            hexStoneTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexStonePath0");
            hexDirtTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexDirt");

            hexTreeTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Decorations\\tree");

            unitTexture = Content.Load<Texture2D>("Graphics\\UnitTextures\\Flail");

            selectSound = Content.Load<SoundEffect>("Audio\\Sounds\\Powerup27");

            worldMapTexture = Content.Load<Texture2D>("Graphics\\Backgrounds\\island");
            pointerTexture = Content.Load<Texture2D>("Graphics\\Other\\pointer");
            nodeTexture = Content.Load<Texture2D>("Graphics\\Other\\node");

            attackOrderTexture = Content.Load<Texture2D>("Graphics\\Menu\\selectionIcon");
            moveOrderTexture = Content.Load<Texture2D>("Graphics\\Menu\\selectionIcon");
            spellOrderTexture = Content.Load<Texture2D>("Graphics\\Menu\\selectionIcon");
            noOrderTexture = Content.Load<Texture2D>("Graphics\\Menu\\selectionIcon");

            markerTexture = Content.Load<Texture2D>("Graphics\\Other\\marker");
        }

        void ConvertTxtToBin(string myFilePath)
        {
            if (File.Exists(myFilePath))
            {
                FileStream txtFile = new FileStream(myFilePath, System.IO.FileMode.Open);
                using (StreamReader txtReader = new StreamReader(txtFile))
                {
                    //Create a new bin file with read name or overwrite it if it already exists
                    FileStream binFile = new FileStream("Content/" + txtReader.ReadLine() + ".bin", System.IO.FileMode.Create);

                    using (BinaryWriter binWriter = new BinaryWriter(binFile))
                    {
                        while (txtReader.EndOfStream == false)
                        {
                            binWriter.Write(txtReader.ReadLine());
                        }
                        binWriter.Close();
                    }

                }
            }
            else
            {
                throw new Exception("No file exists at " + myFilePath + ".");
            }
        }
        List<string> ReadBin(string myID)
        {
            if (File.Exists("Content//" + myID + ".bin"))
            {
                FileStream binFile = new FileStream("Content/" + myID + ".bin", System.IO.FileMode.Open);
                using (BinaryReader binReader = new BinaryReader(binFile))
                {
                    List<string> lines = new List<string>();

                    while (binReader.PeekChar() != -1)
                    {
                        lines.Add(binReader.ReadString());
                    }

                    binReader.Close();
                    return lines;
                }
            }
            else
            {
                throw new Exception(myID + " doesn't exist. Did you load the txt of it?");
            }
        }

        void ProcessWorldMapBin()
        {
            List<string> worldMapLines = ReadBin("WorldMap");

            worldMapEntity = new Entity(1, State.ScreenState.WORLD_MAP);
            worldMapEntity.AddComponent(new SpriteComponent(true, Vector2.Zero, worldMapTexture));
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
                List<string> connectedTo = listOfConnect.Split(',').OfType<string>().ToList(); ;

                NodeState state = (NodeState)Enum.Parse(typeof(NodeState), line[5].Split(':')[1]);

                worldMapComponent.CreateNode(title, id, position, connectedTo, state, nodeTexture);
            }

            worldMapComponent.CreatePointer(worldMapComponent.GetNodeEntity(0), new Vector2(0, -20), pointerTexture);

        }
        Entity ProcessHexMapBin(string myID)
        {
            List<string> binLines = ReadBin(myID);

            int layers = Convert.ToInt32(binLines[0]);
            Vector2 dimensions = new Vector2(float.Parse(binLines[1].Split(' ')[0]), float.Parse(binLines[1].Split(' ')[0]));

            Entity tempBoard = new Entity(0, State.ScreenState.SKIRMISH);
            BoardComponent tempBoardComponent = new BoardComponent(dimensions, hexBaseTexture, font);
            tempBoard.AddComponent(tempBoardComponent);

            EntityManager.AddEntity(tempBoard);

            List<string> hexMapLines = new List<string>();
            for (int line = 2; line < binLines.Count; line++)
            {
                if (binLines[line] != "")
                    hexMapLines.Add(binLines[line]);
            }

            Vector2 terrainCoordinate = Vector2.Zero;
            for (int layer = 0; layer < layers; layer++)
            {
                for (int y = 0 + ((int)dimensions.Y * layer); y < dimensions.Y + ((int)dimensions.Y * layer); y++)
                {
                    string[] line = hexMapLines[y].Split(' ');

                    for (int x = 0; x < line.Length; x++)
                    {
                        terrainCoordinate = new Vector2(x, y - ((int)dimensions.Y * layer));

                        if (line[x] != "*")
                        {
                            tempBoardComponent.AddTerrain(ConvertToHexCoordinate(terrainCoordinate),layer, GetTerrain(line[x]));
                        }
                    }
                }
            }
            return tempBoard;
        }

        #endregion

        #region Update

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
            if(wClick.Evaluate())
            {
                Camera.Move(new Vector2(0, -5));
            }
            if(dClick.Evaluate())
            {
                Camera.Move(new Vector2(5, 0));
            }
            if(sClick.Evaluate())
            {
                Camera.Move(new Vector2(0, 5));
            }
            if (aClick.Evaluate())
            {
                Camera.Move(new Vector2(-5, 0));
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
                            if (click.isColliding(new Vector2(InputState.GetMousePosition().X, InputState.GetMousePosition().Y)))
                            {
                                selectSound.Play();
                                NodeComponent node = click._parent.GetComponent("NodeComponent") as NodeComponent;
                                worldMapComponent.SetSelectedNode(node);


                                boardEntity = ProcessHexMapBin(worldMapComponent.SelectCurrentNode());
                                boardEntity = ProcessHexMapBin("Tutorial_Level");
                                boardComponent = boardEntity.GetComponent("BoardComponent") as BoardComponent;

                                boardComponent.CreateUnit(true, 2, new Vector2(0, 2), unitTexture, 50, 100);
                                boardComponent.CreateUnit(true, 2, new Vector2(0, 3), unitTexture, 50, 100);


                                State.screenState = State.ScreenState.SKIRMISH;
                            }
                        }
                    }
                    if (singleMiddleClick.Evaluate())
                    {
                        State.screenState = State.ScreenState.SKIRMISH;
                    }
                    if (singleRightClick.Evaluate())
                    {
 
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

                    HexComponent hexComp = boardComponent.GetMouseHex();
                    Entity hexEntity = hexComp._parent;

                    if (singleLeftClick.Evaluate())
                    {
                        if (hexComp.HasUnit() && State.selectionState == State.SelectionState.NoSelection)
                        {
                            UnitComponent unit = hexComp.GetUnit();
                            State.selectionState = State.SelectionState.SelectingUnit;
                            State.originalHexClicked = hexComp;
                            State.unitMenuState = State.MenuState.NONE;

                            ////Show the buttons
                            //for (int y = 0; y < orderButtonEntityList.Count; y++)
                            //{
                            //    Entity parent = orderButtonEntityList[y];
                            //    SpriteComponent sprite = parent.GetDrawable("SpriteComponent") as SpriteComponent;

                            //    //sprite._visible = true;
                            //}

                            boardComponent.UpdateVisibilityAllies();

                        }

                        else if (!hexComp.HasUnit() && State.selectionState == State.SelectionState.SelectingUnit)
                        {
                            if (hexComp.ContainsImpassable() == false)
                            {
                                MoveUnit(State.originalHexClicked, boardComponent.GetMouseHex());
                                State.selectionState = State.SelectionState.NoSelection;
                                State.originalHexClicked = null;
                            }
                        }

                        //else if (State.selectionState == State.SelectionState.SelectingOptionsForSkirmishUnits)
                        //{
                        //    //Check to see if you are clicking on an order option


                        //}
                        //else if (State.selectionState == State.SelectionState.SelectingOptionsForSkirmishUnits)
                        //{
                        //    MoveUnit(State.originalHexClicked, boardComponent.GetMouseHex());
                        //    State.selectionState = State.SelectionState.NoSelection;
                        //    State.originalHexClicked = null;

                        //}
                    }
                    else if (singleRightClick.Evaluate())
                    {
                        Camera.Move(new Vector2(5, 5));
                        
                    }
                    if (singleMiddleClick.Evaluate())
                    {

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

                #region Default
                default:
                    //This should nevr happen
                    break;
                #endregion
            }
            base.Update(gameTime);
        }

        Vector2 ConvertToHexCoordinate(Vector2 myVec)
        {
            Vector2 convertedVector = Vector2.Zero;
            convertedVector.X = myVec.X;

            if (myVec.X % 2 == 0)
            {
                convertedVector.Y = myVec.X / 2f + myVec.Y;
            }
            else
            {
                convertedVector.Y = (myVec.X + 1f) / 2f + myVec.Y;
            }

            return convertedVector;
        }

        void MoveUnit(HexComponent original, HexComponent final)
        {
            UnitComponent unit = original.GetUnit();
            Entity unitEntity = unit._parent;

            //SpriteComponent unitSprite = unitEntity.GetDrawable("SpriteComponent") as SpriteComponent;
            //SpriteComponent hexSprite = Entity.GetDrawable("SpriteComponent") as SpriteComponent;
            ////sprite.setPosition(final.

            ((AnimatedSpriteComponent)unitEntity.GetDrawable("AnimatedSpriteComponent")).
                SetPosition(final._parent.GetDrawable("SpriteComponent").GetPosition());

            unit.SetHex(final);
            final.SetUnit(unit);

            boardComponent.UpdateVisibilityAllies();


            original.RemoveUnit(); //todo: removeunit()
        }

        #endregion

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            EntityManager.Draw(spriteBatch,graphics);

            spriteBatch.Begin();

            spriteBatch.DrawString(font, InputState.GetMousePosition().ToString(), new Vector2(0, font.LineSpacing), Color.White);

            numberOfFrames++;
            string fps = string.Format("fps: {0}", framesPerSecond);
            spriteBatch.DrawString(font, fps, Vector2.Zero, Color.White);
            spriteBatch.Draw(markerTexture, new Vector2(640, 340), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
