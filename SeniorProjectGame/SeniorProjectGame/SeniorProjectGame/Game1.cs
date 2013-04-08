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


namespace SeniorProjectGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Random rand;

        Texture2D hexBaseTexture, hexDirtTexture, hexGrassTexture;
        Texture2D unitTexture;

        SpriteFont font;

        InputAction mouseSingleLeftClick, mouseSingleRightClick, mouseSingleMiddleClick, escapeAction;

        BoardComponent boardComp;

        float framesPerSecond, numberOfFrames;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //1280x720
            graphics.PreferredBackBufferHeight = 680;
            graphics.PreferredBackBufferWidth = 1280;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            LoadContent();
            CreateBoard();

            rand = new Random();

            escapeAction = new InputAction(new Keys[] { Keys.Escape }, true);
            mouseSingleLeftClick = new InputAction(MouseButton.left, true);
            mouseSingleRightClick = new InputAction(MouseButton.right, true);
            mouseSingleMiddleClick = new InputAction(MouseButton.middle, true);

            #region stateInit
            State.screenState = State.ScreenState.SKIRMISH;
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
            #endregion

            boardComp.CreateUnit(true, 2, new Vector2(5, 5), unitTexture, 50, 50);
            boardComp.CreateUnit(true, 2, new Vector2(16,13), unitTexture, 50, 50);
            boardComp.CreateUnit(true, 2, new Vector2(10, 9), unitTexture, 50, 50);

            base.Initialize();
        }

        void CreateBoard()
        {
            Entity board = new Entity(0);
            boardComp = new BoardComponent(board, hexBaseTexture, font, new Vector2(27, 12));
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

            switch (State.screenState)
            {
                case State.ScreenState.MAIN_PAGE:
                    break;
                case State.ScreenState.SETTINGS_MENU:
                    break;
                case State.ScreenState.WORLD_MAP:
                    break;
                case State.ScreenState.SHOP:
                    break;
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
                case State.ScreenState.SKIRMISH:

                    EntityManager.Update(gameTime);

                    if (mouseSingleLeftClick.Evaluate())
                    {
                        boardComp.UpdateFog();
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

                        //SpriteComponent sprite = hexEntity.getDrawable("SpriteComponent") as SpriteComponent;

                        //sprite.setColor(Color.BurlyWood);

                        //List<HexComponent> hexRing = boardComp.GetAllRings(3);
                        //for (int p = 0; p < hexRing.Count; p++)
                        //{
                        //    Entity hexParent = hexRing[p]._parent;
                        //    SpriteComponent spriteParent = hexParent.getDrawable("SpriteComponent") as SpriteComponent;
                        //    spriteParent.setColor(Color.CadetBlue);
                        //}
                    //}
                    if (mouseSingleRightClick.Evaluate())
                    {
                        HexComponent hexComp = boardComp.GetCurrentHexAtMouse();
                        Entity hexEntity = hexComp._parent;
                        SpriteComponent sprite = hexEntity.getDrawable("SpriteComponent") as SpriteComponent;

                        boardComp.CreateTerrain(hexComp.getCoordPosition(), hexGrassTexture, false);
                    }
                    if (mouseSingleMiddleClick.Evaluate())
                    {
                        HexComponent hexComp = boardComp.GetCurrentHexAtMouse();
                        boardComp.CreateTerrain(hexComp.getCoordPosition(), hexDirtTexture, false);
                    }
                    break;


                //As of now, we will not be using the states below
                case State.ScreenState.BATTLE_FORECAST:
                    break;
                case State.ScreenState.BATTLING:
                    break;
                case State.ScreenState.BATTLE_RESOLUTION:
                    break;



                default:
                    //This should nevr happen
                    break;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            numberOfFrames++;

            spriteBatch.Begin();


            if (State.screenState == State.ScreenState.DIALOGUE)
            {
                spriteBatch.DrawString(Globals.font, State.displayedDialogueMessage, new Vector2(0, 0), Color.White);
            }
            else if (State.screenState == State.ScreenState.SKIRMISH)
            {
                EntityManager.Draw(spriteBatch);
            }
            string fps = string.Format("fps: {0}", framesPerSecond);


            spriteBatch.DrawString(font, fps, Vector2.Zero, Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
