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

namespace World_Map_Test
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState myMouse;

        Map worldMap;
        List<Node> nodes;
        Pointer nodePointer;

        Texture2D nodeSprite;
        Texture2D mapSprite;
        Texture2D pointerSprite;

        int SCREEN_WIDTH;
        int SCREEN_HEIGHT;
        const float EPSILON = .00001f;

        SpriteFont nodeFont;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            Content.RootDirectory = "Content";
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
            
            worldMap = new Map(new Vector2(0, 0));

            nodePointer = new Pointer();

            nodes = new List<Node>();
            Node node0 = new Node(0, new Vector2 (300, 400), "Node 0"); //Nodes are hard-created for now
            Node node1 = new Node(1, new Vector2 (330, 250), "Node 1");
            Node node2 = new Node(2, new Vector2 (250, 290), "Node 2");
            Node node3 = new Node(3, new Vector2 (460, 240), "Node 3");

            nodes.Add(node0);
            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);

            LoadContent();

            for (int i = 0; i < nodes.Count; i++) //Loads sprites
            {
                nodes[i].SetSprite(nodeSprite);
            }

            IsMouseVisible = true;

            SCREEN_HEIGHT = graphics.GraphicsDevice.Viewport.Height;
            SCREEN_WIDTH = graphics.GraphicsDevice.Viewport.Width;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            nodeFont = Content.Load<SpriteFont>("NodeName");

            nodeSprite = Content.Load<Texture2D>("Node");

            mapSprite = Content.Load<Texture2D>("Island"); 
            worldMap.SetSprite(mapSprite);

            pointerSprite = Content.Load<Texture2D>("pointer");
            nodePointer.SetSprite(pointerSprite);
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
            PlayerInput();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        public void PlayerInput()
        {
            MouseState lastMouse = myMouse; //last state of mouse
            myMouse = Mouse.GetState(); //current state of mouse
            Point mousePosition = new Point(myMouse.X, myMouse.Y);
            bool click = false; //every instance of PlayerInput resets click

            if (lastMouse.LeftButton == ButtonState.Released && myMouse.LeftButton == ButtonState.Pressed)
            {
                click = true; //reacts to click
                nodePointer.SetEnabled(true);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Hitbox().Contains(mousePosition) && click)
                {
                    float X = nodes[i].Position().X;
                    float Y = nodes[i].Position().Y - 20;
                    nodePointer.SetPosition(new Vector2(X,Y));
                }
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(worldMap.Sprite(), new Vector2(0,0), Color.White);

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw(spriteBatch);
                nodes[i].SetHitbox((int)nodes[i].Position().X, (int)nodes[i].Position().Y);
            }

            if (nodePointer.Enabled())
            {
                nodePointer.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
