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

            rand = new Random();
            escapeAction = new InputAction(new Keys[] { Keys.Escape }, true);
            mouseLeftClick = new InputAction(MouseButton.left, false);
            mouseRightClick = new InputAction(MouseButton.right, false);

            State.screenState = State.ScreenState.SKIRMISH;
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
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
                    SpriteComponent sprite = hexEntity.getDrawable("SpriteComponent") as SpriteComponent;

                    sprite.setColor(Color.BurlyWood);
                    
                    List<HexComponent> hexRing = boardComp.GetAllRings(2);
                    for (int p = 0; p < hexRing.Count; p++)
                    {
                        Entity hexParent = hexRing[p]._parent;
                        SpriteComponent spriteParent = hexParent.getDrawable("SpriteComponent") as SpriteComponent;
                        spriteParent.setColor(Color.CadetBlue);
                    }

                    //List<HexComponent> hexRing = boardComp.GetAllRings(3);
                    //for (int p = 0; p < hexRing.Count; p++)
                    //{
                    //    Entity hexParent = hexRing[p]._parent;
                    //    SpriteComponent spriteParent = hexParent.getDrawable("SpriteComponent") as SpriteComponent;
                    //    spriteParent.setColor(Color.CadetBlue);
                    //}

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
            GraphicsDevice.Clear(Color.Black);

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
