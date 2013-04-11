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


namespace SeniorProjectGame
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //State.ScreenState screenState;

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
            mouseSingleRightClick = new InputAction(MouseButton.right, false);
            mouseSingleMiddleClick = new InputAction(MouseButton.middle, true);

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

            Entity unitEntity = new Entity(5);
            UnitComponent myUnitComponent = new UnitComponent(unitEntity,
                boardComp.getHex(new Vector2(3, 3)), true, myUnitData);

            //unitEntity.AddComponent(new AnimatedSpriteComponent(unitEntity, false, 
            //    new Vector2(3,3), unitTexture, 4, 50, 50));

            Texture2D debugTexture = Content.Load<Texture2D>("Graphics\\UnitTextures\\debugUnitOneFrame");

            unitEntity.AddComponent(new SpriteComponent(unitEntity, false, boardComp.getHex(new Vector2(3,3))
                ._parent.getDrawable("SpriteComponent").position, debugTexture));

            unitEntity.AddComponent(myUnitComponent);
            unitEntity.AddComponent(new CameraComponent(unitEntity, new Vector2(3, 3)));

            boardComp.getHex(3, 3).SetUnit(myUnitComponent);
            //boardComp.CreateUnit();
            
            EntityManager.AddEntity(unitEntity);

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

        void MoveUnit(HexComponent original, HexComponent final)
        {
            UnitComponent unit = original.GetUnit();
            Entity unitEntity = unit._parent;

            ((SpriteComponent) unitEntity.getDrawable("SpriteComponent")).
                setPosition(final._parent.getDrawable("SpriteComponent").position);

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
# region dialogue
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
                case State.ScreenState.SKIRMISH:

                    EntityManager.Update(gameTime);

                    if (mouseSingleLeftClick.Evaluate())
                    {
                        HexComponent hexComp = boardComp.GetCurrentHexAtMouse();
                        Entity hexEntity = hexComp._parent;

                        //// PSEUDO-CODE OUTLINE BELOW. DO NOT ERASE! t.Jon

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

                            //if (ButtonPressed == "Back")
                            //{
                            //    State.selectionState = State.SelectionState.NoSelection;
                            //}

                            //else if (OptionSelected == "Wait" || OptionSelected == "Item")
                            //{

                            //////MoveUnit(x1, y1, x2, y2);

                            //    UnitComponent unit = State.originalHexClicked.GetUnit();
                            //    State.originalHexClicked.RemoveUnit();

                            //    hexComp.SetUnit(unit);

                            //    State.SelectionState.NoSelection;
                            //}

                        }
                        

                        //SpriteComponent sprite = hexEntity.getDrawable("SpriteComponent") as SpriteComponent;

                        //sprite.setColor(Color.BurlyWood);

                        //List<HexComponent> hexRing = boardComp.GetAllRings(3);
                        //for (int p = 0; p < hexRing.Count; p++)
                        //{
                        //    Entity hexParent = hexRing[p]._parent;
                        //    SpriteComponent spriteParent = hexParent.getDrawable("SpriteComponent") as SpriteComponent;
                        //    spriteParent.setColor(Color.CadetBlue);
                        //}
                    }
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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            numberOfFrames++;

            // TODO: Add your drawing code here

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

            spriteBatch.DrawString(font, fps, Vector2.Zero, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
