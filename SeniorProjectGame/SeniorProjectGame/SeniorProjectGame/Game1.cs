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
using EntityEngine.Stat_Attribute_Classes;
using EntityEngine.Dialogue;
using System.Text.RegularExpressions;

using SCG = System.Collections.Generic; //  commands needed for C5 library
using C5;

namespace SeniorProjectGame
{
    //  object that is indexed in a priority queue, with a certain priority
    struct Prio<D> : IComparable<Prio<D>> where D : class
    {
        public readonly D data;
        private float priority;
        public Prio(D data, float priority)
        {
            this.data = data; this.priority = priority;
        }
        public int CompareTo(Prio<D> that)
        {
            return this.priority.CompareTo(that.priority);
        }
        public bool Equals(Prio<D> that)
        {
            return this.priority == that.priority;
        }
        public D Data()
        {
            return this.data;
        }

        public static Prio<D> operator +(Prio<D> prio, float delta)
        {
            return new Prio<D>(prio.data, prio.priority + delta);
        }
        public static Prio<D> operator -(Prio<D> prio, float delta)
        {
            return new Prio<D>(prio.data, prio.priority - delta);
        }

    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables

        Random rand = new Random();

        //Graphics and Content Vars
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //  the main game menu
        Menu menu = new Menu(false);
        Texture2D dot;

        //  the main menu for the battle screen (3rd layer)
        NestedMenu battleMenu = new NestedMenu(false);

        //Textures and other content

        //Hexmap Textures

        Texture2D worldMapTexture, nodeTexture, pointerTexture;
        Texture2D hexBaseTexture, dirtTexture, grassTexture, gravelTexture, sandTexture, woodTexture,
            waterTexture, stoneTexture;
        Texture2D treeTexture, wallTexture, bushTexture, tableTexture, carpetTexture, throneTexture, tentTexture;
        Texture2D markerTexture, questionTexture;

        //Unit Textures
        Dictionary<string, Texture2DFramed> unitTextureDictionary = new Dictionary<string, Texture2DFramed>();

        Texture2DFramed unitFramedTexture, axemanFramedTexture, battlemageFramedTexture, bowmanFramedTexture, crossbowmanFramedTexture,
            flailmanFramedTexture, halberdierFramedTexture, knightFramedTexture, mageAssassinFramedTexture, manAtArmsFramedTexture,
            pikemanFramedTexture, riflemanFramedTexture, spearmanFramedTexture, swordsmanFramedTexture, wizardFramedTexture, slimeFramedTexture;

        //Dialogue Textures
        Texture2D dialogueBackdropTexture, dialogueWideBackdropTexture;
        Texture2D missingActorTexture, harryPortrait;
        Dictionary<string, PortraitPackage> portraitDictionary = new Dictionary<string, PortraitPackage>();

        //Sounds
        SoundEffect selectSound, downSound;

        SpriteFont font;

        //Party Member Stuff
        List<Entity> partyMemberList = new List<Entity>();
        List<Entity> partyMemberButtonList = new List<Entity>(); //Select to highlight, doublclick for more information
        Entity[,] currentlyViewedPartyButtons; Vector2[,] currentlyViewedPartyButtonPositions;

        Entity partyMemberAcceptButton, partyMemberUpButton, partyMemberDownButton;
        Texture2D partyMemberButtonTexture, acceptButtonTexture, partyMemberUpButtonTexture, partyMemberDownButtonTexture; //TODO: Make this graphic

        //Party Member Editting Screen
        Entity editedPartyMember;

        Entity memberBackButton;
        Entity[] memberInventoryButtons = new Entity[5];//This persons equiped weapons
        Entity[,] communityInventoryButtons = new Entity[20, 20]; //Community Invetory

        Texture2D backButtonTexture;

