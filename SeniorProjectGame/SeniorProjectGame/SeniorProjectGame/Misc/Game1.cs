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
using EntityEngine.Components.TileComponents;
using EntityEngine.Input;

namespace SeniorProjectGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //State.ScreenState screenState;

        Random rand;

        Texture2D hexTexture;
        Texture2D hexPiece;

        SpriteFont font;

        Entity player;
        InputAction mouseLeftClick, mouseRightClick, escapeAction;

        BoardComponent boardComp;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //1280x720
            //graphics.PreferredBackBufferHeight = 720;
            //graphics.PreferredBackBufferWidth = 1280;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            IsMouseVisible = true;
            LoadContent();
            CreateBoard();

            boardComp.CreateUnit(new Vector2(5, 5), hexPiece);

            rand = new Random();
            escapeAction = new InputAction(new Keys[] { Keys.Escape }, true);
            mouseLeftClick = new InputAction(MouseButton.left, false);
            mouseRightClick = new InputAction(MouseButton.right, false);

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

            Globals.font = Content.Load<SpriteFont>("font");

            base.Initialize();
        }

        void CreateBoard()
        {
            Entity board = new Entity(0);
            boardComp = new BoardComponent(board, hexTexture, font, new Vector2(20, 20));
            board.AddComponent(boardComp);
            EntityManager.AddEntity(board);

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Debug");
            hexTexture = Content.Load<Texture2D>("hex");
            hexPiece = Content.Load<Texture2D>("hexPiece");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (State.screenState == State.ScreenState.DIALOGUE)
            {
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
            }
            else if (State.screenState == State.ScreenState.SKIRMISH)
            {
                // skirmish map update
                EntityManager.Update(gameTime);

                if (escapeAction.Evaluate())
                {
                    this.Exit();
                }
                if (mouseLeftClick.Evaluate())
                {
                    HexComponent hexComp = boardComp.getCurrentHexAtMouse();
                    Entity hexEntity = hexComp._parent;

                    // PSEUDO-CODE OUTLINE BELOW. DO NOT ERASE! t.Jon

                    /*
                    if (hexComp.HasUnit() && State.selectionState == State.SelectionState.NoSelection) {
                        UnitComponent unit = hexComp.GetUnit();
                        State.selectionState = State.SelectionState.SelectingUnit;

                        State.originalHexClicked = hexComp;
                    }
                    else if (State.selectionState == State.SelectionState.SelectingUnit)
                    {
                        State.selectionState = State.SelectionState.SelectingOptionsForSkirmishUnits;
                    }
                    else if (State.selectionState == State.SelectionState.SelectingOptionsForSkirmishUnits) {

                        if (ButtonPressed == "Back")
                        {
                            State.selectionState = State.SelectionState.NoSelection;
                        }
                        else if (OptionSelected == "Wait" || OptionSelected == "Item")
                        {
                            UnitComponent unit = State.originalHexClicked.GetUnit();
                            State.originalHexClicked.RemoveUnit();

                            hexComp.SetUnit(unit);

                            State.SelectionState.NoSelection;
                        }

                    }
                    */

                    SpriteComponent sprite = hexEntity.getDrawable("SpriteComponent") as SpriteComponent;

                    sprite.setColor(Color.BurlyWood);

                }
                if (mouseRightClick.Evaluate())
                {
                    HexComponent hexComp = boardComp.getCurrentHexAtMouse();
                    Entity hexEntity = hexComp._parent;
                    SpriteComponent sprite = hexEntity.getDrawable("SpriteComponent") as SpriteComponent;

                    sprite.setColor(Color.White);
                }

            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            if (State.screenState == State.ScreenState.DIALOGUE)
            {
                spriteBatch.DrawString(Globals.font, State.displayedDialogueMessage, new Vector2(0, 0), Color.White);
            }
            else if (State.screenState == State.ScreenState.SKIRMISH)
            {
                EntityManager.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
