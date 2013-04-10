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

        Random rand;

        Texture2D worldMapTexture, nodeTexture, pointerTexture;

        Texture2D hexBaseTexture, hexDirtTexture, hexGrassTexture;
        Texture2D unitTexture;

        SoundEffect selectSound;

        SpriteFont font;

        InputAction mouseSingleLeftClick, mouseSingleRightClick, mouseSingleMiddleClick, escapeAction;

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
        
            rand = new Random();

            

            escapeAction = new InputAction(new Keys[] { Keys.Escape }, true);
            mouseSingleLeftClick = new InputAction(MouseButton.left, true);
            mouseSingleRightClick = new InputAction(MouseButton.right, false);
            mouseSingleMiddleClick = new InputAction(MouseButton.middle, true);

            InitializeState();

            InitializeWorldMap();

            InitializeHexMap();

            base.Initialize();
        }

        void InitializeWorldMap()
        {
            Entity worldMapEntity = new Entity(0, State.ScreenState.WORLD_MAP);
            worldMapEntity.AddComponent(new SpriteComponent(true, new Vector2(screenWidth/2,screenHeight/2), worldMapTexture));
            worldMapEntity.AddComponent(new CameraComponent(new Vector2(screenWidth/2,screenHeight/2)));
            worldMapComponent = new WorldMapComponent();
            worldMapEntity.AddComponent(worldMapComponent);
            EntityManager.AddEntity(worldMapEntity);

            Entity startingNode = worldMapComponent.CreateAndReturnNode("test", NodeState.unlocked, new Vector2(400, 300), nodeTexture);
            worldMapComponent.CreateNode("test", NodeState.unlocked, new Vector2(450, 270), nodeTexture);
            worldMapComponent.CreateNode("test", NodeState.unlocked, new Vector2(675, 400), nodeTexture);

            worldMapComponent.CreatePointer(startingNode, new Vector2(0, -20),pointerTexture);

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
        void InitializeHexMap()
        {
            CreateBoard(new Vector2(27, 12));

            boardComp.CreateUnit(true, 2, new Vector2(5, 5), unitTexture, 50, 50);
            boardComp.CreateUnit(true, 2, new Vector2(16, 13), unitTexture, 50, 50);
            boardComp.CreateUnit(true, 2, new Vector2(10, 9), unitTexture, 50, 50);
        }

        void SaveMap(string myMapName,Entity myEntity)//THIS OVERWRITES WHAT'S THERE
        {
            FileStream file = new FileStream("Content/" + myMapName + ".bin", System.IO.FileMode.Create);

            using (BinaryWriter bin = new BinaryWriter(file))
            {
                //TODO: SAVE THE BOARD'S STATE W/O UNITS
                bin.Write("Files man");
            }
        }
        Entity LoadMap(string myMapName)
        {
            Entity tempBoardEntity = null;

            if (File.Exists("Content/" + myMapName + ".bin"))
            {
                FileStream file = new FileStream("Content/" + myMapName + ".bin", System.IO.FileMode.Open);

                using (BinaryReader bin = new BinaryReader(file))
                {
                    //TODO: REBUILD THE BOARD
                    //bin.ReadByte();

                    return tempBoardEntity;
                }
            }

            return tempBoardEntity;
        }

        void CreateBoard(Vector2 myDimensions)
        {
            Entity board = new Entity(0,State.ScreenState.SKIRMISH);
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
            hexGrassTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\hexGrass");
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

            if (escapeAction.Evaluate())
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

                    if (mouseSingleLeftClick.Evaluate())
                    {
                        for(int p = 0 ; p < worldMapComponent.GetNodeEntityList().Count;p++)
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
                    if (mouseSingleMiddleClick.Evaluate())
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

                    if (mouseSingleLeftClick.Evaluate())
                    {
                        boardComp.UpdateVisibilityAllies();
                    }
                    //if (mouseSingleLeftClick.Evaluate())
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
                    if (mouseSingleRightClick.Evaluate())
                    {
                        HexComponent hexComp = boardComp.GetCurrentHexAtMouse();
                        Entity hexEntity = hexComp._parent;
                        SpriteComponent sprite = hexEntity.GetDrawable("SpriteComponent") as SpriteComponent;

                        boardComp.CreateTerrain(hexComp.getCoordPosition(), hexGrassTexture, false);
                    }
                    if (mouseSingleMiddleClick.Evaluate())
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
