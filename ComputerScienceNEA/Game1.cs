using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerScienceNEA
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch; 

        int row = 1;
        int position = 1;
        int TurnNum;
        Camera Cam;
        Tile[] Tiles;
        Base[] Bases;
        bool NextTurn = false;
        Unit[] AIUnits;
        Random random = new Random();
        Rectangle Cursor;
        bool unclick = true;
        Unit[] PlayerUnits;
        bool BaseMenu = false;
        bool UnitFinished = true;
        bool TileReset = true;

        int LeftStoneNum = 0;
        int LeftWheatNum = 0;
        int LeftForestNum = 0;
        int LeftHillNum = 0;
        int LeftIteration = 0;

        int RightStoneNum = 0;
        int RightWheatNum = 0;
        int RightForestNum = 0;
        int RightHillNum = 0;
        int RightIteration = 0;

        int StoneRandNum;
        int HillRandNum;
        int WheatRandNum;
        int ForrestRandNum;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            graphics.ApplyChanges();
            base.Initialize();

        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Cam = new Camera(graphics.GraphicsDevice.Viewport);
            Tiles = new Tile[252];
            AIUnits = new Unit[0];

            for (int i = 0; i<252; i++)
            {
                Tiles[i] = new Tile(row, position, 1, new Vector2(position*200,row*200), Content.Load<Texture2D>("Blank Tile"), "", i);
                position++;
                if (position > 28)
                {
                    position = 1;
                    row++;
                }
            }

            foreach (Tile Space in Tiles)
            {
                if (Space.Position <= 14 && Space.GetListNum() != 115)
                {
                    string NewFeature;
                    LeftIteration++;

                    ForrestRandNum = random.Next(0, 250);
                    StoneRandNum = random.Next(0, 250);
                    WheatRandNum = random.Next(0, 250);
                    HillRandNum = random.Next(0, 250);

                    NewFeature = Space.FeatureGenerate(LeftWheatNum, LeftStoneNum, LeftForestNum, LeftHillNum, LeftIteration, ForrestRandNum, HillRandNum, StoneRandNum, WheatRandNum);

                    if (NewFeature == "Wheat")
                    {
                        LeftWheatNum++;
                        Space.SetTexture(Content.Load<Texture2D>("Wheat Tile"));
                    }
                    if (NewFeature == "Stone")
                    {
                        LeftStoneNum++;
                        Space.SetTexture(Content.Load<Texture2D>("Stone Tile"));
                    }
                    if (NewFeature == "Hill")
                    {
                        LeftHillNum++;
                        Space.SetTexture(Content.Load<Texture2D>("Hill Tile"));
                    }
                    if (NewFeature == "Forest")
                    {
                        LeftForestNum++;
                        Space.SetTexture(Content.Load<Texture2D>("Forest Tile"));
                    }
                }
                else if(Space.GetListNum() != 136)
                {
                    string NewFeature;
                    RightIteration++;

                    ForrestRandNum = random.Next(0, 250);
                    StoneRandNum = random.Next(0, 250);
                    WheatRandNum = random.Next(0, 250);
                    HillRandNum = random.Next(0, 250);

                    NewFeature = Space.FeatureGenerate(RightWheatNum, RightStoneNum, RightForestNum, RightHillNum, RightIteration, ForrestRandNum, HillRandNum, StoneRandNum, WheatRandNum);

                    if (NewFeature == "Wheat")
                    {
                        RightWheatNum++;
                        Space.SetTexture(Content.Load<Texture2D>("Wheat Tile"));
                    }
                    if (NewFeature == "Stone")
                    {
                        RightStoneNum++;
                        Space.SetTexture(Content.Load<Texture2D>("Stone Tile"));
                    }
                    if (NewFeature == "Hill")
                    {
                        RightHillNum++;
                        Space.SetTexture(Content.Load<Texture2D>("Hill Tile"));
                    }
                    if (NewFeature == "Forest")
                    {
                        RightForestNum++;
                        Space.SetTexture(Content.Load<Texture2D>("Forest Tile"));
                    }
                }
            }

            Bases = new Base[2];
            Bases[0] = new Base(1, 115, Content.Load<Texture2D>("Base Placeholder"));
            Bases[1] = new Base(2, 136, Content.Load<Texture2D>("Base Placeholder"));
            PlayerUnits = new Unit[0];  //As more units are made/destroyed use Array.Resize(ref PlayerUnits, NewSize) to increase/decrease the size of this array 

            Array.Resize(ref PlayerUnits, PlayerUnits.Length + 1);
            PlayerUnits[PlayerUnits.Length - 1] = new Spear(true, 100, 10, 1, 117, Content.Load<Texture2D>("Spear"), 3);

            IsMouseVisible = true;
        }
        
        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Released && Keyboard.GetState().IsKeyUp(Keys.Space) && Keyboard.GetState().IsKeyUp(Keys.Z) && !unclick)
                unclick = true;

            if ((Mouse.GetState().LeftButton == ButtonState.Pressed))
            {
                TileReset = false;
                Cursor = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 5, 5);
                
                foreach(Tile tile in Tiles)
                {
                    if (tile.ClickBox.Intersects(Cursor))
                    {
                        tile.SetTexture(Content.Load<Texture2D>("Click Tile"));

                        if (tile.GetListNum() == Bases[0].GetListNum() && unclick && BaseMenu)
                        {
                            BaseMenu = false;
                            unclick = false;
                        }

                        else if (tile.GetListNum() == Bases[0].GetListNum() && unclick)
                        {
                            BaseMenu = true;
                            unclick = false;
                        }

                        else
                        {
                            foreach (Unit Player in PlayerUnits)
                            {
                                if (tile.GetListNum() == Player.TileNum && !Player.ChoosingMove && unclick && Player.MP != 0)
                                {
                                    Player.ChoosingMove = true;
                                    Player.SetTexture(Content.Load<Texture2D>("Selected"));
                                    unclick = false;
                                }

                                if (tile.GetListNum() == Player.TileNum && Player.ChoosingMove && unclick && Player.MP != 0)
                                {
                                    Player.ChoosingMove = false;
                                    if(Player.GetTemp() == "Ranged")
                                        Player.SetTexture(Content.Load<Texture2D>("Ranged"));
                                    else if (Player.GetTemp() == "Cavalry")
                                        Player.SetTexture(Content.Load<Texture2D>("Cavalry"));
                                    else if (Player.GetTemp() == "Warrior")
                                        Player.SetTexture(Content.Load<Texture2D>("Warrior"));
                                    else if (Player.GetTemp() == "Spear")
                                        Player.SetTexture(Content.Load<Texture2D>("Spear"));
                                    unclick = false;
                                }

                                if (tile.GetListNum() != Player.TileNum && Player.ChoosingMove && unclick && Player.MP != 0)
                                {
                                    int[] Path = Player.Path(Player.TileNum, tile.GetListNum(), Tiles);
                                    int NextMoveCost = 0;
                                    foreach (int Move in Path)
                                    {
                                        if (Move >= 0)
                                            NextMoveCost = NextMoveCost + Tiles[Move].GetMoveCost();
                                    }
                                    NextMoveCost = NextMoveCost - Tiles[Player.TileNum].GetMoveCost();
                                    if (NextMoveCost <= Player.MP && Player.MP != 0)
                                    {
                                        Player.TileNum = tile.GetListNum();
                                        Player.ChoosingMove = false;
                                        Player.MP = Player.MP - NextMoveCost;

                                        if (Player.GetTemp() == "Ranged")
                                            Player.SetTexture(Content.Load<Texture2D>("Ranged"));
                                        else if (Player.GetTemp() == "Cavalry")
                                            Player.SetTexture(Content.Load<Texture2D>("Cavalry"));
                                        else if (Player.GetTemp() == "Warrior")
                                            Player.SetTexture(Content.Load<Texture2D>("Warrior"));
                                        else if (Player.GetTemp() == "Spear")
                                            Player.SetTexture(Content.Load<Texture2D>("Spear"));
                                    }
                                    unclick = false;
                                }
                            }
                        }
                    }
                }
            }

            else if (!TileReset)
            {
                foreach (Tile Space in Tiles)
                {
                    if (Space.GetFeature() == "Wheat")
                    {
                        Space.SetTexture(Content.Load<Texture2D>("Wheat Tile"));
                    }
                    else if (Space.GetFeature() == "Stone")
                    {
                        Space.SetTexture(Content.Load<Texture2D>("Stone Tile"));
                    }
                    else if (Space.GetFeature() == "Hill")
                    {
                        Space.SetTexture(Content.Load<Texture2D>("Hill Tile"));
                    }
                    else if (Space.GetFeature() == "Forest")
                    {
                        Space.SetTexture(Content.Load<Texture2D>("Forest Tile"));
                    }
                    else
                    {
                        Space.SetTexture(Content.Load<Texture2D>("Blank Tile"));
                    }
                    TileReset = true;
                }
            }


            if (UnitFinished && BaseMenu && Keyboard.GetState().IsKeyDown(Keys.Z) && unclick)   //Can only change unit production after last one has finished, keep it this way 
            {
                Bases[0].BaseMenu("Spear");
                UnitFinished = false;
                BaseMenu = false;
            }

            else if (UnitFinished && BaseMenu && Keyboard.GetState().IsKeyDown(Keys.X) && unclick)
            {
                Bases[0].BaseMenu("Warrior");
                UnitFinished = false;
                BaseMenu = false;
            }

            else if (UnitFinished && BaseMenu && Keyboard.GetState().IsKeyDown(Keys.C) && unclick)
            {
                Bases[0].BaseMenu("Ranged");
                UnitFinished = false;
                BaseMenu = false;
            }

            else if (UnitFinished && BaseMenu && Keyboard.GetState().IsKeyDown(Keys.V) && unclick)
            {
                Bases[0].BaseMenu("Cavalry");
                UnitFinished = false;
                BaseMenu = false;
            }




            //need to add map, buildings (in base), population, science, builders, fog of war, espionage, then AI 

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && unclick && !UnitFinished)
            {
                TurnNum++;
                foreach (Unit AIUnit in AIUnits)
                {
                    bool End = false;
                    while (!End)    //Moves AIUnits to random places 
                    {
                        Random r = new Random();
                        int RandomNum = r.Next(0, 252);
                        int CostOfMove = 0;
                        int[] TempPath = AIUnit.Path(AIUnit.TileNum, RandomNum, Tiles);
                        foreach (int Step in TempPath)
                        {
                            CostOfMove = CostOfMove + Tiles[Step].GetMoveCost();
                        }
                        if (CostOfMove <= AIUnit.MP && !Tiles[RandomNum].UnitPresent)
                        {
                            Tiles[RandomNum].UnitPresent = false;
                            AIUnit.TileNum = RandomNum;
                            Tiles[RandomNum].UnitPresent = true;
                            End = true;
                        }
                    }
                }

                AIUnits = Bases[1].NextTurn(AIUnits, Content.Load<Texture2D>("Warrior"), Content.Load<Texture2D>("Ranged"), Content.Load<Texture2D>("Spear"), Content.Load<Texture2D>("Cavalry"));

                Unit[] TempPlayerUnits = PlayerUnits;
                PlayerUnits = Bases[0].AddProduction(PlayerUnits, Content.Load<Texture2D>("Warrior"), Content.Load<Texture2D>("Ranged"), Content.Load<Texture2D>("Spear"), Content.Load<Texture2D>("Cavalry"));

                if (TempPlayerUnits != PlayerUnits)
                    UnitFinished = true;

                foreach(Unit Player in PlayerUnits)
                    Player.MP = Player.MaxMP;

                unclick = false;
            }

            base.Update(gameTime);
            
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Cam.transform);


            foreach (Tile tile in Tiles)
            {
                tile.Draw(spriteBatch);
            }
            Bases[0].Draw(spriteBatch);
            Bases[1].Draw(spriteBatch);

            foreach (Unit Enemy in AIUnits)
            {
                Enemy.Draw(spriteBatch, Tiles);
            }

            foreach(Unit Player in PlayerUnits)
            {
                Player.Draw(spriteBatch, Tiles);
            }


            Cam.Update(gameTime, Tiles);

            spriteBatch.End();



            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null);

            spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Turn: " + Convert.ToString(TurnNum), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
            if (BaseMenu)
            {
                spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Base Menu Open", new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.1f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Production: " + Bases[0].GetProduction(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.17f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Science: " + Bases[0].GetScience(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.24f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Food: " + Bases[0].GetFood(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.31f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Population: " + Bases[0].GetPopulation(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.38f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);

                //spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Forest: " + RightForestNum, new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.54f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                //spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Stone: " + RightStoneNum, new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.5f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                //spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Wheat: " + RightWheatNum, new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.42f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                //spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Hill: " + RightHillNum, new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.46f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);

                spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Z: Spear, X: Warrior, C: Ranged, V: Cavalry", new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.8f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
