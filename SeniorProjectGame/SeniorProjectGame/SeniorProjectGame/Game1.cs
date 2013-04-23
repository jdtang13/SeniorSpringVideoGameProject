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

namespace SeniorProjectGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables

        Random rand = new Random();

        //Graphics and Content Vars
        GraphicsDeviceManager graphics;
        int screenWidth = 1280;
        int screenHeight = 680;
        SpriteBatch spriteBatch;

        Texture2D worldMapTexture, nodeTexture, pointerTexture;
        Texture2D hexBaseTexture, dirtTexture, grassTexture, gravelTexture, sandTexture, woodTexture,
            waterTexture, stoneTexture;
        Texture2D treeTexture, wallTexture, bushTexture, tableTexture, carpetTexture, throneTexture, tentTexture;
        Texture2D markerTexture, questionTexture;

        Dictionary<string, Texture2DFramed> unitTextureDictionary = new Dictionary<string, Texture2DFramed>();

        Texture2DFramed unitFramedTexture, axemanFramedTexture, battlemageFramedTexture, bowmanFramedTexture, crossbowmanFramedTexture,
            flailmanFramedTexture, halberdierFramedTexture, knightFramedTexture, mageAssassinFramedTexture, manAtArmsFramedTexture,
            pikemanFramedTexture, riflemanFramedTexture, spearmanFramedTexture, swordsmanFramedTexture, wizardFramedTexture, slimeFramedTexture;

        SoundEffect selectSound;

        SpriteFont font;

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
                return new TerrainPackage(questionTexture, false);
            }
        }
        HexComponent ghostHex = null;
        List<HexComponent> pathQueue = new List<HexComponent>();

        bool moving = false;
        bool yourTurn = true;

        //Player Vars
        Dictionary<String, Role> classes = new Dictionary<String, Role>();
        List<UnitDataComponent> partyUnitData = new List<UnitDataComponent>();


        //Input Vars
        InputAction singleLeftClick, singleRightClick, singleMiddleClick;
        InputAction leftHold;

        InputAction wClick, aClick, sClick, dClick, enterClick, escapeClick;

        //Menu Vars
        List<Entity> orderButtonEntityList = new List<Entity>();
        Entity attackOrderEntity, moveOrderEntity, noOrderEntity, spellOrderEntity;
        Texture2D attackOrderTexture, moveOrderTexture, noOrderTexture, spellOrderTexture;

        //FPS Vars
        float framesPerSecond = 60;
        float numberOfFrames;
        TimeSpan elapsedTimeForFps;
        TimeSpan elapsedTimeForMove;

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
            PopulateUnitTextureDictionary();

            ProcessWorldMapBin();
            ProcessPlayerRolesBin();

            ProcessEnemyBestiaryBin();

            InitializeInput();

            State.Initialize();

            base.Initialize();
        }

        void InitializeInput()
        {
            escapeClick = new InputAction(new Keys[] { Keys.Escape }, true);
            enterClick = new InputAction(new Keys[] { Keys.Enter }, true);

            wClick = new InputAction(new Keys[] { Keys.W, Keys.Up }, false);
            dClick = new InputAction(new Keys[] { Keys.D, Keys.Right }, false);
            sClick = new InputAction(new Keys[] { Keys.S, Keys.Down }, false);
            aClick = new InputAction(new Keys[] { Keys.A, Keys.Left }, false);

            singleLeftClick = new InputAction(MouseButton.left, true);

            leftHold = new InputAction(MouseButton.left, false);

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

            //Only run the conversions for developement purposes
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Enemies.txt");
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Player_Roles.txt");

            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Party_Members.txt");

            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\WorldMap.txt");
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Tutorial_Level.txt");
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Tutorial_Level_Enemies.txt");

            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Lab_Yard.txt");
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Lab_Yard_Enemies.txt");

            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Alchemist's_Laboratory.txt");
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Ambushed.txt");
            
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Pavilion.txt");
            ConvertTxtToBin("C:\\Users\\Oliver\\Desktop\\Throne_Room.txt");

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

            treeTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Decorations\\tree");
            bushTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Decorations\\bush");
            wallTexture = Content.Load<Texture2D>("Graphics\\TileTextures\\Decorations\\wooden Walls");
            questionTexture = Content.Load<Texture2D>("Graphics\\Other\\questionTexture");

            PopulateTerrainDictionary();

            selectSound = Content.Load<SoundEffect>("Audio\\Sounds\\Powerup27");
        }

        void PopulateTerrainDictionary()
        {
            terrainDictionary["G"] = new TerrainPackage(grassTexture, false);//Grass
            terrainDictionary["D"] = new TerrainPackage(dirtTexture, false);//Dirt
            terrainDictionary["L"] = new TerrainPackage(waterTexture, true);//Water
            terrainDictionary["W"] = new TerrainPackage(woodTexture, false);//Wood
            terrainDictionary["S"] = new TerrainPackage(stoneTexture, false);//Stone
            terrainDictionary["A"] = new TerrainPackage(sandTexture, false);//Sand
            terrainDictionary["g"] = new TerrainPackage(gravelTexture, false);//Gravel

            terrainDictionary["T"] = new TerrainPackage(treeTexture, true);//Tree
            terrainDictionary["B"] = new TerrainPackage(bushTexture, false);//Bush
            terrainDictionary["C"] = new TerrainPackage(carpetTexture, false);//Carpet
            //terrainDictionary["t"] = new TerrainPackage(treeTexture, true);//Table
            //terrainDictionary["h"] = new TerrainPackage(treeTexture, true);//Throne
            //terrainDictionary["n"] = new TerrainPackage(treeTexture, true);//Tent

            terrainDictionary["X"] = new TerrainPackage(wallTexture, true);
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

                    Entity partyMemberEntity = new Entity(EntityManager.GetHighestLayer()+1, State.ScreenState.SKIRMISH);

                    UnitDataComponent unitDataComp = new UnitDataComponent(
                                        name, role, Alignment.PLAYER, level,
                                        str, mag, dex, agi, def, res, spd,
                                        strGrowth, magGrowth, dexGrowth, agiGrowth, defGrowth, resGrowth, spdGrowth,
                                        strCap, magCap, dexCap, agiCap, defCap, resCap, spdCap,
                                        movement, sightRange, attackRange);
                    partyMemberEntity.AddComponent(unitDataComp);

                    Vector2 coordinate = boardComponent.GetOneAlliedSpawnPoint(rand);
                    SpriteComponent hexSprite = boardComponent.GetHex(coordinate)._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                    partyMemberEntity.AddComponent(new AnimatedSpriteComponent(true, hexSprite.getCenterPosition(), unitTextureDictionary[graphicName]));

                    partyMemberEntity.AddComponent(new UnitComponent(boardComponent.GetHex(coordinate), true));

                    EntityManager.AddEntity(partyMemberEntity);
                    boardComponent.GetHex(coordinate).SetUnit(partyMemberEntity.GetComponent("UnitComponent") as UnitComponent);
                    boardComponent.alliedUnitList.Add(partyMemberEntity);

                }
            }
            boardComponent.UpdateVisibilityAllies();
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
                if (binLines[line] != "")
                    hexMapLines.Add(binLines[line]);
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
                        if (line[x] == "O")
                        {
                            //Add allied spawn
                            tempBoardComponent.AddAlliedSpawnPoint(ConvertToHexCoordinate(new Vector2(x, y - ((int)dimensions.Y * layers))));
                        }

                        else if (line[x] == "V")
                        {
                            //Victory condition
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
                            tempBoardComponent.AddEnemySpawnPoint(Convert.ToInt32(line[x]), ConvertToHexCoordinate(new Vector2(x, y - ((int)dimensions.Y * layers))));
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
                    int level = Convert.ToInt32(nameLine[4]);
                    string graphicName = nameLine[5];

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

                    Vector2 hexLocation = boardComponent.GetEnemySpawnPointForType(unitSpawn, rand);
                    HexComponent hex = boardComponent.GetHex(hexLocation);
                    SpriteComponent hexSprite = hex._parent.GetDrawable("SpriteComponent") as SpriteComponent;

                    Entity blob = new Entity(EntityManager.GetHighestLayer() + 1, State.ScreenState.SKIRMISH);
                    blob.AddComponent(new AnimatedSpriteComponent(true, hexSprite.GetPosition(), unitTextureDictionary[graphicName]));
                    blob.AddComponent(new UnitComponent(hex, false));

                    hex.SetUnit(blob.GetComponent("UnitComponent") as UnitComponent);

                    blob.AddComponent(new UnitDataComponent(
                                        name, role, Alignment.ENEMY, level,
                                        str, mag, dex, agi, def, res, spd,
                                        strGrowth, magGrowth, dexGrowth, agiGrowth, defGrowth, resGrowth, spdGrowth,
                                        strCap, magCap, dexCap, agiCap, defCap, resCap, spdCap,
                                        movement, sightRange, attackRange));
                    EntityManager.AddEntity(blob);
                    boardComponent.nonAlliedUnitList.Add(blob);
                }
            }
        }


        #endregion

        #region Saving_Content_and_File_Processing

        void SavePartyMembersBin()
        {

        }

        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            elapsedTimeForFps += gameTime.ElapsedGameTime;
            if (elapsedTimeForFps > TimeSpan.FromSeconds(1))
            {
                elapsedTimeForFps -= TimeSpan.FromSeconds(1);
                framesPerSecond = numberOfFrames;
                numberOfFrames = 0;
            }

            if (escapeClick.Evaluate())
            {
                this.Exit();
            }
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
                    if (!moving)
                    {
                        if (leftHold.Evaluate())
                        {
                            HexComponent hexComp = boardComponent.GetMouseHex();

                            Entity hexEntity = hexComp._parent;

                            if (hexComp.HasUnit() && hexComp.GetUnit().GetSelectable() && State.selectionState == State.SelectionState.NoSelection )
                            {
                                UnitComponent unit = hexComp.GetUnit();
                                unit.SetSelected(true); //todo set false
                                State.selectionState = State.SelectionState.SelectingUnit;

                                State.originalHexClicked = hexComp;
                                ghostHex = hexComp;
                            }
                            if (hexComp == State.originalHexClicked)
                            {
                                if (pathQueue.Count > 0)
                                {
                                    for (int i = 0; i < pathQueue.Count; i++)
                                    {
                                        pathQueue[i].SetInQueue(false);
                                    }
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
                                if (!hexComp.ContainsImpassable())
                                {
                                    if (!pathQueue.Contains(hexComp) && AreAdjacent(hexComp, ghostHex))
                                    {
                                        pathQueue.Add(hexComp);
                                        hexComp.SetInQueue(true);
                                        ghostHex = hexComp;
                                    }
                                    else if (pathQueue.Contains(hexComp))
                                    {
                                        int originalpathQueueCount = pathQueue.Count;
                                        while (true)
                                        {
                                            if (pathQueue[pathQueue.Count - 1] != hexComp)
                                            {
                                                pathQueue[pathQueue.Count - 1].SetInQueue(false);
                                                pathQueue.Remove(pathQueue[pathQueue.Count - 1]);
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
                    if (moving)
                    {
                        elapsedTimeForMove += gameTime.ElapsedGameTime;

                        if (elapsedTimeForMove > TimeSpan.FromMilliseconds(100))
                        {
                            elapsedTimeForMove = TimeSpan.FromMilliseconds(0);

                            MoveUnit(State.originalHexClicked, pathQueue[0]);
                            State.originalHexClicked = pathQueue[0];
                            pathQueue[0].SetInQueue(false);

                            pathQueue.Remove(pathQueue[0]);
                            if (pathQueue.Count == 0)
                            {
                                State.selectionState = State.SelectionState.NoSelection;
                                State.originalHexClicked.GetUnit().SetSelected(false);
                                State.originalHexClicked = null;
                                moving = false;
                            }
                        }
                    }
                    if (enterClick.Evaluate())
                    {
                        if (pathQueue.Count != 0)
                        {
                            moving = true;
                        }
                        else
                        {
                            State.selectionState = State.SelectionState.NoSelection;

                            if (State.originalHexClicked != null)
                            {
                                State.originalHexClicked.GetUnit().SetSelected(false);
                                State.originalHexClicked = null;
                            }
                        }

                    }

                    else if (singleRightClick.Evaluate())
                    {
                        boardComponent.ToggleFogofWar(false);
                    }
                    if (singleMiddleClick.Evaluate())
                    {
                        boardComponent.UpdateVisibilityAllies();
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

        public void StartNode()
        {
            //Load the state of the party before every level and save the party at end of every


            boardEntity = ProcessHexMapBin(worldMapComponent.GetCurrentNodeID());
            boardComponent = boardEntity.GetComponent("BoardComponent") as BoardComponent;

            ProcessHexMapEnemyBin(worldMapComponent.GetCurrentNodeID());
            ProcessPartyMembersBin();

            //TODO: HAVE AN ACTIVE PARTY MEMBERS LIST
            //TODO: PULL YOUR ACTIVE PARTY MEMBERS instead of creating this nondescript
            //If there aren't enough spaces for them the highest in your queue will go
            //You should be able to reorder your party

            State.screenState = State.ScreenState.SKIRMISH;
        }

        void EndLevel()
        {
            //Save party members to bin
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

            original.RemoveUnit();
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

        #endregion

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            EntityManager.Draw(spriteBatch, graphics);

            spriteBatch.Begin();

            spriteBatch.DrawString(font, InputState.GetMouseIngamePosition().ToString(), new Vector2(0, font.LineSpacing), Color.White);

            numberOfFrames++;
            string fps = string.Format("fps: {0}", framesPerSecond);
            spriteBatch.DrawString(font, fps, Vector2.Zero, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