        //Hex Vars
        Entity worldMapEntity; WorldMapComponent worldMapComponent;
        Entity boardEntity; BoardComponent boardComponent;

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
                return new TerrainPackage(questionTexture, false, 0);
            }
        }
        HexComponent ghostHex = null;
        List<HexComponent> pathQueue = new List<HexComponent>();
        List<UnitComponent> oldEnemiesYouSee = new List<UnitComponent>();
        List<UnitComponent> newEnemiesYouSee = new List<UnitComponent>();
        int clusterDetectionRadius = 4;

        Vector2 skirmishCameraPosition;//We have to save this variable when the map is loaded that it may be used only after
        //the dialogue and the prepartation is finished. Only then will the camera not be at 0,0.

        bool moving = false;
        bool enemiesMoving = false;
        bool yourTurn = true;
        float timePerMove;

        List<HexComponent> pathToDestination = null;
        HexComponent enemyHex;

        string debugString;
        string debugString1;

        List<HexComponent> oldVisible;
        List<HexComponent> newVisible;

        //Events
        Dictionary<string, EventFunction> eventDictionary = new Dictionary<string, EventFunction>();

        //Player Vars
        Dictionary<String, Role> classes = new Dictionary<String, Role>();
        List<UnitData> partyUnitData = new List<UnitData>();

        //Input Vars
        InputAction singleLeftClick, singleRightClick, singleMiddleClick;
        InputAction leftHold, spaceHold;

        InputAction wClick, aClick, sClick, dClick, enterClick, escapeClick, qClick;
        InputAction singleWClick, singleAClick, singleSClick, singleDClick;
        float doubleClickTimer;

        //FPS Vars
        float framesPerSecond = 60;
        float numberOfFrames;
        TimeSpan elapsedTimeForFps;
        float elapsedTimeForMove;
        TimeSpan elapsedTimeForStep;

        #endregion

        #region Initialization

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            State.screenWidth = 1280;
            State.screenHeight = 680;

            //1280x720
            graphics.PreferredBackBufferHeight = State.screenHeight;
            graphics.PreferredBackBufferWidth = State.screenWidth;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            LoadContent();
            PopulateUnitTextureDictionary();
            PopulatePortraitDictionary();
            ChatboxManager.Initialize(portraitDictionary, font, dialogueWideBackdropTexture, new Vector4(50, 50, 50, 50));

            ProcessWorldMapBin();
            ProcessPlayerRolesBin();

            ProcessEnemyBestiaryBin();

            InitializeInput();
            //InitializeSkirmishPreparationEntities();

            //  dot is a generic white pixel texture used for generating colored rectangles
            State.dot = new Texture2D(graphics.GraphicsDevice, 1, 1);
            State.dot.SetData(new Color[] { Color.White });
            dot = State.dot;

            State.Initialize();

            Entity victoryText = new Entity(0, State.ScreenState.SKIRMISH_RESOLUTION);
            victoryText.AddComponent(new TextSpriteComponent(true, "You've won!", Color.White, new Vector2(300, 300), font));
            EntityManager.AddEntity(victoryText);

            base.Initialize();
        }

        void InitializeInput()
        {
            escapeClick = new InputAction(new Keys[] { Keys.Escape }, true);
            enterClick = new InputAction(new Keys[] { Keys.Enter }, true);
            spaceHold = new InputAction(new Keys[] { Keys.Space }, false);

            wClick = new InputAction(new Keys[] { Keys.W, Keys.Up }, false);
            dClick = new InputAction(new Keys[] { Keys.D, Keys.Right }, false);
            sClick = new InputAction(new Keys[] { Keys.S, Keys.Down }, false);
            aClick = new InputAction(new Keys[] { Keys.A, Keys.Left }, false);
            qClick = new InputAction(new Keys[] { Keys.Q }, false);

            singleWClick = new InputAction(new Keys[] { Keys.W, Keys.Up }, true);
            singleDClick = new InputAction(new Keys[] { Keys.D, Keys.Right }, true);
            singleSClick = new InputAction(new Keys[] { Keys.S, Keys.Down }, true);
            singleAClick = new InputAction(new Keys[] { Keys.A, Keys.Left }, true);

            singleLeftClick = new InputAction(MouseButton.left, true);

            leftHold = new InputAction(MouseButton.left, false);

            singleRightClick = new InputAction(MouseButton.right, false);
            singleMiddleClick = new InputAction(MouseButton.middle, true);
        }

        void InitializeSkirmishPreparationEntities()
        {
            ////Party Member Stuff
            //List<Entity> partyMemberList = new List<Entity>();
            //List<Entity> partyMemberButtonList = new List<Entity>(); //Select to highlight, doublclick for more information
            //Entity partyMemberAcceptButton, partyMemberUpButton, partyMemberDownButton;

            //Texture2D partyMemberButtonTexture; //TODO: Make this graphic

            ////Party Member Editting Screen
            //Entity editedPartyMember;

            //Entity memberBackButton;
            //Entity[] memberInventoryButtons = new Entity[5];//This persons equiped weapons
            //Entity[,] communityInventoryButtons = new Entity[20, 20]; //Community Invetory

            //Party Editting Screen
            //Accept Button Creation
            //This button advances you to skirmish with the loadout



            partyMemberAcceptButton = new Entity(5, State.ScreenState.SKIRMISH_PREPARATION);
            Vector2 acceptPosition = new Vector2(400, 300);
            partyMemberAcceptButton.AddComponent(new SpriteComponent(true, acceptPosition, acceptButtonTexture));
            partyMemberAcceptButton.AddComponent(new ClickableComponent(acceptPosition, acceptButtonTexture.Width, acceptButtonTexture.Height));
            partyMemberButtonList.Add(partyMemberAcceptButton);
            EntityManager.AddEntity(partyMemberAcceptButton);

            //Scroll Up Button Creation
            partyMemberUpButton = new Entity(5, State.ScreenState.SKIRMISH_PREPARATION);
            Vector2 upPosition = new Vector2(550, -200);
            partyMemberUpButton.AddComponent(new SpriteComponent(true,upPosition, partyMemberUpButtonTexture));
            partyMemberUpButton.AddComponent(new ClickableComponent(upPosition, partyMemberUpButtonTexture.Width, partyMemberUpButtonTexture.Height));
            partyMemberButtonList.Add(partyMemberUpButton);
            EntityManager.AddEntity(partyMemberUpButton);

            //Scroll Down Button Creation
            partyMemberDownButton = new Entity(5, State.ScreenState.SKIRMISH_PREPARATION);
            Vector2 downPosition = new Vector2(550, 100);
            partyMemberDownButton.AddComponent(new SpriteComponent(true, downPosition, partyMemberDownButtonTexture));
            partyMemberDownButton.AddComponent(new ClickableComponent(downPosition, partyMemberDownButtonTexture.Width, partyMemberDownButtonTexture.Height));
            partyMemberButtonList.Add(partyMemberDownButton);
            EntityManager.AddEntity(partyMemberDownButton);

            Vector2 spaceForPartyMembers = new Vector2(State.screenWidth - 300, State.screenHeight - 500);
            currentlyViewedPartyButtons = new Entity[Convert.ToInt32(spaceForPartyMembers.X / partyMemberButtonTexture.Width), 
                                                               Convert.ToInt32(spaceForPartyMembers.Y / partyMemberButtonTexture.Height)];
            currentlyViewedPartyButtonPositions = new Vector2[Convert.ToInt32(spaceForPartyMembers.X / partyMemberButtonTexture.Width),
                                                               Convert.ToInt32(spaceForPartyMembers.Y / partyMemberButtonTexture.Height)];

            int xBuffer = Convert.ToInt32(spaceForPartyMembers.X / currentlyViewedPartyButtons.GetLength(0));
            int yBuffer = Convert.ToInt32(spaceForPartyMembers.Y / currentlyViewedPartyButtons.GetLength(1));
            for (int y = 0; y < currentlyViewedPartyButtonPositions.GetLength(0); y++)
            {
                for (int x = 0; x < currentlyViewedPartyButtonPositions.GetLength(1); x++)
                {
                    Vector2 position = new Vector2(-spaceForPartyMembers.X / 2 + x * xBuffer + x * partyMemberButtonTexture.Width,
                                                    -spaceForPartyMembers.Y / 2 + y * yBuffer + y * partyMemberButtonTexture.Height);
                    currentlyViewedPartyButtonPositions[x, y] = position;
                }
            }

            //Party Member Editting Buttons
            //Retursn you to party editting screen
            memberBackButton = new Entity(5, State.ScreenState.PARTY_MEMBER_EDITING);
            Vector2 backPosition = new Vector2(400, 300);
            memberBackButton.AddComponent(new SpriteComponent(true, backPosition, backButtonTexture));
            memberBackButton.AddComponent(new ClickableComponent(backPosition, backButtonTexture.Width, backButtonTexture.Height));
            partyMemberButtonList.Add(memberBackButton);
            EntityManager.AddEntity(memberBackButton);
        }

        #endregion

        #region Loading_Content_and_File_Processing

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Graphics\\Fonts\\Debug");
            State.font = font;


            string prefix = "C:\\Users\\TheNextGuy\\Dropbox\\Senior Project Material\\TxtFiles\\Development\\";


            ConvertTxtToBin(prefix + "Enemies.txt");
            ConvertTxtToBin(prefix + "Player_Roles.txt");
            ConvertTxtToBin(prefix + "Party_Members.txt");
            ConvertTxtToBin(prefix + "WorldMap.txt");


            ConvertTxtToBin(prefix + "Playtest_Map.txt");
            ConvertTxtToBin(prefix + "Playtest_Map_Enemies.txt");
            ConvertTxtToBin(prefix + "Playtest_Map_Dialogue.txt");

            ConvertTxtToBin(prefix + "Lab_Yard.txt");
            ConvertTxtToBin(prefix + "Lab_Yard_Enemies.txt");
            ConvertTxtToBin(prefix + "Lab_Yard_Dialogue.txt");

            ConvertTxtToBin(prefix + "Testing_Grounds.txt");
            ConvertTxtToBin(prefix + "Testing_Grounds_Enemies.txt");

            ConvertTxtToBin(prefix + "Tutorial_Level.txt");
            ConvertTxtToBin(prefix + "Tutorial_Level_Enemies.txt");

            ConvertTxtToBin(prefix + "Ambushed.txt");
            ConvertTxtToBin(prefix + "Ambushed_Enemies.txt");

            ConvertTxtToBin(prefix + "Pavilion.txt");
            ConvertTxtToBin(prefix + "Pavilion_Enemies.txt");


            font = Content.Load<SpriteFont>("Graphics\\Fonts\\chatboxFont");

            worldMapTexture = Content.Load<Texture2D>("Graphics\\Backgrounds\\island");
            pointerTexture = Content.Load<Texture2D>("Graphics\\Other\\pointer");
            nodeTexture = Content.Load<Texture2D>("Graphics\\Other\\node");

            hexBaseTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexBase");
            grassTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexGrass");
            gravelTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexGravel");
            sandTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexSand");
            woodTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexWood2");
            waterTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexWater1");
            stoneTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\stone path");
            carpetTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\red carpet");
            dirtTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Bases\\hexDirt");

            treeTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Decorations\\tree");
            bushTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Decorations\\bush");
            wallTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Decorations\\wooden Walls");
            questionTexture = Content.Load<Texture2D>("Graphics\\Other\\questionTexture");

            dialogueBackdropTexture = Content.Load<Texture2D>("Graphics\\Dialogue\\chatboxBackdrop");
            dialogueWideBackdropTexture = Content.Load<Texture2D>("Graphics\\Dialogue\\chatboxWideBackdrop");
            missingActorTexture = Content.Load<Texture2D>("Graphics\\Dialogue\\Actors\\missing");
            harryPortrait = Content.Load<Texture2D>("Graphics\\Dialogue\\Actors\\Harry");

            acceptButtonTexture = Content.Load<Texture2D>("Graphics\\Menu\\acceptButton");
            partyMemberButtonTexture = Content.Load<Texture2D>("Graphics\\Menu\\partyMemberBackdrop");
            partyMemberUpButtonTexture = Content.Load<Texture2D>("Graphics\\Menu\\partyMemberScrollUp");
            partyMemberDownButtonTexture = Content.Load<Texture2D>("Graphics\\Menu\\partyMemberScrollDown");

            PopulateTerrainDictionary();

            selectSound = Content.Load<SoundEffect>("Audio\\Sounds\\Powerup27");
            downSound = Content.Load<SoundEffect>("Audio\\Sounds\\down");
        }

        void PopulateTerrainDictionary()
        {
            terrainDictionary["G"] = new TerrainPackage(grassTexture, false, 0);//Grass
            terrainDictionary["D"] = new TerrainPackage(dirtTexture, false, 0);//Dirt
            terrainDictionary["L"] = new TerrainPackage(waterTexture, true, 0);//Water
            terrainDictionary["W"] = new TerrainPackage(woodTexture, false, 0);//Wood        
            terrainDictionary["S"] = new TerrainPackage(stoneTexture, false, 0);//Stone
            terrainDictionary["A"] = new TerrainPackage(sandTexture, false, 0);//Sand
            terrainDictionary["g"] = new TerrainPackage(gravelTexture, false, 0);//Gravel
            terrainDictionary["C"] = new TerrainPackage(carpetTexture, false, 0);//Carpet

            terrainDictionary["T"] = new TerrainPackage(treeTexture, true, 40);//Tree
            terrainDictionary["B"] = new TerrainPackage(bushTexture, false, 0);//Bush

            //terrainDictionary["t"] = new TerrainPackage(treeTexture, true);//Table
            //terrainDictionary["h"] = new TerrainPackage(treeTexture, true);//Throne
            //terrainDictionary["n"] = new TerrainPackage(treeTexture, true);//Tent

            terrainDictionary["X"] = new TerrainPackage(wallTexture, true, 50);
        }
        void PopulateUnitTextureDictionary()
        {
            axemanFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Axe"), 400f, 50, 100);
            unitTextureDictionary.Add("Axeman", axemanFramedTexture);

            battlemageFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Battlemage"), 400f, 50, 100);
            unitTextureDictionary.Add("Battlemage", battlemageFramedTexture);

            bowmanFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Bow"), 400f, 50, 100);
            unitTextureDictionary.Add("Bowman", bowmanFramedTexture);

            crossbowmanFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Crossbow"), 400f, 50, 100);
            unitTextureDictionary.Add("Crossbowman", crossbowmanFramedTexture);

            flailmanFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Flail"), 400f, 50, 100);
            unitTextureDictionary.Add("Flailman", flailmanFramedTexture);

            halberdierFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Halberd"), 400f, 50, 100);
            unitTextureDictionary.Add("Habledier", halberdierFramedTexture);

            knightFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Knight"), 400f, 50, 100);
            unitTextureDictionary.Add("Knight", knightFramedTexture);

            mageAssassinFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\MageAssassin"), 400f, 50, 100);
            unitTextureDictionary.Add("MageAssassin", mageAssassinFramedTexture);

            manAtArmsFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Man-at-Arms"), 400f, 50, 100);
            unitTextureDictionary.Add("ManAtArms", manAtArmsFramedTexture);

            pikemanFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Pike"), 400f, 50, 100);
            unitTextureDictionary.Add("Pikeman", pikemanFramedTexture);

            riflemanFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Rifle"), 400f, 50, 100);
            unitTextureDictionary.Add("Rifleman", riflemanFramedTexture);

            spearmanFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Spear"), 400f, 50, 100);
            unitTextureDictionary.Add("Spearman", spearmanFramedTexture);

            swordsmanFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Sword"), 400f, 50, 100);
            unitTextureDictionary.Add("Swordsman", swordsmanFramedTexture);

            wizardFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Wizard"), 400f, 50, 100);
            unitTextureDictionary.Add("Wizard", wizardFramedTexture);

            slimeFramedTexture = new Texture2DFramed(Content.Load<Texture2D>("Graphics\\UnitTextures\\Slime"), 400f, 50, 100);
            unitTextureDictionary.Add("Slime", slimeFramedTexture);
        }
        void PopulatePortraitDictionary()
        {
            portraitDictionary.Add("Harry", new PortraitPackage(missingActorTexture));
            portraitDictionary["Harry"].AddEmotionTexture(Emotion.Neutral, new Texture2D[2] { harryPortrait, harryPortrait });

            portraitDictionary.Add("Liam", new PortraitPackage(missingActorTexture));
            portraitDictionary.Add("Jon", new PortraitPackage(missingActorTexture));
            portraitDictionary.Add("Nosa", new PortraitPackage(missingActorTexture));

            portraitDictionary.Add("Klement", new PortraitPackage(missingActorTexture));
            portraitDictionary.Add("Jesper", new PortraitPackage(missingActorTexture));
            portraitDictionary.Add("Old Man", new PortraitPackage(missingActorTexture));
            portraitDictionary.Add("Kaijin", new PortraitPackage(missingActorTexture));

        }

        void ConvertTxtToBin(string myFilePath)
        {
            if (File.Exists(myFilePath))
            {
                FileStream txtFile = new FileStream(myFilePath, System.IO.FileMode.Open);
                using (StreamReader txtReader = new StreamReader(txtFile))
                {
                    //Create a new bin file with read unitName or overwrite it if it already exists
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
            //string[] files = Directory.GetFiles(Content.RootDirectory + "\\Binary");

            //C:\Users\Oliver\Documents\GitHub\SeniorSpringVideoGameProject\SeniorProjectGame\SeniorProjectGame\SeniorProjectGameContent\Binary\Alchemist's_Laboratory.bin
            if (File.Exists(Content.RootDirectory + "\\" + myID + ".bin"))
            {
                FileStream binFile = new FileStream(Content.RootDirectory + "//" + myID + ".bin", System.IO.FileMode.Open);
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
                throw new Exception("Content//" + myID + ".bin" + " doesn't exist. Did you load the txt of it?");
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
        void ProcessPlayerRolesBin()
        {
            List<string> binLines = ReadBin("Player_Roles");

            List<string> relevantLines = new List<string>();

            for (int line = 0; line < binLines.Count; line++)
            {
                if (!binLines[line].Contains("//") && binLines[line] != "")
                {
                    relevantLines.Add(binLines[line]);
                }
            }

            for (int line = 0; line < relevantLines.Count; line++)
            {
                if (relevantLines[line].Contains("-"))
                {
                    string nameOfRole = relevantLines[line].Split(' ')[1];
                    string[] statLine = relevantLines[line + 1].Split(' ');
                    string[] growthLine = relevantLines[line + 2].Split(' ');
                    string[] capLine = relevantLines[line + 3].Split(' ');
                    string weapon = relevantLines[line + 4];
                    string[] magicLine = relevantLines[line + 5].Split(' ');
                    string[] movementLine = relevantLines[line + 6].Split(' ');

                    int str = Convert.ToInt32(statLine[0]);
                    int mag = Convert.ToInt32(statLine[1]);
                    int dex = Convert.ToInt32(statLine[2]);
                    int agi = Convert.ToInt32(statLine[3]);
                    int def = Convert.ToInt32(statLine[4]);
                    int res = Convert.ToInt32(statLine[5]);
                    int spd = Convert.ToInt32(statLine[6]);

                    float strGrowth = float.Parse(growthLine[0]);
                    float magGrowth = float.Parse(growthLine[1]);
                    float dexGrowth = float.Parse(growthLine[2]);
                    float agiGrowth = float.Parse(growthLine[3]);
                    float defGrowth = float.Parse(growthLine[4]);
                    float resGrowth = float.Parse(growthLine[5]);
                    float spdGrowth = float.Parse(growthLine[6]);

                    int strCap = Convert.ToInt32(capLine[0]);
                    int magCap = Convert.ToInt32(capLine[1]);
                    int dexCap = Convert.ToInt32(capLine[2]);
                    int agiCap = Convert.ToInt32(capLine[3]);
                    int defCap = Convert.ToInt32(capLine[4]);
                    int resCap = Convert.ToInt32(capLine[5]);
                    int spdCap = Convert.ToInt32(capLine[6]);


                    bool light = bool.Parse(magicLine[0]);
                    bool anima = bool.Parse(magicLine[0]);
                    bool dark = bool.Parse(magicLine[0]);

                    int movement = Convert.ToInt32(movementLine[0]);
                    int sightRange = Convert.ToInt32(movementLine[1]);
                    int attackRange = Convert.ToInt32(movementLine[2]);

                    Role role = new Role(
                                        str, mag, dex, agi, def, res, spd,
                                        strGrowth, magGrowth, dexGrowth, agiGrowth, defGrowth, resGrowth, spdGrowth,
                                        strCap, magCap, dexCap, agiCap, defCap, resCap, spdCap,
                                        weapon,
                                        light, anima, dark,
                                        movement, sightRange, attackRange);
                    classes[nameOfRole] = role;
                }
            }

        }
        void ProcessEnemyBestiaryBin()
        {
            //List<Entity> enemyEntityList = new List<Entity>();
            List<string> binLines = ReadBin("Enemies");

            List<string> relevantLines = new List<string>();

            for (int line = 0; line < binLines.Count; line++)
            {
                if (!binLines[line].Contains("//") && binLines[line] != "")
                {
                    relevantLines.Add(binLines[line]);
                }
            }

            for (int line = 0; line < relevantLines.Count; line++)
            {
                if (relevantLines[line].Contains("-"))
                {
                    string nameOfRole = relevantLines[line].Split(' ')[1];
                    string[] statLine = relevantLines[line + 1].Split(' ');
                    string[] growthLine = relevantLines[line + 2].Split(' ');
                    string[] capLine = relevantLines[line + 3].Split(' ');
                    string weapon = relevantLines[line + 4];
                    string[] magicLine = relevantLines[line + 5].Split(' ');
                    string[] movementLine = relevantLines[line + 6].Split(' ');

                    int str = Convert.ToInt32(statLine[0]);
                    int mag = Convert.ToInt32(statLine[1]);
                    int dex = Convert.ToInt32(statLine[2]);
                    int agi = Convert.ToInt32(statLine[3]);
                    int def = Convert.ToInt32(statLine[4]);
                    int res = Convert.ToInt32(statLine[5]);
                    int spd = Convert.ToInt32(statLine[6]);

                    float strGrowth = float.Parse(growthLine[0]);
                    float magGrowth = float.Parse(growthLine[1]);
                    float dexGrowth = float.Parse(growthLine[2]);
                    float agiGrowth = float.Parse(growthLine[3]);
                    float defGrowth = float.Parse(growthLine[4]);
                    float resGrowth = float.Parse(growthLine[5]);
                    float spdGrowth = float.Parse(growthLine[6]);

                    int strCap = Convert.ToInt32(capLine[0]);
                    int magCap = Convert.ToInt32(capLine[1]);
                    int dexCap = Convert.ToInt32(capLine[2]);
                    int agiCap = Convert.ToInt32(capLine[3]);
                    int defCap = Convert.ToInt32(capLine[4]);
                    int resCap = Convert.ToInt32(capLine[5]);
                    int spdCap = Convert.ToInt32(capLine[6]);


                    bool light = bool.Parse(magicLine[0]);
                    bool anima = bool.Parse(magicLine[0]);
                    bool dark = bool.Parse(magicLine[0]);

                    int movement = Convert.ToInt32(movementLine[0]);
                    int sightRange = Convert.ToInt32(movementLine[1]);
                    int attackRange = Convert.ToInt32(movementLine[2]);

                    Role role = new Role(
                                        str, mag, dex, agi, def, res, spd,
                                        strGrowth, magGrowth, dexGrowth, agiGrowth, defGrowth, resGrowth, spdGrowth,
                                        strCap, magCap, dexCap, agiCap, defCap, resCap, spdCap,
                                        weapon,
                                        light, anima, dark,
                                        movement, sightRange, attackRange);
                    classes[nameOfRole] = role;
                }
            }
        }

        void ProcessPartyMembersBin()
        {
            List<string> binLines = ReadBin("Party_Members");

            List<string> relevantLines = new List<string>();
            for (int line = 2; line < binLines.Count; line++)
            {
                if (binLines[line] != "" && !binLines[line].Contains("//"))
                    relevantLines.Add(binLines[line]);
            }

            for (int line = 0; line < relevantLines.Count; line++)
            {
                if (relevantLines[line].Contains("-"))
                {
                    string[] nameLine = relevantLines[line].Split(' ');
                    string[] statLine = relevantLines[line + 1].Split(' ');
                    string[] growthLine = relevantLines[line + 2].Split(' ');
                    string[] capLine = relevantLines[line + 3].Split(' ');
                    string[] movementLine = relevantLines[line + 4].Split(' ');


                    string name = nameLine[1];
                    Role role = classes[nameLine[2]];
                    int level = Convert.ToInt32(nameLine[3]);
                    string graphicName = nameLine[4];

                    int str = Convert.ToInt32(statLine[0]);
                    int mag = Convert.ToInt32(statLine[1]);
                    int dex = Convert.ToInt32(statLine[2]);
                    int agi = Convert.ToInt32(statLine[3]);
                    int def = Convert.ToInt32(statLine[4]);
                    int res = Convert.ToInt32(statLine[5]);
                    int spd = Convert.ToInt32(statLine[6]);

                    float strGrowth = float.Parse(growthLine[0]);
                    float magGrowth = float.Parse(growthLine[1]);
                    float dexGrowth = float.Parse(growthLine[2]);
                    float agiGrowth = float.Parse(growthLine[3]);
                    float defGrowth = float.Parse(growthLine[4]);
                    float resGrowth = float.Parse(growthLine[5]);
                    float spdGrowth = float.Parse(growthLine[6]);

                    int strCap = Convert.ToInt32(capLine[0]);
                    int magCap = Convert.ToInt32(capLine[1]);
                    int dexCap = Convert.ToInt32(capLine[2]);
                    int agiCap = Convert.ToInt32(capLine[3]);
                    int defCap = Convert.ToInt32(capLine[4]);
                    int resCap = Convert.ToInt32(capLine[5]);
                    int spdCap = Convert.ToInt32(capLine[6]);

                    int movement = Convert.ToInt32(movementLine[0]);
                    int sightRange = Convert.ToInt32(movementLine[1]);
                    int attackRange = Convert.ToInt32(movementLine[2]);

                    Entity partyMemberEntity = new Entity(EntityManager.GetHighestLayer() + 1, State.ScreenState.SKIRMISH);

                    UnitData tempUnitData = new UnitData(
                                        name, role, Alignment.Player, level,
                                        str, mag, dex, agi, def, res, spd,
                                        strGrowth, magGrowth, dexGrowth, agiGrowth, defGrowth, resGrowth, spdGrowth,
                                        strCap, magCap, dexCap, agiCap, defCap, resCap, spdCap,
                                        movement, sightRange, attackRange);

                    Vector2 coordinate = boardComponent.GetOneAlliedSpawnPoint(rand);
                    SpriteComponent hexSprite = boardComponent.GetHex(coordinate)._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                    partyMemberEntity.AddComponent(new UnitSpriteComponent(true, hexSprite.GetCenterPosition(), unitTextureDictionary[graphicName]));

                    partyMemberEntity.AddComponent(new UnitComponent(boardComponent.GetHex(coordinate), true));
                    (partyMemberEntity.GetComponent("UnitComponent") as UnitComponent).SetUnitData(tempUnitData);

                    EntityManager.AddEntity(partyMemberEntity);
                    boardComponent.GetHex(coordinate).SetUnit(partyMemberEntity.GetComponent("UnitComponent") as UnitComponent);

                    boardComponent.alliedUnitList.Add(partyMemberEntity);
                    AddPartyMember(partyMemberEntity);
                }
            }
            boardComponent.UpdateVisibilityAllies();
        }
        void AddPartyMember(Entity myPartyMember)
        {
            partyMemberList.Add(myPartyMember);

            Entity button = new Entity(5, State.ScreenState.SKIRMISH_PREPARATION);
            button.AddComponent(new SpriteComponent(true, Vector2.Zero, partyMemberButtonTexture));

            UnitComponent unit = myPartyMember.GetComponent("UnitComponent") as UnitComponent;
            UnitData data = unit.GetUnitData();
            button.AddComponent(new TextSpriteComponent(false,data.GetName(),Color.White,Vector2.Zero,font));

            button.AddComponent(new ClickableComponent(Vector2.Zero, partyMemberButtonTexture.Width, partyMemberButtonTexture.Height));
            partyMemberButtonList.Add(button);
            EntityManager.AddEntity(button);
        }

        Entity ProcessHexMapBin(string myID)
        {
            List<string> binLines = ReadBin(myID);

            int layers = Convert.ToInt32(binLines[0]);
            Vector2 dimensions = new Vector2(float.Parse(binLines[1].Split(' ')[0]), float.Parse(binLines[1].Split(' ')[1]));

            Entity tempBoard = new Entity(0, State.ScreenState.SKIRMISH);
            BoardComponent tempBoardComponent = new BoardComponent(dimensions, hexBaseTexture, font);
            tempBoard.AddComponent(tempBoardComponent);

            EntityManager.AddEntity(tempBoard);

            //Removing the user-friendly formatting from the bin
            List<string> hexMapLines = new List<string>();
            for (int line = 2; line < binLines.Count; line++)
            {
                if (binLines[line] != "" && !binLines[line].Contains("//"))
                    hexMapLines.Add(Regex.Replace(binLines[line], "  ", " "));
            }

            //Reading the terrian baseHexLayer
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
                            tempBoardComponent.AddTerrain(ConvertToHexCoordinate(terrainCoordinate), layer, GetTerrain(line[x]));
                        }
                    }
                }
            }

            //Reading the objective baseHexLayer
            for (int y = 0 + ((int)dimensions.Y * layers); y < dimensions.Y + ((int)dimensions.Y * layers); y++)
            {
                string[] line = hexMapLines[y].Split(' ');

                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] != "*")
                    {
                        if (!line[x].Contains('E'))
                        {
                            if (line[x] == "Z")
                            {
                                //Add allied spawn
                                tempBoardComponent.AddAlliedSpawnPoint(ConvertToHexCoordinate(new Vector2(x, y - ((int)dimensions.Y * layers))));
                            }

                            else if (line[x] == "V")
                            {
                                //Victory Point
                                State.screenState = State.ScreenState.DIALOGUE;
                                ChatboxManager.SetEventName("Beginning");
                                
                                ChatboxManager.SetNewInfo(ProcessHexMapDialogue(worldMapComponent.GetCurrentNodeID(), ChatboxManager.GetEvent()));
                            }
                            else if (line[x] == "C")
                            {
                                //Camera location
                                HexComponent hex = tempBoardComponent.GetHex(ConvertToHexCoordinate(new Vector2(x, y - ((int)dimensions.Y * layers))));
                                SpriteComponent hexSprite = hex._parent.GetDrawable("SpriteComponent") as SpriteComponent;


                                Camera.MoveTo(hexSprite.centerScreenPosition);
                            }
                            else
                            {
                                //Enemy Spawn
                                tempBoardComponent.AddEnemySpawnPoint(Convert.ToInt32(line[x]), ConvertToHexCoordinate(new Vector2(x, y - ((int)dimensions.Y * layers))));
                            }

                        }
                        else 
                        {
                            //We found an event
                        }

                    }
                }
            }
            return tempBoard;


        }
        void ProcessHexMapEnemyBin(string myID)
        {
            List<string> binLines = ReadBin(myID + "_Enemies");

            List<string> relevantLines = new List<string>();
            for (int line = 2; line < binLines.Count; line++)
            {
                if (binLines[line] != "" && !binLines[line].Contains("//"))
                    relevantLines.Add(binLines[line]);
            }
            for (int line = 0; line < relevantLines.Count; line++)
            {
                if (relevantLines[line].Contains("-"))
                {
                    string[] nameLine = relevantLines[line].Split(' ');
                    string[] statLine = relevantLines[line + 1].Split(' ');
                    string[] growthLine = relevantLines[line + 2].Split(' ');
                    string[] capLine = relevantLines[line + 3].Split(' ');
                    string[] movementLine = relevantLines[line + 4].Split(' ');

                    int unitSpawn = Convert.ToInt32(nameLine[1]);
                    string name = nameLine[2];
                    Role role = classes[nameLine[3]];
                    Alignment align = (Alignment)Enum.Parse(typeof(Alignment), nameLine[4], true); 

                    int level = Convert.ToInt32(nameLine[5]);
                    string graphicName = nameLine[6];

                    int str = Convert.ToInt32(statLine[0]);
                    int mag = Convert.ToInt32(statLine[1]);
                    int dex = Convert.ToInt32(statLine[2]);
                    int agi = Convert.ToInt32(statLine[3]);
                    int def = Convert.ToInt32(statLine[4]);
                    int res = Convert.ToInt32(statLine[5]);
                    int spd = Convert.ToInt32(statLine[6]);

                    float strGrowth = float.Parse(growthLine[0]);
                    float magGrowth = float.Parse(growthLine[1]);
                    float dexGrowth = float.Parse(growthLine[2]);
                    float agiGrowth = float.Parse(growthLine[3]);
                    float defGrowth = float.Parse(growthLine[4]);
                    float resGrowth = float.Parse(growthLine[5]);
                    float spdGrowth = float.Parse(growthLine[6]);

                    int strCap = Convert.ToInt32(capLine[0]);
                    int magCap = Convert.ToInt32(capLine[1]);
                    int dexCap = Convert.ToInt32(capLine[2]);
                    int agiCap = Convert.ToInt32(capLine[3]);
                    int defCap = Convert.ToInt32(capLine[4]);
                    int resCap = Convert.ToInt32(capLine[5]);
                    int spdCap = Convert.ToInt32(capLine[6]);

                    int movement = Convert.ToInt32(movementLine[0]);
                    int sightRange = Convert.ToInt32(movementLine[1]);
                    int attackRange = Convert.ToInt32(movementLine[2]);


                    //For every spawn point associated with 1, 2, 3 make the right amount of enemies
                    for (int createdEnemies = 0; createdEnemies <= boardComponent.enemySpawnPoints[unitSpawn].Count; createdEnemies++)
                    {
                        Vector2 hexLocation = boardComponent.GetEnemySpawnPointForType(unitSpawn);
                        HexComponent hex = boardComponent.GetHex(hexLocation);
                        SpriteComponent hexSprite = hex._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                        Entity blob = new Entity(EntityManager.GetHighestLayer() + 1, State.ScreenState.SKIRMISH);
                        blob.AddComponent(new UnitSpriteComponent(true, hexSprite.GetPosition(), unitTextureDictionary[graphicName]));
                        blob.AddComponent(new UnitComponent(hex, false));

                        hex.SetUnit(blob.GetComponent("UnitComponent") as UnitComponent);

                        hex.GetUnit().SetUnitData(new UnitData(
                                            name, role, Alignment.Enemy, level,
                                            str, mag, dex, agi, def, res, spd,
                                            strGrowth, magGrowth, dexGrowth, agiGrowth, defGrowth, resGrowth, spdGrowth,
                                            strCap, magCap, dexCap, agiCap, defCap, resCap, spdCap,
                                            movement, sightRange, attackRange));
                        EntityManager.AddEntity(blob);
                        boardComponent.nonAlliedUnitList.Add(blob);
                        boardComponent.totalUnitList.Add(blob);
                    }


                }
            }
        }

        //Read this every time you want an event, handle for null return values
        //Returns a unproccessed, dialogue lines, for instance "0000 Liam Let's do it!" is an element
        List<string> ProcessHexMapDialogue(string myID, string myEventName)
        {
            List<string> binLines = ReadBin(myID + "_Dialogue");

            List<string> relevantLines = new List<string>();
            for (int line = 0; line < binLines.Count; line++)
            {
                if (binLines[line] != "" && !binLines[line].Contains("//"))
                    relevantLines.Add(binLines[line]);
            }

            List<string> dialogueLines = new List<string>();

            for (int lineIndex = 0; lineIndex < relevantLines.Count; lineIndex++)
            {
                if (relevantLines[lineIndex].Contains("-") && relevantLines[lineIndex].Contains(myEventName))
                {
                    int lineBuffer = 1;
                    string currentLine = relevantLines[lineIndex + lineBuffer];
                    while (!currentLine.Contains("-") && lineIndex + lineBuffer < relevantLines.Count)
                    {
                        dialogueLines.Add(relevantLines[lineIndex + lineBuffer]);
                        lineBuffer++;
                        if (lineIndex + lineBuffer < relevantLines.Count)
                        {
                            currentLine = relevantLines[lineIndex + lineBuffer];
                        }
                    }

                }
            }
            return dialogueLines;
        }

        #endregion

        #region Saving_Content_and_File_Processing

        void SavePartyMembersBin()
        {

        }
        void SaveWorldMapBin()
        {

        }

        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (escapeClick.Evaluate())
            {
                this.Exit();
            }

            //Calculating the FPS, doesn't go above 60
            elapsedTimeForFps += gameTime.ElapsedGameTime;
            if (elapsedTimeForFps > TimeSpan.FromSeconds(1))
            {
                elapsedTimeForFps -= TimeSpan.FromSeconds(1);
                framesPerSecond = numberOfFrames;
                numberOfFrames = 0;
            }

            if (State.selectionState != State.SelectionState.SelectingMenuOptions
                && (State.screenState == State.ScreenState.SKIRMISH || State.screenState == State.ScreenState.WORLD_MAP))//|| State.screenState == State.ScreenState.SKIRMISH_PREPARATION))
            {
                if (wClick.Evaluate())
                {
                    Camera.Move(new Vector2(0, -5));
                }
                if (dClick.Evaluate())
                {
                    Camera.Move(new Vector2(5, 0));
                }
                if (sClick.Evaluate())
                {
                    Camera.Move(new Vector2(0, 5));
                }
                if (aClick.Evaluate())
                {
                    Camera.Move(new Vector2(-5, 0));
                }
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
                            if (click.isColliding(new Vector2(InputState.GetMouseIngamePosition().X, InputState.GetMouseIngamePosition().Y)))
                            {
                                selectSound.Play();
                                NodeComponent node = click._parent.GetComponent("NodeComponent") as NodeComponent;
                                worldMapComponent.SetSelectedNode(node);
                            }
                        }
                        //TODO: See if you are clicking on "START LEVEL" button and use that instead
                        //Also have a nice gui that shows the level and what not
                    }
                    if (enterClick.Evaluate())
                    {
                        //TODO: Play a resounding start node sound
                        //For now enter click will do
                        StartNode();
                    }

                    break;
                #endregion

                #region Shop
                case State.ScreenState.SHOP:
                    break;
                #endregion

                #region Skirmish_Prep
                case State.ScreenState.SKIRMISH_PREPARATION:

                    if (singleLeftClick.Evaluate())
                    {
                        ClickableComponent acceptClick = partyMemberAcceptButton.GetComponent("ClickableComponent") as ClickableComponent;
                        if (acceptClick.isColliding(new Vector2(InputState.GetMouseIngamePosition().X, InputState.GetMouseIngamePosition().Y)))
                        {
                            selectSound.Play();
                            //TODO: Load the info!
                            Camera.MoveTo(skirmishCameraPosition);
                            State.screenState = State.ScreenState.SKIRMISH;
                        }
                        for (int p = 0; p < partyMemberButtonList.Count; p++)
                        {
                            //Cycle through every party member button to see if you are clicking
                            ClickableComponent click = partyMemberButtonList[p].GetComponent("ClickableComponent") as ClickableComponent;
                            if (click.isColliding(new Vector2(InputState.GetMouseIngamePosition().X, InputState.GetMouseIngamePosition().Y)))
                            {
                                /*
                                if(doubleClick)
                                {
                                   //Go into the party memebers invertory screen
                                   State.screenState = State.ScreenState.PARTY_MEMBER_EDITING;
                                }
                                */
                                selectSound.Play();
                                //Take care that when you reorder the buttons you also reorder the party member list
                                editedPartyMember = partyMemberList[p];
                            }
                        }
                        //TODO: Handle reoderign buttons

                        //TODO: Handle scrolling over party members
                    }

                    break;
                #endregion

                #region Party_Member_Editing

                case State.ScreenState.PARTY_MEMBER_EDITING:

                    if (singleLeftClick.Evaluate())
                    {
                        //TODO: Handle for holding inventory and equiping it

                        ClickableComponent backButtonClick = memberBackButton.GetComponent("ClickableComponent") as ClickableComponent;
                        if (backButtonClick.isColliding(new Vector2(InputState.GetMouseIngamePosition().X, InputState.GetMouseIngamePosition().Y)))
                        {
                            selectSound.Play();
                            //TODO: Save all player information!
                            //TODO: Load the info!
                            State.screenState = State.ScreenState.SKIRMISH_PREPARATION;
                        }
                    }

                    break;

                #endregion

                #region Dialogue
                case State.ScreenState.DIALOGUE:

                    ChatboxManager.Update(gameTime);

                    if (ChatboxManager.GetStatus() == ChatboxStatus.Writing)
                    {
                        doubleClickTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (singleLeftClick.Evaluate())
                        {
                            if (doubleClickTimer < State.doubleClickSpeed)
                            {
                                ChatboxManager.Instawrite();
                            }
                            doubleClickTimer = 0;
                        }
                        if (leftHold.Evaluate())
                        {
                            ChatboxManager.RushTyping();
                        }
                        else
                        {
                            ChatboxManager.SlowTyping();
                        }

                    }
                    else if (ChatboxManager.GetStatus() == ChatboxStatus.WaitingInput)
                    {
                        if (singleLeftClick.Evaluate())
                        {
                            ChatboxManager.Advance();
                        }
                    }
                    else if (ChatboxManager.GetStatus() == ChatboxStatus.Finished)
                    {
                        if (ChatboxManager.GetEvent() == "Beginning")
                        {
                            State.screenState = State.ScreenState.SKIRMISH_PREPARATION;
                        }

                        if (ChatboxManager.GetEvent() == "Defeat")
                        {
                            State.screenState = State.ScreenState.WORLD_MAP;
                        }
                        else if (ChatboxManager.GetEvent() == "Victory")
                        {
                            State.screenState = State.ScreenState.SKIRMISH_RESOLUTION;
                        }

                        else
                        {
                            State.screenState = State.ScreenState.SKIRMISH;
                        }
                    }


                    break;
                #endregion

                #region Skirmish
                case State.ScreenState.SKIRMISH:
                    if (State.turnState == null)
                        State.turnState = State.TurnState.AlliesTurn;

                    #region AlliesTurn
                    UpdateEnemiesSeen(); //TODO: this is a hacky fix to a visual bug we have. by "refreshing" the sight, it gets rid of vision errors.
                    if (State.turnState == State.TurnState.AlliesTurn)
                    {
                        if (!moving)
                        {
                            if (leftHold.Evaluate())
                            {
                                HexComponent hexComp = boardComponent.GetMouseHex();

                                Entity hexEntity = hexComp._parent;

                                if (hexComp.HasUnit() && hexComp.GetUnit().GetSelectable() && State.selectionState == State.SelectionState.NoSelection)
                                {
                                    UnitComponent unit = hexComp.GetUnit();
                                    unit.SetSelected(true);
                                    State.selectionState = State.SelectionState.SelectingUnit;

                                    State.originalHexClicked = hexComp;
                                    ghostHex = hexComp;
                                }
                                if (hexComp == State.originalHexClicked)
                                {
                                    int originalPathQueueCount = pathQueue.Count();
                                    if (pathQueue.Count > 0)
                                    {
                                        for (int i = 0; i < pathQueue.Count; i++)
                                        {
                                            pathQueue[i].SetInQueue(false);
                                        }
                                        State.originalHexClicked.GetUnit().ChangeMovesLeft(originalPathQueueCount);
                                        pathQueue.Clear();
                                        ghostHex = hexComp;
                                    }
                                    //else
                                    //{
                                    //    State.selectionState = State.SelectionState.NoSelection;
                                    //    State.originalHexClicked.GetUnit().SetSelected(false);
                                    //    State.originalHexClicked = null;
                                    //}                                
                                }
                                else if (!hexComp.HasUnit() && State.selectionState == State.SelectionState.SelectingUnit && hexComp.GetVisibility() != Visibility.Unexplored)
                                {
                                    UnitComponent currentUnit = State.originalHexClicked.GetUnit();
                                    if (!hexComp.ContainsImpassable())
                                    {
                                        if (!pathQueue.Contains(hexComp) && AreAdjacent(hexComp, ghostHex))
                                        {
                                            if (currentUnit.GetMovesLeft() > 0)
                                            {
                                                //add hex you click on to pathQueue
                                                pathQueue.Add(hexComp);
                                                hexComp.SetInQueue(true);
                                                currentUnit.ChangeMovesLeft(-1);
                                                ghostHex = hexComp;
                                            }
                                        }
                                        else if (pathQueue.Contains(hexComp))
                                        {
                                            int originalpathQueueCount = pathQueue.Count;
                                            while (true)
                                            {
                                                if (pathQueue[pathQueue.Count - 1] != hexComp)
                                                {
                                                    //remove hexes from pathQueue until you reach the one you click on
                                                    pathQueue[pathQueue.Count - 1].SetInQueue(false);
                                                    pathQueue.Remove(pathQueue[pathQueue.Count - 1]);
                                                    currentUnit.ChangeMovesLeft(1);
                                                }
                                                else
                                                {
                                                    ghostHex = hexComp;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                //else if (State.selectionState == State.SelectionState.SelectingUnit)
                                //{
                                //    State.selectionState = State.SelectionState.SelectingOptionsForSkirmishUnits;
                                //}
                                //else if (State.selectionState == State.SelectionState.SelectingOptionsForSkirmishUnits)
                                //{
                                //    MoveUnit(State.originalHexClicked, boardComponent.GetMouseHex());
                                //    State.selectionState = State.SelectionState.NoSelection;
                                //    State.originalHexClicked = null;
                                //}
                            }
                        }

                        // handles the actions when you left click while selecting an option
                        if ((leftHold.Evaluate() || enterClick.Evaluate()) && State.selectionState == State.SelectionState.SelectingMenuOptions)
                        {
                            if (menu.CurrentOptionIndex() != -1)
                            {
                                string option = menu.Options()[menu.CurrentOptionIndex()];

                                switch (option)
                                {
                                    case "Wait":
                                        menu.Hide();
                                        State.selectionState = State.SelectionState.NoSelection;
                                        menu.SetSelectedOption(0);
                                        break;
                                    case "Trade":
                                        break;
                                    case "Heal":
                                        break;
                                    case "Convoy":
                                        break;
                                    case "Seize":
                                        break;
                                    case "Negotiate":
                                        break;
                                    case "Attack":
                                        // todo: initiate battle

                                        // currently the fight mechanic only selects the first
                                        // adjacent enemy. in the future, TODO: allow user to 
                                        // select the enemy to fight.

                                        menu.Hide();
                                        State.selectionState = State.SelectionState.NoSelection;
                                        menu.SetSelectedOption(0);

                                        int selectedEnemyIndex = 0;

                                        StartFight(State.originalHexClicked.GetUnit(),
                                            enemiesAdjacentTo(boardComponent, State.originalHexClicked.GetCoordPosition())[selectedEnemyIndex]);

                                        // todo: end battle
                                        // EndCurrentFight();
                                        break;
                                    default:
                                        break;
                                }

                            }
                        }

                        if (moving)
                        {
                            if (spaceHold.Evaluate())
                            {
                                //  hold space to accelerate
                                timePerMove = 120;
                            }
                            else
                            {
                                timePerMove = 360;
                            }
                            State.elapsedTimeForMove += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                            //animated movement
                            UnitComponent unit = State.originalHexClicked.GetUnit();
                            UnitSpriteComponent sprite = unit._parent.GetDrawable("UnitSpriteComponent") as UnitSpriteComponent;
                            SpriteComponent originalHexSprite = State.originalHexClicked._parent.GetDrawable("SpriteComponent") as SpriteComponent;
                            SpriteComponent finalHexSprite = pathQueue[0]._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                            float percentTraveled = State.elapsedTimeForMove / timePerMove;
                            sprite.SetPosition(originalHexSprite.GetCenterPosition() + (-originalHexSprite.GetCenterPosition() + finalHexSprite.GetCenterPosition()) * percentTraveled);

                            if (State.elapsedTimeForMove > timePerMove)
                            {
                                State.elapsedTimeForMove = State.elapsedTimeForMove - timePerMove;

                                pathQueue[0].SetInQueue(false);
                                MoveUnit(State.originalHexClicked, pathQueue[0]);
                                State.originalHexClicked = pathQueue[0];
                                
                                //if (CheckForEventsAtCoordinate(pathQueue[0].GetCoordPosition()) != null)
                                //{
                                //    ChatboxManager.SetEventName(CheckForEventsAtCoordinate(pathQueue[0].GetCoordPosition()).eventName);
                                //    ChatboxManager.SetNewInfo(ProcessHexMapDialogue(worldMapComponent.GetCurrentNodeID(), ChatboxManager.GetEvent()));
                                //    State.screenState = State.ScreenState.DIALOGUE;

                                //    CheckForEventsAtCoordinate(pathQueue[0].GetCoordPosition()).Run();
                                //}

                                pathQueue.Remove(pathQueue[0]);
                                CheckToStopForNewEnemies(State.originalHexClicked.GetUnit());

                                //Woah, we are checking to see if you triggered an event




                                if (pathQueue.Count == 0)
                                {
                                    State.elapsedTimeForMove = 0;

                                    State.originalHexClicked.GetUnit().SetSelected(false);
                                    State.selectionState = State.SelectionState.NoSelection;
                                    UpdateTurnState();
                                    AvailableToMoveCheck();

                                    moving = false;
                                }
                            }
                        }

                        if (enterClick.Evaluate())
                        {
                            if (pathQueue.Count != 0)
                            {
                                //UnitComponent unit = State.originalHexClicked.GetUnit();
                                //AnimatedSpriteComponent sprite = unit._parent.GetDrawable("AnimatedSpriteComponent") as AnimatedSpriteComponent;
                                //sprite._visible = false;

                                moving = true;
                            }
                            else if (State.selectionState != State.SelectionState.SelectingMenuOptions)
                            {
                                AvailableToMoveCheck();

                                State.selectionState = State.SelectionState.NoSelection;

                                if (State.originalHexClicked != null)
                                {
                                    State.originalHexClicked.GetUnit().SetSelected(false);
                                    State.originalHexClicked = null;
                                }
                            }
                            if (!moving)
                            {
                                UpdateTurnState();
                                AvailableToMoveCheck();
                            }
                        }

                        else if (singleRightClick.Evaluate())
                        {
                            HexComponent hexComp = boardComponent.GetMouseHex();
                            if (State.selectionState != State.SelectionState.SelectingMenuOptions)
                            {
                                if (hexComp.HasUnit())
                                {
                                    UnitComponent unitComp = hexComp.GetUnit();
                                    if (boardComponent.alliedUnitList.Contains(unitComp._parent))
                                    {
                                        List<string> options = new List<string>(new string[] { "Items", "Wait" });
                                        int skirmishMenuX = 300;
                                        int skirmishMenuY = 200;
                                        int skirmishMenuOptionHeight = 40;
                                        int skirmishMenuOptionWidth = 200;
                                        Color skirmishMenuColor = Color.DarkGray;

                                        if (enemiesAdjacentTo(boardComponent, hexComp.GetCoordPosition()).Count > 0)
                                        {
                                            options.Insert(0, "Attack"); //  have "attack" as an option if enemy nearby
                                        }

                                        if (alliesAdjacentTo(boardComponent, hexComp.GetCoordPosition()).Count > 0)
                                        {
                                            options.Insert(0, "Heal"); //  have "attack" as an option if enemy nearby
                                        }

                                        // State.originalHexClicked = null;

                                        // todo: pseudocode:
                                        //if enemyUnitIsAdjacent, options.Add("Attack");
                                        // if alliedUnitIsAdjacent && healStaffEquipped, options.Add(new string[] {"Heal", "Trade"});
                                        // if convoyIsAdjacent, options.Add("Convoy");
                                        // if neutralUnitIsAdjacent, options.Add("Negotiate");
                                        // if lordSelected && standingOnObjective, options.Add("Seize");

                                        menu = new Menu(options, skirmishMenuOptionWidth, skirmishMenuOptionHeight, skirmishMenuX, skirmishMenuY,
                                            skirmishMenuColor, dot, font);

                                        State.selectionState = State.SelectionState.SelectingMenuOptions;
                                    }
                                }
                            }
                        }

                        if (singleMiddleClick.Evaluate())
                        {
                            boardComponent.UpdateVisibilityAllies();
                        }

                        if (State.selectionState == State.SelectionState.SelectingMenuOptions)
                        {
                            if (singleWClick.Evaluate())
                            {
                                menu.SetSelectedOption((menu.Options().Count + menu.CurrentOptionIndex() - 1) % menu.Options().Count);
                            }
                            else if (singleSClick.Evaluate())
                            {
                                menu.SetSelectedOption((menu.CurrentOptionIndex() + 1) % menu.Options().Count);
                            }
                            else
                            {
                                for (int i = 0; i < menu.Options().Count; i++)
                                {
                                    if (menu.Hitboxes()[i].isColliding(InputState.GetMouseScreenPosition()))
                                    {
                                        menu.SetSelectedOption(i);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region EnemiesTurn
                    if (State.turnState == State.TurnState.EnemiesTurn)
                    {
                        Entity enemy = null;
                        if (State.enemyMoveIndex < boardComponent.nonAlliedUnitList.Count())
                        {
                            enemy = boardComponent.nonAlliedUnitList[State.enemyMoveIndex];
                        }
                        else
                        {
                            State.enemyMoveIndex = 0;
                            ResetTurnsLeft();
                            UpdateTurnState();
                            AvailableToMoveCheck();

                            State.firstTimeEnemyTurn = true;
                            return;
                        }
                        UnitComponent unitComp = enemy.GetComponent("UnitComponent") as UnitComponent;

                        List<HexComponent> visibleHexes = GetVisibleHexes(enemy);

                        HexComponent destinationHex = DestinationForUnit(unitComp, "default");

                        UnitComponent enemyComp = enemy.GetComponent("UnitComponent") as UnitComponent;

                        if (State.firstTimeEnemyTurn)
                        {
                            pathToDestination = PathToHex(boardComponent, unitComp.GetHex(), destinationHex);
                            UpdateDebugList(pathToDestination);
                            enemyHex = enemyComp.GetHex();
                        }

                        enemiesMoving = (State.enemyMoveIndex < boardComponent.nonAlliedUnitList.Count());

                        if (enemiesMoving)
                        {
                            timePerMove = 360;

                            State.elapsedTimeForMove += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                            //animated movement

                            UnitSpriteComponent sprite = enemy.GetDrawable("UnitSpriteComponent") as UnitSpriteComponent;
                            //UnitSpriteComponent originalHexSprite = enemyHex.GetUnit()._parent.GetDrawable("UnitSpriteComponent") as UnitSpriteComponent;
                            SpriteComponent finalHexSprite = pathToDestination[0]._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                            Vector2 originalPosition = enemyHex.GetCenterPosition();
                            Vector2 finalPosition = finalHexSprite.GetCenterPosition();

                            float percentTraveled = State.elapsedTimeForMove / timePerMove;
                            sprite.SetPosition(originalPosition + (finalPosition - originalPosition) * percentTraveled);

                            if (State.elapsedTimeForMove > timePerMove)
                            {
                                State.elapsedTimeForMove = State.elapsedTimeForMove - timePerMove;

                                //pathToDestination[0].SetInQueue(false);
                                MoveUnit(enemyHex, pathToDestination[0]);
                                enemyHex = pathToDestination[0];
                                debugString = enemyHex.GetCoordPosition().ToString();
                                pathToDestination.RemoveAt(0);
                                UpdateDebugList(pathToDestination);
                            }
                            if (pathToDestination.Count() == 0)
                            {
                                State.elapsedTimeForMove = 0;
                                State.enemyMoveIndex++;
                                State.firstTimeEnemyTurn = true;
                                //enemiesMoving = false;
                            }
                        }
                        //else
                        //{
                        //    State.enemyMoveIndex = 0;
                        //    ResetTurnsLeft();
                        //    UpdateTurnState();
                        //    AvailableToMoveCheck();
                        //}

                        State.firstTimeEnemyTurn = false;
                    }
                    #endregion

                    if (qClick.Evaluate())
                    {
                        ChatboxManager.SetEventName("Testing");
                        ChatboxManager.SetNewInfo(ProcessHexMapDialogue(worldMapComponent.GetCurrentNodeID(), ChatboxManager.GetEvent()));
                        State.screenState = State.ScreenState.DIALOGUE;
                    }

                    break;
                #endregion

                #region Skirmish_Resolution

                case State.ScreenState.SKIRMISH_RESOLUTION:

                    if (singleLeftClick.Evaluate())
                    {
                        SavePartyMembersBin();
                        SaveWorldMapBin();
                        ProcessWorldMapBin();

                        State.screenState = State.ScreenState.WORLD_MAP;
                    }

                    break;

                #endregion

                #region Battle

                //As of now, we will not be using the states below
                case State.ScreenState.BATTLE_FORECAST:
                    break;
                case State.ScreenState.BATTLING:

                    // todo: make some sort of data structure for a menu with nested options
                    //  the first layer is exactly the same as a normal menu, but the second layers
                    //  are actually menus associated with strings/index numbers for other options. in other
                    //  words it's like menus in menus.

                    UnitData attacker = State.currentAttacker.GetUnitData();
                    UnitData defender = State.currentDefender.GetUnitData();

                    //  switch the attacker and defender if a counterattack is being launched
                    if (State.battleState == State.BattleState.CounterAttack)
                    {
                        UnitData tmp = attacker;
                        attacker = defender;
                        defender = tmp;
                    }

                    //  player battle turn
                    if (attacker.GetAlignment() == Alignment.Player)
                    {

                        if (singleWClick.Evaluate())
                        {
                            battleMenu.ScrollUp();
                        }
                        else if (singleSClick.Evaluate())
                        {
                            battleMenu.ScrollDown();
                        }
                        else if (enterClick.Evaluate())
                        {
                            if (battleMenu.Layer() == 0)
                            {
                                battleMenu.Enter();
                            }
                            else
                            {
                                if (battleMenu.RegisteredOption() == "Attack")
                                {
                                    if (battleMenu.CurrentOption() == "Slash")
                                    {
                                        // Todo: physical strike
                                        defender.RemoveHealth(attacker.PhysicalDamageAgainst(defender));
                                        State.battleMessage = "Attacked with a slash!";
                                        EndBattleTurn();
                                    }
                                }
                                else if (battleMenu.RegisteredOption() == "Use Item")
                                {
                                }
                                else if (battleMenu.RegisteredOption() == "Cast")
                                {
                                    if (battleMenu.CurrentOption() == "Elfire")
                                    {
                                        defender.RemoveHealth(attacker.MagicalDamageAgainst(defender));
                                        State.battleMessage = "Cast Elfire!";
                                        EndBattleTurn();
                                    }
                                }
                                else if (battleMenu.RegisteredOption() == "Guard")
                                {
                                    //  todo: each unit has a damage resistance variable
                                    //  similar to "armor" that's 100% by default. guarding
                                    //  changes that variable to 80% for one turn.
                                }
                            }
                        }
                        else if (singleAClick.Evaluate())
                        {
                            battleMenu.Back();
                        }
                    }
                    else
                    {
                        //  todo: enemy battle turn..deals 10 damage

                        //defender.RemoveHealth(attacker.PhysicalDamageAgainst(defender));
                        defender.RemoveHealth(1);
                        EndBattleTurn();

                    }
                    break;
                case State.ScreenState.BATTLE_RESOLUTION:
                    break;
                #endregion

                #region Default
                default:
                    //This should never happen
                    break;
                #endregion
            }
            base.Update(gameTime);
        }

        public void UpdateDebugList(List<HexComponent> list)
        {
            debugString1 = "";
            foreach (HexComponent hex in list)
            {
                debugString1 += hex.GetCoordPosition().ToString();
            }

        }

        public void UpdateEnemiesSeen()
        {
            foreach (Entity enemy in boardComponent.nonAlliedUnitList)
            {
                UnitComponent unitComp = enemy.GetComponent("UnitComponent") as UnitComponent;
                UnitSpriteComponent unitSprite = enemy.GetDrawable("UnitSpriteComponent") as UnitSpriteComponent;
                if (unitComp.GetHex().GetVisibility() != Visibility.Visible)
                {
                    unitSprite._visible = false;
                }
                else
                {
                    unitSprite._visible = true;
                }
            }
        }

        public void AvailableToMoveCheck() //if unit has no moves left he/she is turned gray, otherwise he/she is white
        {
            foreach (Entity ally in boardComponent.alliedUnitList)
            {
                UnitComponent unitComp = ally.GetComponent("UnitComponent") as UnitComponent;
                UnitSpriteComponent unitSprite = ally.GetDrawable("UnitSpriteComponent") as UnitSpriteComponent;
                if (unitComp.GetMovesLeft() <= 0)
                {
                    unitSprite.SetColor(Color.SlateGray);
                    unitComp.SetAvailableToMove(false);
                }
                else
                {
                    unitSprite.SetColor(Color.White);
                    unitComp.SetAvailableToMove(true);
                }
            }
        }

        //  heuristic function that acts as a "rule of thumb" for
        //  estimating path distance between two points.
        public float HeuristicDistance(HexComponent start, HexComponent end)
        {
            float dist;
            dist = (start.GetCoordPosition() - end.GetCoordPosition()).Length();
            return dist;
        }

        //  given a board, start, and destination, return a list
        //  of hexes that form an optimal path.
        //  source: http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
        public List<HexComponent> PathToHex(BoardComponent board, HexComponent start, HexComponent destination)
        {
            // for documentation, check page 178 on http://www.itu.dk/research/c5/latest/ITU-TR-2006-76.pdf

            IPriorityQueue<Prio<HexComponent>> open = new IntervalHeap<Prio<HexComponent>>();
            Dictionary<HexComponent, IPriorityQueueHandle<Prio<HexComponent>>> handles = new Dictionary<HexComponent, IPriorityQueueHandle<Prio<HexComponent>>>();

            List<HexComponent> closed = new List<HexComponent>();

            //  g function in A*. it is the actual path distance from A to B (B is the key). contrast
            //  this with the heuristic path distance.
            Dictionary<HexComponent, float> g = new Dictionary<HexComponent, float>();

            Dictionary<HexComponent, HexComponent> pathParent = new Dictionary<HexComponent, HexComponent>();

            //open.Add(start, 0);
            IPriorityQueueHandle<Prio<HexComponent>> h = handles[start];
            open.Add(ref h, new Prio<HexComponent>(start, 0));
            //  the number you put next to it is equal to the priority. start has priority 0.

            HexComponent current;
            while (open.FindMin().Data() != destination)
            {
                current = open.DeleteMin().Data() as HexComponent;
                closed.Add(current);

                foreach (HexComponent neighbor in board.GetAdjacentList(current))
                {
                    //  add one because a hex is one step away from its neighbor

                    float gCurrentVal = 0;
                    if (g.ContainsKey(current)) gCurrentVal = g[current];

                    float cost = gCurrentVal + 1;
                    if (neighbor.ContainsImpassable()) cost += float.PositiveInfinity;
                    //  if a hex is impassable (e.g., a tree) then it has infinite movecost

                    float gNeighborVal = 0;
                    if (g.ContainsKey(neighbor)) gNeighborVal = g[neighbor];

                    //  check to see if open contains neighbor
                    //if (open.Any(p => (p.Data() == neighbor)) && cost < gNeighborVal)
                    Prio<HexComponent> tmp;
                    if (open.Find(handles[neighbor], out tmp) && cost < gNeighborVal)
                    {
                        //  remove neighbor from open
                        open.Delete(handles[neighbor]);
                    }
                    if (closed.Contains(neighbor) && cost < gNeighborVal)
                    {
                        closed.Remove(neighbor);
                    }
                    if (!open.Find(handles[neighbor], out tmp) && !closed.Contains(neighbor))
                    {
                        g[neighbor] = cost;

                        h = handles[neighbor];
                        open.Add(ref h, new Prio<HexComponent>(neighbor, g[neighbor] + HeuristicDistance(start, neighbor)));

                        pathParent[neighbor] = current;
                    }
                }
            }

            //  follow parent in a while loop to reconstruct the path

            List<HexComponent> path = new List<HexComponent>();

            HexComponent hex = destination;
            while (pathParent.ContainsKey(hex))
            {
                path.Add(hex);
                hex = pathParent[hex];
            }
            path.Reverse();

            //path.Add(start);
            //path.Add(destination);
            return path;
        }

        //  calculate a destination for an enemy or neutral unit
        //  that is "optimal" in terms of its artificial intelligence.
        //  the destination is guaranteed to be within the unit's moverange.

        //  the AI will have a specified "strategy" that determines how it behaves,
        //  e.g. "berserk" might be overly aggressive, "passive" might never attack anything.
        public HexComponent DestinationForUnit(UnitComponent unit, string strategy)
        {
            float[] c = new float[7]; //  constants that correspond to AI values.

            if (strategy == "default")
            {
                c = new float[] { 10, 20, 5, 10, 5, 3, 10 };
            }

            List<UnitComponent> seenUnits = unit.seenUnitList;

            float maxScore = -1;
            UnitComponent maxUnit = null;

            List<float> seenUnitsScores = new List<float>();
            foreach (UnitComponent u in seenUnits)
            {
                float score = 0;

                // todo: implement pseudocode
                /*score = (c[0] / (currentHealth * (def + res))) + c[1] * (CanThisUnitCanFightBack?) + 
                    (c[2] / DamageThisUnitWouldDealToYou) + c[3]*(RandomNumber) + (c[4] / DistanceThisEnemyIsFromYou)
                    + c[5] / (DoesThisUnitHaveAHealingItem?) + c[6] * (IsThisUnitASupport?)
                */

                if (score > maxScore)
                {
                    maxScore = score;
                    maxUnit = u;
                }

                seenUnitsScores.Add(score);
            }
            if (maxUnit != null)
            {
                return maxUnit.GetHex();
            }
            else
            {
                return boardComponent.GetHex(new Vector2(0, 0)); 
            }
        }

        //  calculate an optimal action for a unit to perform when its done moving.
        //  for example, a unit may choose to attack, heal, or simply "wait".
        public void ActionForUnit(UnitComponent unit, string strategy)
        {
            // todo: get adjacent hexes. calculate best action according to those.
        }

        //  have two units fight and enter a battle menu
        //  TODO: add turn based fighting system. one unit strikes, then the other strikes back.
        //  TODO: in the update code, keep track of who's the active unit (e.g., who's turn it is)
        //  TODO: and when both units have taken their turn, end the battle and return to the skirmish menu.
        public void StartFight(UnitComponent attacker, UnitComponent defender)
        {
            State.screenState = State.ScreenState.BATTLING;

            State.currentAttacker = attacker;
            State.currentDefender = defender;

            List<string> options = new List<string>(new string[] { "Attack", "Cast", "Use Item", "Guard", "Run" });
            battleMenu = new NestedMenu(options, 200, 50,
                                    0, 400,
                                    Color.Gray, dot, font);

            battleMenu.AddNestedOptions("Attack", State.currentAttacker.GetUnitData().Attacks());
            battleMenu.AddNestedOptions("Cast", State.currentAttacker.GetUnitData().Spells());
            battleMenu.AddNestedOptions("Use Item", State.currentAttacker.GetUnitData().Items());

            battleMenu.Show();
        }

        //  exit the fight scene and clean up the changed variables
        public void EndCurrentFight()
        {
            State.screenState = State.ScreenState.SKIRMISH;
            State.battleState = State.BattleState.Attack;

            if (State.currentAttacker.GetUnitData().GetCurrentHealth() == 0)
            {
                State.currentAttacker.GetHex().SetUnit(null);
                EntityManager.RemoveEntity(State.currentAttacker._parent);

                // give exp bounty... to the victor go the spoils.
                int oldLevel = State.currentDefender.GetUnitData().GetCurrentLevel();

                State.currentDefender.GetUnitData().GainExp(State.currentAttacker.GetUnitData().ExpBounty());

                int newLevel = State.currentDefender.GetUnitData().GetCurrentLevel();
                if (newLevel - oldLevel > 0)
                {
                    // TODO: level up message
                }
            }

            if (State.currentDefender.GetUnitData().GetCurrentHealth() == 0)
            {
                State.currentDefender.GetHex().SetUnit(null);
                EntityManager.RemoveEntity(State.currentDefender._parent);

                int oldLevel = State.currentDefender.GetUnitData().GetCurrentLevel();

                State.currentAttacker.GetUnitData().GainExp(State.currentDefender.GetUnitData().ExpBounty());

                int newLevel = State.currentDefender.GetUnitData().GetCurrentLevel();
                if (newLevel - oldLevel > 0)
                {
                    // TODO: level up message
                }

            }

            State.attackerBattleStatus = State.BattleStatus.NoStatus;
            State.defenderBattleStatus = State.BattleStatus.NoStatus;

            State.currentAttacker = null;
            State.currentDefender = null;

            battleMenu.Hide();
        }

        // todo: end the turn for the current user and allow the enemy to attack
        public void EndBattleTurn()
        {
            if (State.battleState == State.BattleState.Attack)
            {
                State.battleState = State.BattleState.CounterAttack;
            }
            else
            {
                EndCurrentFight();
            }

            State.battleMessage = "";
        }

        //  given a position on the map, return all enemies adjacent to it
        public List<UnitComponent> enemiesAdjacentTo(BoardComponent board, Vector2 position)
        {
            List<UnitComponent> units = unitsAdjacentTo(board, position);
            List<UnitComponent> enemies = new List<UnitComponent>();

            foreach (UnitComponent u in units)
            {
                UnitData data = u.GetUnitData();
                if (data.GetAlignment() == Alignment.Enemy)
                {
                    enemies.Add(u);
                }
            }

            return enemies;
        }

        //  returns the allies adjacent to positions
        public List<UnitComponent> alliesAdjacentTo(BoardComponent board, Vector2 position)
        {
            List<UnitComponent> units = unitsAdjacentTo(board, position);
            List<UnitComponent> allies = new List<UnitComponent>();

            foreach (UnitComponent u in units)
            {
                UnitData data = u.GetUnitData();
                if (data.GetAlignment() == Alignment.Player)
                {
                    allies.Add(u);
                }
            }

            return allies;
        }

        //  given a position on the map, return all units adjacent to it
        public List<UnitComponent> unitsAdjacentTo(BoardComponent board, Vector2 position)
        {
            List<HexComponent> hexes = board.GetRing(position, 1);
            List<UnitComponent> units = new List<UnitComponent>();

            foreach (HexComponent h in hexes)
            {
                if (h.GetUnit() != null)
                {
                    units.Add(h.GetUnit());
                }
            }

            return units;
        }

        public void StartNode()
        {
            //Load the state of the party before every level and save the party at end of every
            if (worldMapComponent.GetCurrentNodeID() != null)
            {
                boardEntity = ProcessHexMapBin(worldMapComponent.GetCurrentNodeID());
                boardComponent = boardEntity.GetComponent("BoardComponent") as BoardComponent;

                ProcessHexMapEnemyBin(worldMapComponent.GetCurrentNodeID());
                ProcessPartyMembersBin();

                //TODO: HAVE AN ACTIVE PARTY MEMBERS LIST
                //If there aren't enough spaces for them the highest in your queue will go
                //You should be able to reorder your party


                State.screenState = State.ScreenState.DIALOGUE;
                ChatboxManager.SetEventName("Beginning");
                ChatboxManager.SetNewInfo(ProcessHexMapDialogue(worldMapComponent.GetCurrentNodeID(), ChatboxManager.GetEvent()));
                //Camera.MoveTo(Vector2.Zero);
            }
        }

        void EndLevel()
        {
            //Save party members to bin
        }

        void StartDialogue(String myEventName)
        {
            //Even if there is one already going it starts it over with thos event
            String temporaryEventStandIn = "Beginning";
            List<string> messageLines = ProcessHexMapDialogue(worldMapComponent.GetCurrentNodeID(), temporaryEventStandIn);
            ChatboxManager.SetNewInfo(messageLines);
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

        public bool AreAdjacent(HexComponent one, HexComponent two)
        {
            if (boardComponent.GetAdjacentList(one).Contains(two))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void MoveUnit(HexComponent original, HexComponent final)
        {
            UnitComponent unit = original.GetUnit();
            Entity unitEntity = unit._parent;

            //SpriteComponent unitSprite = unitEntity.GetDrawable("SpriteComponent") as SpriteComponent;
            //SpriteComponent hexSprite = Entity.GetDrawable("SpriteComponent") as SpriteComponent;
            //sprite.setPosition(final.

            ((UnitSpriteComponent)unitEntity.GetDrawable("UnitSpriteComponent")).
                 SetPosition(final._parent.GetDrawable("SpriteComponent").GetPosition());

            unit.SetHex(final);
            final.SetUnit(unit);

            boardComponent.UpdateVisibilityAllies();
            UpdateEnemiesSeen();

            original.RemoveUnit();
        }

        void MoveAnimation(HexComponent original, HexComponent final, GameTime gameTime)
        {
            UnitComponent unit = original.GetUnit();
            UnitSpriteComponent tempSprite = unit._parent.GetDrawable("UnitSpriteComponent") as UnitSpriteComponent;
        }

        public void UpdateTurnState()
        {
            if (State.turnState == State.TurnState.EnemiesTurn)
            {
                foreach (Entity ally in boardComponent.alliedUnitList)
                {
                    UnitComponent unit = ally.GetComponent("UnitComponent") as UnitComponent;
                    if (unit.GetMovesLeft() > 0)
                    {
                        State.turnState = State.TurnState.AlliesTurn;
                        break;
                    }
                }
            }

            else if (State.turnState == State.TurnState.AlliesTurn)
            {
                State.sumOfMoves = 0;
                foreach (Entity ally in boardComponent.alliedUnitList)
                {
                    UnitComponent unit = ally.GetComponent("UnitComponent") as UnitComponent;
                    State.sumOfMoves += unit.GetMovesLeft();
                }
                if (State.sumOfMoves <= 0)
                {
                    State.turnState = State.TurnState.EnemiesTurn;
                }
            }
        }

        public void ResetTurnsLeft()
        {
            foreach (Entity ally in boardComponent.alliedUnitList)
            {
                UnitComponent unitComp = ally.GetComponent("UnitComponent") as UnitComponent;
                unitComp.SetMovesLeft(unitComp.GetUnitData().GetMovement());
            }
        }

        public List<HexComponent> GetVisibleHexes(Entity myEnt)
        {
            List<HexComponent> visibleHexes = new List<HexComponent>();

            UnitComponent unitComp = myEnt.GetComponent("UnitComponent") as UnitComponent;
            UnitData unitData = unitComp.GetUnitData();

            List<HexComponent> obstructionHexList = new List<HexComponent>();

            for (int r = 0; r <= unitData.GetSightRadius(); r++)
            {
                List<HexComponent> currentRing = boardComponent.GetRing(unitComp.GetHex().GetCoordPosition(), r);
                for (int i = 0; i < currentRing.Count; i++) //i is hex index within currentRing
                {
                    HexComponent currentHex = currentRing[i];
                    bool obstructed = false;

                    if (obstructionHexList.Count > 0)
                    {
                        foreach (HexComponent obstruction in obstructionHexList)
                        {
                            if (Math.Abs(boardComponent.GetTargetAngle(unitComp.GetHex(), obstruction, currentHex)) < Math.Abs(boardComponent.GetObstructionAngle(unitComp.GetHex(), obstruction)))
                            {
                                obstructed = true;
                                break;
                            }
                        }
                    }
                    if (obstructed == false)
                    {
                        visibleHexes.Add(currentHex);
                    }

                    if (currentHex.GetLargestTerrainVisibilityBlock() != 0)
                    {
                        obstructionHexList.Add(currentHex);
                    }
                }
            }

            return visibleHexes;
        }

        public void CheckToStopForNewEnemies(UnitComponent unitMoving)
        {
            newEnemiesYouSee = new List<UnitComponent>();
            foreach (Entity enemy in boardComponent.nonAlliedUnitList)
            {
                UnitComponent unitComp = enemy.GetComponent("UnitComponent") as UnitComponent;
                if (unitComp.GetHex().GetVisibility() == Visibility.Visible)
                {
                    newEnemiesYouSee.Add(unitComp);
                }
            }
            bool doStopQueue = false;
            foreach (UnitComponent enemy in newEnemiesYouSee)
            {
                if (oldEnemiesYouSee.Contains(enemy) == false)
                {
                    //a previously unknown enemy is found
                    doStopQueue = true;
                    List<HexComponent> clusterHexList = boardComponent.GetAllRings(enemy.GetHex().GetCoordPosition(), clusterDetectionRadius);
                    foreach (UnitComponent visibleEnemy in newEnemiesYouSee)
                    {
                        if (visibleEnemy != enemy)
                        {
                            if (clusterHexList.Contains(visibleEnemy.GetHex()))
                            {
                                if (oldEnemiesYouSee.Contains(visibleEnemy))
                                {
                                    //an OLD, known enemy is visible in surrounding cluster
                                    doStopQueue = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (doStopQueue)
                {
                    for (int i = pathQueue.Count() - 1; i >= 0; i--)
                    {
                        pathQueue[i].SetInQueue(false);
                        pathQueue.RemoveAt(i);
                        State.originalHexClicked.GetUnit().ChangeMovesLeft(1);
                    }
                    break;
                }
            }
            oldEnemiesYouSee = newEnemiesYouSee;
        }

        public void UpdateAllSeenUnitList() // this function is used exclusively for enemies
        {
            foreach (Entity enemy in boardComponent.nonAlliedUnitList)
            {
                UnitComponent unit = enemy.GetComponent("UnitComponent") as UnitComponent;
                List<HexComponent> visibleHexes = GetVisibleHexes(unit._parent);
                foreach (Entity unitSeen in boardComponent.totalUnitList)
                {
                    UnitComponent unitComp = unitSeen.GetComponent("UnitComponent") as UnitComponent;
                    if (visibleHexes.Contains(unitComp.GetHex()))
                    {
                        unit.AddToSeenUnitList(unitComp);
                    }
                }
            }
        }

        public void UpdateAllKnownUnitList() // takes all allies that other enemies current enemy sees and puts in list
        {
            foreach (Entity enemy in boardComponent.nonAlliedUnitList)
            {
                UnitComponent unit = enemy.GetComponent("UnitComponent") as UnitComponent;
                foreach (UnitComponent unitSeen in unit.seenUnitList)
                {
                    if (boardComponent.nonAlliedUnitList.Contains(unitSeen._parent)) // if it's an enemy
                    {
                        if (unitSeen != unit) // if it's not itself
                        {
                            List<UnitComponent> alliesEnemySees = unitSeen.GetSeenUnitList();
                            foreach (UnitComponent enemySees in alliesEnemySees)
                            {
                                if (boardComponent.alliedUnitList.Contains(enemySees._parent))
                                {
                                    unit.AddToKnownUnitList(enemySees);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            EntityManager.Draw(spriteBatch, graphics);

            spriteBatch.Begin();



            ChatboxManager.Draw(spriteBatch);

            //spriteBatch.DrawString(font, InputState.GetMouseIngamePosition().ToString(), new Vector2(0, font.LineSpacing), Color.White);
            //spriteBatch.DrawString(font, InputState.GetMouseScreenPosition().ToString(), new Vector2(0, 2 * font.LineSpacing), Color.White);

            if (boardComponent != null)
            {
                spriteBatch.DrawString(font, "Current Hex Coord " + boardComponent.GetMouseHex().GetCoordPosition().ToString(), new Vector2(0, font.LineSpacing), Color.White);
                //    double a = Vector2.Distance(boardComponent.GetHexPosition(boardComponent.GetHex(5, 5)), boardComponent.GetHexPosition(boardComponent.GetHex(6, 5)));
                //    double b = Vector2.Distance(boardComponent.GetHexPosition(boardComponent.GetHex(5, 5)), boardComponent.GetHexPosition(boardComponent.GetHex(8, 5)));
                //    double c = Vector2.Distance(boardComponent.GetHexPosition(boardComponent.GetHex(6, 5)), boardComponent.GetHexPosition(boardComponent.GetHex(8, 5)));
                //    double input = Math.Round((-Math.Pow(c, 2) + Math.Pow(a, 2) + Math.Pow(b, 2)) / (2 * a * b), 5);

                //    spriteBatch.DrawString(font, boardComponent.GetMouseHex().getCoordPosition().ToString(), new Vector2(0, 3 * font.LineSpacing), Color.White);
                //    spriteBatch.DrawString(font, boardComponent.GetTargetAngle(boardComponent.GetHex(5, 5), boardComponent.GetHex(6, 5), boardComponent.GetHex(9, 5)).ToString(), new Vector2(0, 4 * font.LineSpacing), Color.White);
                //    spriteBatch.DrawString(font, boardComponent.GetObstructionAngle(boardComponent.GetHex(5, 5), boardComponent.GetHex(6, 5)).ToString(), new Vector2(0, 5 * font.LineSpacing), Color.White);
                //    spriteBatch.DrawString(font, Vector2.Distance(boardComponent.GetHexPosition(boardComponent.GetHex(5, 5)), boardComponent.GetHexPosition(boardComponent.GetHex(6, 5))).ToString(), new Vector2(0, 6 * font.LineSpacing), Color.White);
                //    spriteBatch.DrawString(font, Vector2.Distance(boardComponent.GetHexPosition(boardComponent.GetHex(5, 5)), boardComponent.GetHexPosition(boardComponent.GetHex(8, 5))).ToString(), new Vector2(0, 7 * font.LineSpacing), Color.White);
                //    spriteBatch.DrawString(font, Vector2.Distance(boardComponent.GetHexPosition(boardComponent.GetHex(6, 5)), boardComponent.GetHexPosition(boardComponent.GetHex(8, 5))).ToString(), new Vector2(0, 8 * font.LineSpacing), Color.White);
                //    spriteBatch.DrawString(font, Vector2.Distance(boardComponent.GetHexPosition(boardComponent.GetHex(6, 5)), boardComponent.GetHexPosition(boardComponent.GetHex(8, 5))).ToString(), new Vector2(0, 9 * font.LineSpacing), Color.White);
                //    spriteBatch.DrawString(font, input.ToString(), new Vector2(0, 10 * font.LineSpacing), Color.White);
                if (oldEnemiesYouSee.Count() != 0)
                {
                    spriteBatch.DrawString(font, "old " + oldEnemiesYouSee.Count().ToString(), new Vector2(0, 5 * font.LineSpacing), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(font, "old " + 0.ToString(), new Vector2(0, 5 * font.LineSpacing), Color.White);
                }
                if (newEnemiesYouSee.Count() != 0)
                {
                    spriteBatch.DrawString(font, "new " + newEnemiesYouSee.Count().ToString(), new Vector2(0, 6 * font.LineSpacing), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(font, "new " + 0.ToString(), new Vector2(0, 6 * font.LineSpacing), Color.White);
                }
                if (State.originalHexClicked != null)
                {
                    int movesLeft = State.originalHexClicked.GetUnit().GetMovesLeft();
                    //  spriteBatch.DrawString(font, movesLeft.ToString(), new Vector2(0, 3 * font.LineSpacing), Color.White);
                }
                spriteBatch.DrawString(font, State.elapsedTimeForMove.ToString(), new Vector2(0, 7 * font.LineSpacing), Color.White);
                spriteBatch.DrawString(font, State.turnState.ToString(), new Vector2(0, 8 * font.LineSpacing), Color.White);
                if (debugString != null)
                {
                    spriteBatch.DrawString(font, debugString, new Vector2(0, 9 * font.LineSpacing), Color.White);

                }
                if (debugString1 != null)
                {
                    spriteBatch.DrawString(font, debugString1, new Vector2(0, 10 * font.LineSpacing), Color.White);
                }
                //  spriteBatch.DrawString(font, State.sumOfMoves.ToString(), new Vector2(0, 4 * font.LineSpacing), Color.White);

            }

            numberOfFrames++;
            string fps = string.Format("fps: {0}", framesPerSecond);
            spriteBatch.DrawString(font, fps, Vector2.Zero, Color.White);

            menu.Draw(spriteBatch);

            switch (State.screenState)
            {
                case State.ScreenState.WORLD_MAP:

                    if (worldMapComponent.GetCurrentNodeID() != null)
                    {
                        spriteBatch.DrawString(font, worldMapComponent.GetCurrentNodeID(), new Vector2(0, 300), Color.White);
                    }
                    break;

                case (State.ScreenState.BATTLING):
                    //  draw battle scene

                    (State.currentAttacker._parent.GetDrawable("UnitSpriteComponent") as AnimatedSpriteComponent).Draw(spriteBatch, new Vector2(400, 200));
                    (State.currentDefender._parent.GetDrawable("UnitSpriteComponent") as AnimatedSpriteComponent).Draw(spriteBatch, new Vector2(800, 200));

                    battleMenu.Draw(spriteBatch);

                    //  debug code
                    spriteBatch.DrawString(font, "current option selected: " + battleMenu.CurrentOption(), new Vector2(0, 4 * font.LineSpacing), Color.White);
                    spriteBatch.DrawString(font, "Attacker HP: " + State.currentAttacker.GetUnitData().GetCurrentHealth(), new Vector2(0, 5 * font.LineSpacing), Color.White);
                    spriteBatch.DrawString(font, "Defender HP: " + State.currentDefender.GetUnitData().GetCurrentHealth(), new Vector2(0, 6 * font.LineSpacing), Color.White);

                    break;
                case (State.ScreenState.SKIRMISH):

                    break;
            }
            spriteBatch.DrawString(font, State.screenState.ToString(), new Vector2(0, 550), Color.White);
            spriteBatch.DrawString(font, "Ingame: " + InputState.GetMouseIngamePosition().ToString(), new Vector2(0, 570), Color.White);
            spriteBatch.DrawString(font, "Camera: " + Camera.Pos.ToString(), new Vector2(0, 590), Color.White);
            spriteBatch.DrawString(font, "Screen: " + InputState.GetMouseScreenPosition().ToString(), new Vector2(0, 610), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
