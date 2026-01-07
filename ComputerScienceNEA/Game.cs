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
using System.Xml.Schema;
using System.IO;
using System.Diagnostics;

namespace ComputerScienceNEA
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera Cam;

        Tile[] Tiles;

        Base[] Bases;

        Random random = new Random();

        Rectangle Cursor;
        Rectangle TechBox = new Rectangle(Convert.ToInt32((910) + (0.447f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100))), Convert.ToInt32((-395) + (0.403f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100))), 100, 100);

        Unit[] AIUnits;
        Unit[] PlayerUnits;

        bool unclick = true;
        bool BaseMenu = false;
        bool UnitFinished = true;
        bool TileReset = true;
        bool ShowProdMessage = false;
        bool ShowPopMessage = false;
        bool ShowSciMessage = false;
        bool ManagePop = false;
        bool TechMenu = false;
        bool UnitSelected = false;

        string InitialJob = "";
        string NewJob = "";

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

        float[] Weightings = new float[44];
        float[] OldWeightings = new float[44];
        int[] PreviousScores = new int[5];
        int AIScore = 0;
        int PlayerKills = 0;
        bool GameEnd = false;
        bool PlayerWin = false;
        bool WeightsChanged = false;

        int TurnSinceProd = 1;


        int TurnNum = 1;

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

            StreamReader Wsr = new StreamReader("Weight.txt");
            int Counter = 0;

            //Weightings Alterations
            foreach (float Weight in Weightings)
            {
                Weightings[Counter] = float.Parse(Wsr.ReadLine());
                Counter++;
            }
            Wsr.Dispose();
            StreamReader Wsr2 = new StreamReader("Weight.txt");
            Counter = 0;
            foreach (float Weight in OldWeightings)
            {
                OldWeightings[Counter] = float.Parse(Wsr2.ReadLine());
                Counter++;
            }
            Wsr2.Dispose();

			//Want to change a number of weightings (depending on the the average of the previous 5 scores) by a certain percentage (depending on the the average of the previous 5 scores) 
			//Minimum weightings changed should be either 1 or 2 and the maximum should be about 6, minimum percentage changed is about 2% and maximum is about 30%
			//The weightings that are changed should be selected at random 
			//The minimums should occur when the score average is greater than or equal to 500
			//The maximums should occur when the score average is less than or equal to -500

			StreamReader Ssr = new StreamReader("Scores.txt");
            int ChangeNum;
            float PercentChange = 0.02f;
            int ScoreAverage = 0;

            Counter = 0;
            foreach(int Score in PreviousScores)
            {
                PreviousScores[Counter] = int.Parse(Ssr.ReadLine());
                ScoreAverage = ScoreAverage + PreviousScores[Counter];
                Counter++;
            }
            ScoreAverage = ScoreAverage / 5;

            if (ScoreAverage >= 500)
            {
                ChangeNum = 2;
            }
            else if(ScoreAverage >= 250)
            {
                ChangeNum = 3;
            }
            else if (ScoreAverage >= 0)
            {
                ChangeNum = 4;
            }
            else if (ScoreAverage >= -250)
            {
                ChangeNum = 5;
            }
            else
            {
                ChangeNum = 6;
            }

            if(ScoreAverage > 500)
            {
                ScoreAverage = 500;
            }
            if(ScoreAverage < -500)
            {
                ScoreAverage = -500;
            }

            if (ScoreAverage > 1)
            {
                PercentChange = ((0.056f * ScoreAverage) + 2) / 100;
            }
            if (ScoreAverage < -1)
            {
                PercentChange = ((0.056f * ScoreAverage) - 2) / 100;
            }
            

            bool End = false;
            bool Used = false;
            int[] Changed = new int[0];

            Counter = 0;
            foreach(int Num in Changed)
            {
                Changed[Counter] = -1;
                Counter++;
            }

            int SuccessIteration = 0;
            while (!End)
            {
                Used = false;
                int Rand = random.Next(0, Weightings.Length);
                int Random = random.Next(0, 2);
                foreach (int Num in Changed)
                {
                    if(Rand == Num)
                    {
                        Used = true;
                    }
                }
                if (!Used)
                {
                    if (Random == 1)
                    {
                        Weightings[Rand] = Weightings[Rand] * (1 - PercentChange);
                    }
                    else
                    {
                        Weightings[Rand] = Weightings[Rand] * (1 + PercentChange);
                    }
                    Array.Resize(ref Changed, (Changed.Length + 1));
                    Changed[Changed.Length - 1] = Rand;
                    SuccessIteration++;
                }
                if(SuccessIteration == ChangeNum)
                {
                    End = true;
                }
            }


            int row = 1;
            int position = 1;
            for (int i = 0; i < 252; i++)
            {
                Tiles[i] = new Tile(row, position, 1, new Vector2(position * 200, row * 200), Content.Load<Texture2D>("Blank Tile"), "", i);
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
                else if (Space.GetListNum() != 136)
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
            Bases[0] = new Base(true, 115, Content.Load<Texture2D>("Base Placeholder"));
            Bases[1] = new Base(false, 136, Content.Load<Texture2D>("Base Placeholder"));
            Bases[0].BaseTechChoices();
            Bases[0].CombatTechChoices();
            PlayerUnits = new Unit[0];  //As more units are made the program will use Array.Resize(ref PlayerUnits, NewSize) to increase the size of this array

            IsMouseVisible = true;
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Released && Keyboard.GetState().IsKeyUp(Keys.Space) && Keyboard.GetState().IsKeyUp(Keys.F1) && Keyboard.GetState().IsKeyUp(Keys.F2) && Keyboard.GetState().IsKeyUp(Keys.F3) && Keyboard.GetState().IsKeyUp(Keys.P) && Keyboard.GetState().IsKeyUp(Keys.Z) && Keyboard.GetState().IsKeyUp(Keys.X) && Keyboard.GetState().IsKeyUp(Keys.C) && Keyboard.GetState().IsKeyUp(Keys.V) && Keyboard.GetState().IsKeyUp(Keys.B) && Keyboard.GetState().IsKeyUp(Keys.N) && Keyboard.GetState().IsKeyUp(Keys.M) && Keyboard.GetState().IsKeyUp(Keys.L) && Keyboard.GetState().IsKeyUp(Keys.K) && Keyboard.GetState().IsKeyUp(Keys.J) && Keyboard.GetState().IsKeyUp(Keys.H) && Keyboard.GetState().IsKeyUp(Keys.D1) && Keyboard.GetState().IsKeyUp(Keys.NumPad1) && Keyboard.GetState().IsKeyUp(Keys.T) && !unclick)
                unclick = true;

            if ((Mouse.GetState().LeftButton == ButtonState.Pressed))
            {
                TileReset = false;
                Cursor = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 5, 5);

                foreach (Tile tile in Tiles)
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
                            TechMenu = false;
                            unclick = false;
                        }

                        else
                        {
                            int Counter = -1;
                            foreach (Unit Player in PlayerUnits)
                            {
                                Counter++;
                                if (Player != null)
                                {
                                    if (tile.GetListNum() == Player.TileNum && !UnitSelected && unclick)
                                    {
                                        Player.Selected = true;
                                        UnitSelected = true;
                                        Player.SetTexture(Content.Load<Texture2D>("Selected"));
                                        unclick = false;
                                    }

                                    if (tile.GetListNum() == Player.TileNum && Player.Selected && unclick)
                                    {
                                        Player.Selected = false;
                                        UnitSelected = false;
                                        if (Player.UnitType == "Ranged")
                                            Player.SetTexture(Content.Load<Texture2D>("Ranged"));
                                        else if (Player.UnitType == "Cavalry")
                                            Player.SetTexture(Content.Load<Texture2D>("Cavalry"));
                                        else if (Player.UnitType == "Warrior")
                                            Player.SetTexture(Content.Load<Texture2D>("Warrior"));
                                        else if (Player.UnitType == "Spear")
                                            Player.SetTexture(Content.Load<Texture2D>("Spear"));
                                        unclick = false;
                                    }

                                    if (tile.GetListNum() != Player.TileNum && Player.Selected && unclick && Player.MP != 0 && !tile.PlayerPresent)
                                    {
                                        int[] Path = Player.Path(Player.TileNum, tile.GetListNum(), Tiles);
                                        int NextMoveCost = 0;
                                        foreach (int Move in Path)
                                        {
                                            if (Tiles[Move].GetMoveCost() == 99999 && Tiles[Move].EnemyPresent && (Tiles[Move].GetFeature() == "Wheat" || Tiles[Move].GetFeature() == "Regular"))
                                            {
                                                NextMoveCost = NextMoveCost + 1;
                                            }
                                            else if (Tiles[Move].GetMoveCost() == 99999 && Tiles[Move].EnemyPresent)
                                            {
                                                NextMoveCost = NextMoveCost + 2;
                                            }
                                            else
                                            {
                                                NextMoveCost = NextMoveCost + Tiles[Move].GetMoveCost();
                                            }
                                        }
                                        NextMoveCost = NextMoveCost - Tiles[Player.TileNum].GetMoveCost();

                                        if (tile.GetListNum() == Bases[1].GetListNum() && Player.Selected && Player.MP != 0 && unclick)
                                        {
                                            if (Player.UnitType == "Ranged" && Player.InRange(Tiles, Player.TileNum, tile.GetListNum(), 2, 0))
                                            {
                                                Bases[1].Health = Bases[1].Health - Player.BaseRangedAttack(Player, Bases[1]);
                                                Player.MP = 0;
                                                Player.Selected = false;
                                                UnitSelected = false;
                                            }
                                            if (Player.UnitType != "Ranged" && Player.MP >= NextMoveCost)
                                            {
                                                int[] Damage = Player.BaseCombat(Player, Bases[1]);
                                                Player.Health = Player.Health - Damage[0];
                                                Bases[1].Health = Bases[1].Health - Damage[1];
                                                Player.MP = 0;
                                                Player.Selected = false;
                                                UnitSelected = false;

                                                if (Player.Health <= 0)
                                                {
                                                    if (Tiles[PlayerUnits[Counter].TileNum].GetFeature() == "Wheat" || Tiles[PlayerUnits[Counter].TileNum].GetFeature() == "Regular")
                                                    {
                                                        Tiles[PlayerUnits[Counter].TileNum].SetMoveCost(1);
                                                    }
                                                    else
                                                    {
                                                        Tiles[PlayerUnits[Counter].TileNum].SetMoveCost(2);
                                                    }
                                                    PlayerUnits[Counter] = null;
                                                    Array.Resize(ref PlayerUnits, PlayerUnits.Length - 1);
                                                }
                                            }
                                        }

                                        if (NextMoveCost <= Player.MP && Player.MP != 0 && !tile.EnemyPresent)
                                        {
                                            Tiles[Player.TileNum].PlayerPresent = false;
                                            if (Tiles[Player.TileNum].GetFeature() == "Wheat" || Tiles[Player.TileNum].GetFeature() == "Regular")
                                            {
                                                Tiles[Player.TileNum].SetMoveCost(1);
                                            }
                                            else
                                            {
                                                Tiles[Player.TileNum].SetMoveCost(2);
                                            }
                                            Player.TileNum = tile.GetListNum();
                                            Tiles[Player.TileNum].SetMoveCost(99999);
                                            Player.Selected = false;
                                            UnitSelected = false;
                                            Player.MP = Player.MP - NextMoveCost;

                                            if (Player.UnitType == "Ranged")
                                                Player.SetTexture(Content.Load<Texture2D>("Ranged"));
                                            else if (Player.UnitType == "Cavalry")
                                                Player.SetTexture(Content.Load<Texture2D>("Cavalry"));
                                            else if (Player.UnitType == "Warrior")
                                                Player.SetTexture(Content.Load<Texture2D>("Warrior"));
                                            else if (Player.UnitType == "Spear")
                                                Player.SetTexture(Content.Load<Texture2D>("Spear"));
                                            tile.PlayerPresent = true;
                                        }

                                        if (NextMoveCost <= Player.MP && Player.MP != 0 && tile.EnemyPresent && Player.UnitType != "Ranged")
                                        {
                                            int[] Damage = Player.Combat(Player, AIUnits[tile.GetUnitNum()], Tiles, Bases);
                                            Player.Health = Player.Health - Damage[0];
                                            AIUnits[tile.GetUnitNum()].Health = AIUnits[tile.GetUnitNum()].Health - Damage[1];
                                            
                                            if (Player.Health <= 0)
                                            {
                                                if (Tiles[PlayerUnits[Counter].TileNum].GetFeature() == "Wheat" || Tiles[PlayerUnits[Counter].TileNum].GetFeature() == "Regular")
                                                {
                                                    Tiles[PlayerUnits[Counter].TileNum].SetMoveCost(1);
                                                }
                                                else
                                                {
                                                    Tiles[PlayerUnits[Counter].TileNum].SetMoveCost(2);
                                                }
                                                PlayerUnits[Counter] = null;
                                            }
                                            if (AIUnits[tile.GetUnitNum()].Health <= 0)
                                            {
                                                if (Tiles[AIUnits[tile.GetUnitNum()].TileNum].GetFeature() == "Wheat" || Tiles[AIUnits[tile.GetUnitNum()].TileNum].GetFeature() == "Regular")
                                                {
                                                    Tiles[AIUnits[tile.GetUnitNum()].TileNum].SetMoveCost(1);
                                                }
                                                else
                                                {
                                                    Tiles[AIUnits[tile.GetUnitNum()].TileNum].SetMoveCost(2);
                                                }
                                                AIUnits[tile.GetUnitNum()] = null;
                                            }
                                            Player.MP = 0;
                                            Player.Selected = false;
                                            UnitSelected = false;
                                            if (Player.UnitType == "Ranged")
                                                Player.SetTexture(Content.Load<Texture2D>("Ranged"));
                                            else if (Player.UnitType == "Cavalry")
                                                Player.SetTexture(Content.Load<Texture2D>("Cavalry"));
                                            else if (Player.UnitType == "Warrior")
                                                Player.SetTexture(Content.Load<Texture2D>("Warrior"));
                                            else if (Player.UnitType == "Spear")
                                                Player.SetTexture(Content.Load<Texture2D>("Spear"));

                                        }
                                        if (tile.EnemyPresent && Player.UnitType == "Ranged" && Player.MP != 0 && Player.InRange(Tiles, Player.TileNum, tile.GetListNum(), 2, 0))
                                        {
                                            int Damage = Player.RangedAttack(Player, AIUnits[tile.GetUnitNum()], Tiles);
                                            AIUnits[tile.GetUnitNum()].Health = AIUnits[tile.GetUnitNum()].Health - Damage;
                                            if (AIUnits[tile.GetUnitNum()].Health <= 0)
                                            {
                                                if (Tiles[AIUnits[tile.GetUnitNum()].TileNum].GetFeature() == "Wheat" || Tiles[AIUnits[tile.GetUnitNum()].TileNum].GetFeature() == "Regular")
                                                {
                                                    Tiles[AIUnits[tile.GetUnitNum()].TileNum].SetMoveCost(1);
                                                }
                                                else
                                                {
                                                    Tiles[AIUnits[tile.GetUnitNum()].TileNum].SetMoveCost(2);
                                                }
                                                AIUnits[tile.GetUnitNum()] = null;
                                            }
                                            Player.MP = 0;
                                            Player.Selected = false;
                                            UnitSelected = false;
                                            if (Player.UnitType == "Ranged")
                                                Player.SetTexture(Content.Load<Texture2D>("Ranged"));
                                            else if (Player.UnitType == "Cavalry")
                                                Player.SetTexture(Content.Load<Texture2D>("Cavalry"));
                                            else if (Player.UnitType == "Warrior")
                                                Player.SetTexture(Content.Load<Texture2D>("Warrior"));
                                            else if (Player.UnitType == "Spear")
                                                Player.SetTexture(Content.Load<Texture2D>("Spear"));
                                        }
                                        unclick = false;
                                    }
                                }
                            }

                            foreach (Unit Enemy in AIUnits)
                            {
                                if (Enemy != null)
                                {
                                    if (tile.GetListNum() == Enemy.TileNum && !UnitSelected && unclick)
                                    {
                                        Enemy.Selected = true;
                                        UnitSelected = true;
                                        unclick = false;
                                    }
                                    if (Enemy.Selected && tile.GetListNum() == Enemy.TileNum && unclick)
                                    {
                                        Enemy.Selected = false;
                                        UnitSelected = false;
                                        unclick = false;
                                    }
                                }
                            }
                        }
                    }
                }
            
                if (TechBox.Intersects(Cursor) && !TechMenu && unclick)
                {
                    TechMenu = true;
                    BaseMenu = false;
                    unclick = false;
                }
                else if (TechBox.Intersects(Cursor) && unclick)
                {
                    TechMenu = false;
                    unclick = false;
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


            if (Bases[0].Health <= 0 && !GameEnd)
            {
                PlayerWin = false;
                GameEnd = true;
                foreach(Unit AI in AIUnits)
                {
                    if(AI == null)
                    {
                        PlayerKills++;
                    }
                }

                int AIKills = 0;
                foreach(Unit Player in PlayerUnits)
                {
                    if(Player == null)
                    {
                        AIKills++;
                    }
                }

                AIScore = 500;
                AIScore = AIScore - (PlayerKills * 10);
                AIScore = AIScore + (AIKills * 10);
                AIScore = AIScore + (AIUnits.Length * 2);
                AIScore = AIScore - (TurnNum * 4);
                if (AIScore <= 0)
                {
                    AIScore = 1;
                }
            }

            if (Bases[1].Health <= 0 && !GameEnd)
            {
                PlayerWin = true;
                GameEnd = true;
                foreach (Unit AI in AIUnits)
                {
                    if (AI == null)
                    {
                        PlayerKills++;
                    }
                }

                int AIKills = 0;
                foreach (Unit Player in PlayerUnits)
                {
                    if (Player == null)
                    {
                        AIKills++;
                    }
                }

                AIScore = -500;
                AIScore = AIScore - (PlayerKills * 10);
                AIScore = AIScore + (AIKills * 10);
                AIScore = AIScore + (AIUnits.Length * 2);
                AIScore = AIScore + (TurnNum * 2);
                if (AIScore >= 0)
                {
                    AIScore = -1;
                }
            }

            if(GameEnd && !WeightsChanged)
            {
                WeightsChanged = true;
                int ScoreAverage = 0;
                float PercentageChange;
                foreach(int Score in PreviousScores)
                {
                    ScoreAverage = ScoreAverage + Score;
                }
                ScoreAverage = ScoreAverage / 5;
                PercentageChange = (ScoreAverage - AIScore) / ScoreAverage;
                if(PercentageChange > 1)
                {
                    PercentageChange = 1;
                }
                else if (PercentageChange < 0.5f && PercentageChange > 0)
                {
                    PercentageChange = 0.5f;
                }
                else if (PercentageChange > -0.5f && PercentageChange < 0)
                {
                    PercentageChange = -0.5f;
                }
                else if(PercentageChange < -1)
                {
                    PercentageChange = -1;
                }

                int Counter = 0;
                StreamWriter I = new StreamWriter("Weight.txt", false);
                I.Write("");
                I.Dispose();
                StreamWriter Wsw = new StreamWriter("Weight.txt", true);
                foreach (float Weight in Weightings)
                {
                    Wsw.WriteLine(OldWeightings[Counter] + ((Weight - OldWeightings[Counter]) * PercentageChange));
                    Counter++;
                }
                Wsw.Dispose();

                Counter = 0;
                StreamWriter II = new StreamWriter("Scores.txt", false);
                II.Write("");
                II.Dispose();
                StreamWriter Ssw = new StreamWriter("Scores.txt", true);
                foreach (int Weight in PreviousScores)
                {
                    if (Counter < 4)
                    {
                        Ssw.WriteLine(PreviousScores[Counter + 1]);
                    }
                    else
                    {
                        Ssw.WriteLine(AIScore);
                    }
                    Counter++;
                }
                Ssw.Dispose();

            }

            if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.Z) && unclick)
            {
                Bases[0].BaseMenu("Spear");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.X) && unclick)
            {
                Bases[0].BaseMenu("Warrior");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.C) && unclick)
            {
                Bases[0].BaseMenu("Ranged");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.V) && unclick)
            {
                Bases[0].BaseMenu("Cavalry");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.B) && unclick)
            {
                Bases[0].BaseMenu("Granary");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.N) && unclick)
            {
                Bases[0].BaseMenu("Blacksmith");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.M) && unclick)
            {
                Bases[0].BaseMenu("Barracks");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.L) && unclick)
            {
                Bases[0].BaseMenu("School");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.K) && unclick)
            {
                Bases[0].BaseMenu("University");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.H) && unclick)
            {
                Bases[0].BaseMenu("Outpost");
                UnitFinished = false;
                ShowProdMessage = false;
                unclick = false;
            }
            else if (BaseMenu && Keyboard.GetState().IsKeyDown(Keys.P) && unclick)
            {
                if (!ManagePop)
                    ManagePop = true;
                else
                    ManagePop = false;
                unclick = false;
            }
            if (BaseMenu && ManagePop && Keyboard.GetState().IsKeyDown(Keys.F1) && InitialJob == "" && unclick && !Bases[0].GetUnassigned())
            {
                InitialJob = "Worker";
                unclick = false;
            }
            else if (BaseMenu && ManagePop && Keyboard.GetState().IsKeyDown(Keys.F2) && InitialJob == "" && unclick && !Bases[0].GetUnassigned())
            {
                InitialJob = "Farmer";
                unclick = false;
            }
            else if (BaseMenu && ManagePop && Keyboard.GetState().IsKeyDown(Keys.F3) && InitialJob == "" && unclick && !Bases[0].GetUnassigned())
            {
                InitialJob = "Researcher";
                unclick = false;
            }
            if (BaseMenu && ManagePop && Keyboard.GetState().IsKeyDown(Keys.F1) && InitialJob != "" && unclick && !Bases[0].GetUnassigned())
            {
                NewJob = "Worker";
                Bases[0].MovePop(InitialJob, NewJob);
                InitialJob = "";
                unclick = false;
            }
            else if (BaseMenu && ManagePop && Keyboard.GetState().IsKeyDown(Keys.F2) && InitialJob != "" && unclick && !Bases[0].GetUnassigned())
            {
                NewJob = "Farmer";
                Bases[0].MovePop(InitialJob, NewJob);
                InitialJob = "";
                unclick = false;
            }
            else if (BaseMenu && ManagePop && Keyboard.GetState().IsKeyDown(Keys.F3) && InitialJob != "" && unclick && !Bases[0].GetUnassigned())
            {
                NewJob = "Researcher";
                Bases[0].MovePop(InitialJob, NewJob);
                InitialJob = "";
                unclick = false;
            }
            if (BaseMenu && ManagePop && Keyboard.GetState().IsKeyDown(Keys.F1) && unclick && Bases[0].GetUnassigned())
            {
                Bases[0].AssignWorker();
                unclick = false;
                ShowPopMessage = false;
            }
            else if (BaseMenu && ManagePop && Keyboard.GetState().IsKeyDown(Keys.F2) && unclick && Bases[0].GetUnassigned())
            {
                Bases[0].AssignFarmer();
                unclick = false;
                ShowPopMessage = false;
            }
            else if (BaseMenu && ManagePop && Keyboard.GetState().IsKeyDown(Keys.F3) && unclick && Bases[0].GetUnassigned())
            {
                Bases[0].AssignResearcher();
                unclick = false;
                ShowPopMessage = false;
            }
            if(TechMenu && Bases[0].GetBaseTech() == "" && (Keyboard.GetState().IsKeyDown(Keys.D1) || Keyboard.GetState().IsKeyDown(Keys.NumPad1)) && unclick)
            {
                Bases[0].SetTech(Bases[0].BaseChoices[0], false);
                unclick = false;
                if (Bases[0].GetCombatTech() != "")
                    ShowSciMessage = false;
            }
            else if (TechMenu && Bases[0].GetBaseTech() == "" && (Keyboard.GetState().IsKeyDown(Keys.D2) || Keyboard.GetState().IsKeyDown(Keys.NumPad1)) && unclick)
            {
                Bases[0].SetTech(Bases[0].BaseChoices[1], false);
                unclick = false;
                if (Bases[0].GetCombatTech() != "")
                    ShowSciMessage = false;
            }
            else if (TechMenu && Bases[0].GetBaseTech() == "" && (Keyboard.GetState().IsKeyDown(Keys.D3) || Keyboard.GetState().IsKeyDown(Keys.NumPad1)) && unclick)
            {
                Bases[0].SetTech(Bases[0].BaseChoices[2], false);
                unclick = false;
                if (Bases[0].GetCombatTech() != "")
                    ShowSciMessage = false;
            }
            if (TechMenu && Bases[0].GetCombatTech() == "" && (Keyboard.GetState().IsKeyDown(Keys.D4) || Keyboard.GetState().IsKeyDown(Keys.NumPad1)) && unclick)
            {
                Bases[0].SetTech(Bases[0].CombatChoices[0], true);
                unclick = false;
                if (Bases[0].GetBaseTech() != "")
                    ShowSciMessage = false;
            }
            else if (TechMenu && Bases[0].GetCombatTech() == "" && (Keyboard.GetState().IsKeyDown(Keys.D5) || Keyboard.GetState().IsKeyDown(Keys.NumPad1)) && unclick)
            {
                Bases[0].SetTech(Bases[0].CombatChoices[1], true);
                unclick = false;
                if (Bases[0].GetBaseTech() != "")
                    ShowSciMessage = false;
            }
            else if (TechMenu && Bases[0].GetCombatTech() == "" && (Keyboard.GetState().IsKeyDown(Keys.D6) || Keyboard.GetState().IsKeyDown(Keys.NumPad1)) && unclick)
            {
                Bases[0].SetTech(Bases[0].CombatChoices[2], true);
                unclick = false;
                if (Bases[0].GetBaseTech() != "")
                    ShowSciMessage = false;
            }
            if (TechMenu && Keyboard.GetState().IsKeyDown(Keys.T) && unclick)
            {
                Bases[0].ChangeFocus();
                unclick = false;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && unclick && !UnitFinished && !Bases[0].GetUnassigned() && !(Bases[0].GetCombatTech() == "" || Bases[0].GetBaseTech() == "")) //NEXT TURN
            {
                TurnNum++;
                int[] Moves = new int[49*AIUnits.Length];
                int[] MoveCosts = new int[AIUnits.Length * PlayerUnits.Length];
                float[] MoveValues = new float[49*AIUnits.Length];
                float[] ECSHT = new float[PlayerUnits.Length]; //Stands for Enemy Combat Strength Health Total
                float[] FCSHT = new float[AIUnits.Length]; //Stands for Friendly Combat Strength Health Total
                float[] CSHT = new float[ECSHT.Length * FCSHT.Length];
                bool[] BaseBeatable = new bool[AIUnits.Length];
                float BCSHT = 0; //Stands for Base Combat Strength Health Total
                int Counter = 0;
                foreach (int I in MoveValues)
                {
                    MoveValues[Counter] = 0;
                    Counter++;
                }

                Counter = 0;
                foreach (Unit Player in PlayerUnits)
                {
                    if (Player != null)
                    {
                        int FirstTile = Player.TileNum - 87;

                        ECSHT[Counter] = (Player.CS * Weightings[1]) + (Player.Health * Weightings[0]);
                        for (int A = 0; A < 7; A++)
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                if (((FirstTile + i) >= 0) && ((FirstTile + i) <= 251))
                                {
                                    if (Tiles[FirstTile + i].PlayerPresent)
                                    {
                                        Unit Nearby = PlayerUnits[Tiles[FirstTile + i].GetUnitNum()];
                                        ECSHT[Counter] = ECSHT[Counter] + Nearby.CS + Nearby.Health;

                                        if (Nearby.InRange(Tiles, Nearby.TileNum, 115, 2, 0))
                                        {
                                            ECSHT[Counter] = ECSHT[Counter] + Bases[0].GetSupport();
                                        }
                                    }
                                }
                            }
                            FirstTile = FirstTile + 28;
                        }
                    }
                    Counter++;
                }

                Counter = 0;
                foreach (Unit AIUnit in AIUnits)    //wait unit end of loop to move units and if two units want to move to the same space then move the one with the highest value for that tile and then do the next best move for the other one, also so that the FCSHT stays the same for all units as if they moved they might get out of range so something to keep in mind
                {  //Also if a unit is on the outside of the units radius it might find that it shouldn't attack so if one unit decides that all should attack should then check that other units in radius dont have some better moves, if they do recalculate it without that unit and if not make all the units attack the other units 
                   //judge whether or not to move towards enemy by looking at if it and the units within two tiles of it have a better total CS and health than the enemy unit and the enemy units within two tiles of it may need to make health worth more/less than CS for better decision making, also don't forget to consider the enemy's bonuses and support value 
                    if (AIUnit != null)
                    {
                        int[] NearAI = new int[0];
                        int FirstTile = AIUnit.TileNum - 87;

                        FCSHT[Counter] = (AIUnit.CS * Weightings[1]) + (AIUnit.Health * Weightings[0]);
                        for (int A = 0; A < 7; A++)
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                if (((FirstTile + i) >= 0) && ((FirstTile + i) <= 251))
                                {
                                    int[] Path = AIUnit.Path(AIUnit.TileNum, Tiles[FirstTile + i].GetListNum(), Tiles);
                                    int MP = 0;
                                    foreach (int Move in Path)
                                    {
                                        if (Tiles[Move].GetMoveCost() == 99999 && Tiles[Move].PlayerPresent && (Tiles[Move].GetFeature() == "Wheat" || Tiles[Move].GetFeature() == "Regular"))
                                        {
                                            MP = MP + 1;
                                        }
                                        else if (Tiles[Move].GetMoveCost() == 99999 && Tiles[Move].PlayerPresent)
                                        {
                                            MP = MP + 2;
                                        }
                                        else
                                        {
                                            MP = MP + Tiles[Move].GetMoveCost();
                                        }
                                    }
                                    MP = MP - Tiles[AIUnit.TileNum].GetMoveCost();

                                    if (MP <= AIUnit.MP)
                                    {
                                        Moves[i + (A * 7) + (Counter * 49)] = FirstTile + i;
                                    }
                                    else
                                    {
                                        Moves[i + (A * 7) + (Counter * 49)] = -1;
                                    }

                                    if (Tiles[FirstTile + i].EnemyPresent && (i <= 5 && A <= 5) || (i >= 1 && A >= 1)) 
                                    {
                                        bool InArray = true;
                                        if (NearAI != null)
                                        {
                                            InArray = false;
                                            foreach (int AI in NearAI)
                                            {
                                                if (AI == Tiles[FirstTile + i].GetUnitNum())
                                                {
                                                    InArray = true;
                                                }
                                            }
                                        }
                                        if (!InArray && Tiles[FirstTile + i].GetUnitNum() != AIUnit.TileNum)
                                        {
                                            Array.Resize(ref NearAI, NearAI.Length + 1);
                                            NearAI[NearAI.Length - 1] = Tiles[FirstTile + i].GetUnitNum();
                                        }
                                    }
                                }
                                else
                                {
                                    Moves[i + (A * 7) + (Counter * 49)] = -1;
                                }
                            }
                            FirstTile = FirstTile + 28;
                        }
                        foreach (int AINum in NearAI)
                        {
                            if (AIUnits[AINum] != null && AIUnits[AINum] != AIUnit)
                            {
                                FCSHT[Counter] = FCSHT[Counter] + (AIUnits[AINum].Health * Weightings[0]) + (AIUnits[AINum].CS * Weightings[1]);
                            }
                        }
                    }
                    Counter++;
                }

                Counter = 0;
                foreach(int FCSH in FCSHT)
                {
                    foreach(int ECSH in ECSHT)
                    {
                        CSHT[Counter] = (FCSH * Weightings[4]) - (ECSH * Weightings[5]);
                        Counter++;
                    }
                }
                Counter = 0;
                foreach(int Num in CSHT)
                {
                    if(Num >= 0)
                    {
                        CSHT[Counter] = (1 * Weightings[2]);
                    }
                    else if (Num < 0) 
                    {
                        CSHT[Counter] = (-1 * Weightings[3]);
                    }
                    Counter++;
                }

                BCSHT = BCSHT + (Bases[0].Health * Weightings[6]) + (Bases[0].CS * Weightings[7]);
                for(int I = 0; I < 5; I++)
                {
                    for(int A = 0; A < 5; A++)
                    {
                        if (Tiles[(Bases[0].GetListNum() - 58) + I + (A * 28)].PlayerPresent)
                        {
                            BCSHT = BCSHT + (PlayerUnits[Tiles[(Bases[0].GetListNum() - 58) + I + (A * 28)].GetUnitNum()].CS * Weightings[8]) + (PlayerUnits[Tiles[(Bases[0].GetListNum() - 58) + I + (A * 28)].GetUnitNum()].Health * Weightings[9]);
                        }
                    }
                }




                foreach(Unit AI in AIUnits)
                {
                    if (AI != null)
                    {
                        if (BCSHT < FCSHT[AI.ListNum])
                        {
                            int[] BasePath = AI.Path(AI.TileNum, Bases[0].GetListNum(), Tiles);
                            int TotalMoveCost2 = 0;
                            int FinalMove2 = -1;
                            int LastFinalMove2 = -2;
                            int BestMoveCost2 = -999;
                            int Distance2 = BasePath.Length - 1;
                            bool MoveFound2 = false;

                            if (Tiles[BasePath[1]].EnemyPresent && !AI.InRange(Tiles, AI.TileNum, BasePath[0], 3, 0))
                            {
                                foreach (int Adjacent in Tiles[AI.TileNum].GetAdTiles())
                                {
                                    int[] NewPath = AI.Path(Adjacent, Bases[0].GetListNum(), Tiles);
                                    int MC = 0; //Stands for Move Cost 
                                    foreach (int Move in NewPath)
                                    {
                                        MC = MC + Tiles[Move].GetMoveCost();
                                    }
                                    MC = MC - Tiles[AI.TileNum].GetMoveCost();
                                    if (MC > BestMoveCost2 && !Tiles[BasePath[0]].EnemyPresent)
                                    {
                                        BestMoveCost2 = MC;
                                        BasePath = NewPath;
                                    }
                                }
                            }

                            while (!MoveFound2)
                            {
                                Counter = 0;
                                foreach (int Move in BasePath)
                                {
                                    if (Counter >= Distance2)
                                    {
                                        TotalMoveCost2 = TotalMoveCost2 + Tiles[Move].GetMoveCost();
                                    }
                                    Counter++;
                                }
                                TotalMoveCost2 = TotalMoveCost2 - Tiles[AI.TileNum].GetMoveCost();

                                if (TotalMoveCost2 <= AI.MP && !AI.InRange(Tiles, AI.TileNum, BasePath[0], 2, 0))
                                {
                                    FinalMove2 = BasePath[Distance2];
                                }
                                else if (AI.InRange(Tiles, AI.TileNum, BasePath[0], 2, 0))
                                {
                                    FinalMove2 = BasePath[0];
                                }
                                if (LastFinalMove2 == FinalMove2)
                                {
                                    MoveFound2 = true;
                                }
                                if (Distance2 > 0)
                                {
                                    Distance2 = Distance2 - 1;
                                }

                                LastFinalMove2 = FinalMove2;
                                TotalMoveCost2 = 0;
                            }

                            for (int i = 0; i < 49; i++)
                            {
                                if (Moves[i + (AI.ListNum * 49)] == FinalMove2)
                                {
                                    MoveValues[i + (AI.ListNum * 49)] = MoveValues[i + (AI.ListNum * 49)] + (30 * Weightings[10]);
                                }
                            }
                        }

                        foreach (Unit Player in PlayerUnits)
                        {
                            if (Player != null)
                            {
                                int[] Path = AI.Path(AI.TileNum, Player.TileNum, Tiles);
                                int TotalMoveCost = 0;
                                int FinalMove = -1;
                                int LastFinalMove = -2;
                                int Distance = Path.Length - 1;
                                int BestMoveCost = -999;
                                bool MoveFound = false;

                                if (Tiles[Path[1]].EnemyPresent && !AI.InRange(Tiles, AI.TileNum, Path[0], 3, 0))
                                {
                                    foreach (int Adjacent in Tiles[AI.TileNum].GetAdTiles())
                                    {
                                        int[] NewPath = AI.Path(Adjacent, Player.TileNum, Tiles);
                                        int MC = 0; //Stands for Move Cost 
                                        foreach (int Move in NewPath)
                                        {
                                            if (Tiles[Move].GetMoveCost() == 99999 && Tiles[Move].PlayerPresent && (Tiles[Move].GetFeature() == "Wheat" || Tiles[Move].GetFeature() == "Regular"))
                                            {
                                                MC = MC + 1;
                                            }
                                            else if (Tiles[Move].GetMoveCost() == 99999 && Tiles[Move].PlayerPresent)
                                            {
                                                MC = MC + 2;
                                            }
                                            else
                                            {
                                                MC = MC + Tiles[Move].GetMoveCost();
                                            }
                                        }
                                        MC = MC - Tiles[AI.TileNum].GetMoveCost();
                                        if (MC > BestMoveCost && !Tiles[Path[0]].EnemyPresent)
                                        {
                                            BestMoveCost = MC;
                                            Path = NewPath;
                                        }
                                    }
                                }

                                while (!MoveFound)
                                {
                                    Counter = 0;
                                    foreach (int Move in Path)
                                    {
                                        if (Counter >= Distance)
                                        {
                                            TotalMoveCost = TotalMoveCost + Tiles[Move].GetMoveCost();
                                        }
                                        Counter++;
                                    }
                                    TotalMoveCost = TotalMoveCost - Tiles[AI.TileNum].GetMoveCost();

                                    if (TotalMoveCost <= AI.MP && !AI.InRange(Tiles, AI.TileNum, Path[0], 2, 0))
                                    {
                                        FinalMove = Path[Distance];
                                    }
                                    else if (AI.InRange(Tiles, AI.TileNum, Path[0], 2, 0))
                                    {
                                        FinalMove = Path[0];
                                    }
                                    if (LastFinalMove == FinalMove)
                                    {
                                        MoveFound = true;
                                    }
                                    if (Distance > 0)
                                    {
                                        Distance = Distance - 1;
                                    }

                                    LastFinalMove = FinalMove;
                                    TotalMoveCost = 0;
                                }

                                for (int i = 0; i < 49; i++)
                                {
                                    if (Moves[i + (AI.ListNum * 49)] == FinalMove && !(MoveValues[i + (AI.ListNum * 49)] > CSHT[(AI.ListNum * PlayerUnits.Length) + Player.ListNum]))
                                    {
                                        MoveValues[i + (AI.ListNum * 49)] = CSHT[(AI.ListNum * PlayerUnits.Length) + Player.ListNum];
                                        CSHT[(AI.ListNum * PlayerUnits.Length) + Player.ListNum] = -999;    //Used to indicate that it's already been used 
                                    }
                                }
                            }
                        }
                    }
                }

                Counter = 0;
                foreach (Unit AI in AIUnits)
                {
                    int Counter2 = 0;
                    foreach (Unit Player in PlayerUnits)
                    {
                        int[] Path = AI.Path(Player.TileNum, AI.TileNum, Tiles);
                        int Distance = Path.Length;
                        int TotalMoveCost = 0 - Tiles[AI.TileNum].GetMoveCost();
                        bool End = false;

                        foreach (int Move in Path)
                        {
                            if (Tiles[Move].GetMoveCost() <= 2)
                            {
                                TotalMoveCost = TotalMoveCost + Tiles[Move].GetMoveCost();
                            }
                            else if (Tiles[Move].GetMoveCost() == 99999 && Tiles[Move].PlayerPresent && !(Tiles[Move].GetFeature() == "Wheat" || Tiles[Move].GetFeature() == "Regular"))
                            {
                                TotalMoveCost = TotalMoveCost + 2;
                            }
                            else
                            {
                                TotalMoveCost = TotalMoveCost + 1;
                            }
                        }
                        while (!End)
                        {
                            int MC = 0;
                            int TileNum = 0;
                            for (int i = 0; i < Distance; i++)
                            {
                                if (Tiles[Path[i]].GetMoveCost() <= 2)
                                {
                                    MC = MC + Tiles[Path[i]].GetMoveCost();
                                }
                                else if (Tiles[Path[i]].GetMoveCost() == 99999 && Tiles[Path[i]].PlayerPresent && !(Tiles[Path[i]].GetFeature() == "Wheat" || Tiles[Path[i]].GetFeature() == "Regular"))
                                {
                                    MC = MC + 2;
                                }
                                else
                                {
                                    MC = MC + 1;
                                }
                                TileNum = Path[i];
                            }
                            MC = MC - Tiles[AI.TileNum].GetMoveCost();
                            if (MC <= AI.MP)
                            {
                                MoveCosts[(Counter * PlayerUnits.Length) + Counter2] = TotalMoveCost;
                                for (int A = 0; A < 7; A++)
                                {
                                    for (int I = 0; I < 7; I++)
                                    {
                                        if (Moves[(AI.ListNum * 49) + (A * 7) + I] == TileNum && (CSHT[(AI.ListNum * PlayerUnits.Length) + Player.ListNum] == -999))
                                        {
                                            MoveValues[(AI.ListNum * 49) + (A * 7) + I] = (MoveValues[(AI.ListNum * 49) + (A * 7) + I]) / ((TotalMoveCost * Weightings[11]) / 5);
                                        }
                                    }
                                }
                                End = true;
                            }
                            else
                            {
                                Distance--;
                            }
                        }
                        Counter2++;
                    }
                    Counter++;
                }
                




                int Iteration = 0;
                foreach (Unit AI in AIUnits)
                {
                    if (AI != null)
                    {
                        bool End = false;
                        int[] UsedArray = new int[0];
                        Counter = 0;
                        while (!End)
                        {
                            float LargestValue = -99999999;
                            int LargestIndex = -1;
                            bool Unused = true;
                            for (int I = 0; I < 49; I++)
                            {
                                Unused = true;
                                foreach (int Index in UsedArray)
                                {
                                    if (Index == ((AI.ListNum * 49) + I))
                                    {
                                        Unused = false;
                                    }
                                }
                                if ((MoveValues[(AI.ListNum * 49) + I] > LargestValue) && Unused)
                                {
                                    LargestValue = MoveValues[(AI.ListNum * 49) + I];
                                    LargestIndex = (AI.ListNum * 49) + I;
                                }
                            }
                            if (Moves[LargestIndex] != -1)
                            {
                                if (!Tiles[Moves[LargestIndex]].EnemyPresent && !Tiles[Moves[LargestIndex]].PlayerPresent && Moves[LargestIndex] != Bases[0].GetListNum() && Moves[LargestIndex] != Bases[1].GetListNum())
                                {
                                    Tiles[AI.TileNum].EnemyPresent = false;
                                    if (Tiles[AI.TileNum].GetFeature() == "Wheat" || Tiles[AI.TileNum].GetFeature() == "Regular")
                                    {
                                        Tiles[AI.TileNum].SetMoveCost(1);
                                    }
                                    else
                                    {
                                        Tiles[AI.TileNum].SetMoveCost(2);
                                    }
                                    AI.TileNum = Moves[LargestIndex];
                                    Tiles[Moves[LargestIndex]].EnemyPresent = true;
                                    Tiles[Moves[LargestIndex]].SetUnitNum(AI.ListNum);
                                    Tiles[Moves[LargestIndex]].SetMoveCost(99999);
                                    End = true;
                                }
                                else if (!Tiles[Moves[LargestIndex]].EnemyPresent && Moves[LargestIndex] != Bases[0].GetListNum() && Moves[LargestIndex] != Bases[1].GetListNum() && AI.UnitType != "Ranged" && PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()] != null)
                                {
                                    int[] Damage = AI.Combat(AI, PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()], Tiles, Bases);
                                    AI.Health = AI.Health - Damage[0];
                                    if (AI.Health <= 0)
                                    {
                                        if (Tiles[AI.TileNum].GetFeature() == "Wheat" || Tiles[AI.TileNum].GetFeature() == "Regular")
                                        {
                                            Tiles[AI.TileNum].SetMoveCost(1);
                                        }
                                        else
                                        {
                                            Tiles[AI.TileNum].SetMoveCost(2);
                                        }
                                        AIUnits[Iteration] = null;
                                    }
                                    PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].Health = PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].Health - Damage[1];
                                    if (PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].Health <= 0)
                                    {
                                        if (Tiles[PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].TileNum].GetFeature() == "Wheat" || Tiles[PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].TileNum].GetFeature() == "Regular")
                                        {
                                            Tiles[PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].TileNum].SetMoveCost(1);
                                        }
                                        else
                                        {
                                            Tiles[PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].TileNum].SetMoveCost(2);
                                        }
                                        PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()] = null;
                                    }
                                    AI.MP = 0;
                                    End = true;
                                }
                                else if (!Tiles[Moves[LargestIndex]].EnemyPresent && Moves[LargestIndex] != Bases[0].GetListNum() && Moves[LargestIndex] != Bases[1].GetListNum() && AI.UnitType == "Ranged" && AI.InRange(Tiles, AI.TileNum, PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].TileNum, 3, 0) && PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()] != null)
                                {
                                    int Damage = AI.RangedAttack(AI, PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()], Tiles);
                                    PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].Health = PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].Health - Damage;

                                    if (PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].Health <= 0)
                                    {
                                        if (Tiles[PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].TileNum].GetFeature() == "Wheat" || Tiles[PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].TileNum].GetFeature() == "Regular")
                                        {
                                            Tiles[PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].TileNum].SetMoveCost(1);
                                        }
                                        else
                                        {
                                            Tiles[PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()].TileNum].SetMoveCost(2);
                                        }
                                        PlayerUnits[Tiles[Moves[LargestIndex]].GetUnitNum()] = null;
                                    }
                                    AI.MP = 0;
                                    End = true;
                                }
                                else if (!Tiles[Moves[LargestIndex]].EnemyPresent && Moves[LargestIndex] != Bases[1].GetListNum() && AI.UnitType != "Ranged") 
                                {
                                    int[] Damage = AI.BaseCombat(AI, Bases[0]);
                                    AI.Health = AI.Health - Damage[0];
                                    if (AI.Health <= 0)
                                    {
                                        if (Tiles[AI.TileNum].GetFeature() == "Wheat" || Tiles[AI.TileNum].GetFeature() == "Regular")
                                        {
                                            Tiles[AI.TileNum].SetMoveCost(1);
                                        }
                                        else
                                        {
                                            Tiles[AI.TileNum].SetMoveCost(2);
                                        }
                                        AIUnits[Iteration] = null;
                                    }
                                    Bases[0].Health = Bases[0].Health - Damage[1];
                                    AI.MP = 0;
                                    End = true;
                                }
                                else if (!Tiles[Moves[LargestIndex]].EnemyPresent && Moves[LargestIndex] != Bases[1].GetListNum() && AI.UnitType == "Ranged" && AI.InRange(Tiles, AI.TileNum, Tiles[Moves[LargestIndex]].GetListNum(), 3, 0))   
                                {
                                    int Damage = AI.BaseRangedAttack(AI, Bases[0]);
                                    Bases[0].Health = Bases[0].Health - Damage;
                                    AI.MP = 0;
                                    End = true;
                                }
                                else
                                {
                                    Array.Resize(ref UsedArray, UsedArray.Length + 1);
                                    UsedArray[UsedArray.Length - 1] = LargestIndex;
                                }
                            }
                            else
                            {
                                Array.Resize(ref UsedArray, UsedArray.Length + 1);
                                UsedArray[UsedArray.Length - 1] = LargestIndex;
                            }
                            if (Counter == 48)
                            {
                                End = true;
                            }
                            Counter++;
                        }
                    }
                    Iteration++;
                }
                
                int AIStrength = 0;
                int PlayerStrength = 0;
                foreach (Unit AI in AIUnits)
                {
                    if (AI != null)
                    {
                        AIStrength = AIStrength + AI.CS + AI.Health;
                    }
                }
                foreach (Unit Player in PlayerUnits)
                {
                    if (Player != null)
                    {
                        PlayerStrength = PlayerStrength + Player.CS + Player.Health;
                    }
                }

                Unit[] OldUnits = AIUnits;
                AIUnits = Bases[1].AddProduction(AIUnits, Content.Load<Texture2D>("Warrior"), Content.Load<Texture2D>("Ranged"), Content.Load<Texture2D>("Spear"), Content.Load<Texture2D>("Cavalry"));
                if(AIUnits != OldUnits)
                {
                    Tiles[134].EnemyPresent = true;
                }

                if (Bases[1].GetChoice() == "Nothing")
                {
                    float[] ProductionValues = new float[10];

                    Counter = 0;
                    foreach (float Value in ProductionValues)
                    {
                        ProductionValues[Counter] = 0;
                        Counter++;
                    }

                    if (PlayerStrength >= AIStrength)
                    {
                        ProductionValues[0] = ProductionValues[0] + (5 * Weightings[12]);
                        ProductionValues[1] = ProductionValues[1] + (5 * Weightings[12]);
                        ProductionValues[2] = ProductionValues[2] + (5 * Weightings[12]);
                        ProductionValues[3] = ProductionValues[3] + (5 * Weightings[12]);
                    }
                    else
                    {
                        ProductionValues[0] = ProductionValues[0] - (5 * Weightings[13]);
                        ProductionValues[1] = ProductionValues[1] - (5 * Weightings[13]);
                        ProductionValues[2] = ProductionValues[2] - (5 * Weightings[13]);
                        ProductionValues[3] = ProductionValues[3] - (5 * Weightings[13]);
                    }
                    ProductionValues[4] = ProductionValues[4] - (Bases[1].GetFood() * Weightings[18]) + (Bases[1].GetPopulation() * Weightings[17]);
                    ProductionValues[5] = ProductionValues[5] + (TurnSinceProd * Weightings[15]);
                    ProductionValues[6] = ProductionValues[6] + ((PlayerStrength - AIStrength) * Weightings[16]) + (TurnSinceProd * Weightings[15]);

                    if (Bases[0].GetProduction() > (5 * Weightings[19]))
                    {
                        ProductionValues[8] = ProductionValues[8] + (AIUnits.Length * Weightings[20]) - (Bases[1].GetScience() * Weightings[21]);
                    }
                    else
                    {
                        ProductionValues[7] = ProductionValues[7] + (AIUnits.Length * Weightings[20]) - (Bases[1].GetScience() * Weightings[21]);
                    }

                    BCSHT = 0;
                    BCSHT = BCSHT - (Bases[1].Health * Weightings[6]) - (Bases[1].CS * Weightings[7]);
                    for (int I = 0; I < 5; I++)
                    {
                        for (int A = 0; A < 5; A++)
                        {
                            if (Tiles[(Bases[1].GetListNum() - 58) + I + (A * 28)].PlayerPresent)
                            {
                                BCSHT = BCSHT + (PlayerUnits[Tiles[(Bases[0].GetListNum() - 58) + I + (A * 28)].GetUnitNum()].CS * Weightings[8]) + (PlayerUnits[Tiles[(Bases[0].GetListNum() - 58) + I + (A * 28)].GetUnitNum()].Health * Weightings[9]);
                            }
                        }
                    }
                    ProductionValues[9] = ProductionValues[9] + (BCSHT * Weightings[14]);

                    float HighestValue = -9999;
                    int HighestIndex = -1;
                    for (int I = 0; I < 10; I++)
                    {
                        if (HighestValue < ProductionValues[I])
                        {
                            HighestIndex = I;
                            HighestValue = ProductionValues[I];
                        }
                    }
                    if (HighestIndex == 0)
                    {
                        Bases[1].BaseMenu("Spear");
                    }
                    else if (HighestIndex == 1)
                    {
                        Bases[1].BaseMenu("Warrior");
                    }
                    else if (HighestIndex == 2)
                    {
                        Bases[1].BaseMenu("Ranged");
                    }
                    else if (HighestIndex == 3)
                    {
                        Bases[1].BaseMenu("Cavalry");
                    }
                    else if (HighestIndex == 4)
                    {
                        Bases[1].BaseMenu("Granary");
                    }
                    else if (HighestIndex == 5)
                    {
                        Bases[1].BaseMenu("Blacksmith");
                    }
                    else if (HighestIndex == 6)
                    {
                        Bases[1].BaseMenu("Barracks");
                    }
                    else if (HighestIndex == 7)
                    {
                        Bases[1].BaseMenu("School");
                    }
                    else if (HighestIndex == 8)
                    {
                        Bases[1].BaseMenu("Uni");
                    }
                    else if (HighestIndex == 9)
                    {
                        Bases[1].BaseMenu("Outposts");
                    }
                    TurnSinceProd = 0;
                }

                Bases[1].NextFood();
                if (Bases[1].GetUnassigned())
                {
                    if (Bases[1].GetProduction() < Bases[1].GetFood() && Bases[1].GetProduction() < Bases[1].GetScience())
                    {
                        Bases[1].AssignWorker();
                    }
                    else if (Bases[1].GetFood() < Bases[1].GetProduction() && Bases[1].GetFood() < Bases[1].GetScience())
                    {
                        Bases[1].AssignFarmer();
                    }
                    else
                    {
                        Bases[1].AssignResearcher();
                    }
                }

                Bases[1].NextSci();
                if (Bases[1].GetCombatProgress() >= Bases[1].GetCombatCost())
                {
                    AIUnits = Bases[1].FinishTech(true, AIUnits);
                    Bases[1].SetTech("", true);
                }
                if (Bases[1].GetBaseProgress() >= Bases[1].GetBaseCost())
                {
                    AIUnits = Bases[1].FinishTech(false, AIUnits);
                    Bases[1].SetTech("", false);
                }

                if (Bases[1].GetBaseTech() == "")
                    Bases[1].BaseTechChoices();
                if (Bases[1].GetCombatTech() == "")
                    Bases[1].CombatTechChoices();

                float[] BaseTechValues = new float[7];
                float[] CombatTechValues = new float[11];
                int WarriorNum = 0;
                int SpearNum = 0;
                int RangedNum = 0;
                int CavalryNum = 0;

                foreach(Unit AI in AIUnits)
                {
                    if (AI != null)
                    {
                        if (AI.UnitType == "Warrior")
                        {
                            WarriorNum++;
                        }
                    }
                }

                foreach (Unit AI in AIUnits)
                {
                    if (AI != null)
                    {
                        if (AI.UnitType == "Spear")
                        {
                            SpearNum++;
                        }
                    }
                }

                foreach (Unit AI in AIUnits)
                {
                    if (AI != null)
                    {
                        if (AI.UnitType == "Ranged")
                        {
                            RangedNum++;
                        }
                    }
                }

                foreach (Unit AI in AIUnits)
                {
                    if (AI != null)
                    {
                        if (AI.UnitType == "Cavalry")
                        {
                            CavalryNum++;
                        }
                    }
                }

                CombatTechValues[0] = WarriorNum * Weightings[22];
                CombatTechValues[7] = WarriorNum * Weightings[23] + (((Bases[1].WarriorHealth * 1.25f) - Bases[1].WarriorHealth) * Weightings[24]);
                CombatTechValues[4] = WarriorNum * Weightings[23] + ((40) * Weightings[24]);

                CombatTechValues[1] = CavalryNum * Weightings[25];
                CombatTechValues[9] = CavalryNum * Weightings[26] + (((Bases[1].CavalryHealth * 1.25f) - Bases[1].CavalryHealth) * Weightings[27]);
                CombatTechValues[6] = CavalryNum * Weightings[26] + ((40) * Weightings[27]);

                CombatTechValues[2] = SpearNum * Weightings[28];
                CombatTechValues[8] = SpearNum * Weightings[29] + (((Bases[1].SpearHealth * 1.25f) - Bases[1].SpearHealth) * Weightings[30]);
                CombatTechValues[5] = SpearNum * Weightings[29] + ((40) * Weightings[30]);

                CombatTechValues[10] = (10 * Weightings[31]);
                CombatTechValues[3] = (RangedNum * Weightings[43]);

                BaseTechValues[0] = (Bases[1].GetWorker() * Weightings[32]);
                BaseTechValues[1] = (Bases[1].GetFarmer() * Weightings[33]);
                BaseTechValues[2] = (Bases[1].GetResearcher() * Weightings[34]);
                if(BCSHT < (200 * Weightings[35]))
                {
                    BaseTechValues[3] = 20 * Weightings[36];
                }
                BaseTechValues[4] = BaseTechValues[4] + ((PlayerStrength - AIStrength) * Weightings[37]) - (Bases[1].GetProduction() * Weightings[38]);
                BaseTechValues[5] = BaseTechValues[5] + ((20) * Weightings[39]) - (Bases[1].GetProduction() * Weightings[40]);
                if (BCSHT < (200 * Weightings[41]))
                {
                    BaseTechValues[6] = 20 * Weightings[42];
                }

                bool[] CombatChecked = new bool[11];
                Counter = 0;
                foreach(bool Check in CombatChecked)
                {
                    CombatChecked[Counter] = false;
                    Counter++;
                }
                bool[] BaseChecked = new bool[7];
                Counter = 0;
                foreach (bool Check in BaseChecked)
                {
                    BaseChecked[Counter] = false;
                    Counter++;
                }

                bool loop = true;
                float LargestValue2 = -9999;
                int LargestIndex2 = 0;
                if (Bases[1].GetCombatTech() != "")
                {
                    loop = false;
                }
                while (loop)
                {
                    LargestValue2 = -99999;
                    for (int I = 0; I < 11; I++)
                    {
                        if (LargestValue2 < CombatTechValues[I] && !CombatChecked[I])
                        {
                            LargestIndex2 = I;
                            LargestValue2 = CombatTechValues[I];
                        }
                    }
                    if(LargestIndex2 == 0 && (Bases[1].CombatChoices[0] == "CS for warriors" || Bases[1].CombatChoices[1] == "CS for warriors" || Bases[1].CombatChoices[2] == "CS for warriors") && !CombatChecked[0])
                    {
                        Bases[1].SetTech("CS for warriors", true);
                        loop = false;
                    }
                    else if(LargestIndex2 == 0)
                    {
                        CombatChecked[0] = true;
                    }
                    if (LargestIndex2 == 1 && (Bases[1].CombatChoices[0] == "CS for cavalry" || Bases[1].CombatChoices[1] == "CS for cavalry" || Bases[1].CombatChoices[2] == "CS for cavalry") && !CombatChecked[1])
                    {
                        Bases[1].SetTech("CS for cavalry", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 1)
                    {
                        CombatChecked[1] = true;
                    }
                    if (LargestIndex2 == 2 && (Bases[1].CombatChoices[0] == "CS for spearmen" || Bases[1].CombatChoices[1] == "CS for spearmen" || Bases[1].CombatChoices[2] == "CS for spearmen") && !CombatChecked[2])
                    {
                        Bases[1].SetTech("CS for spearmen", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 2)
                    {
                        CombatChecked[2] = true;
                    }
                    if (LargestIndex2 == 3 && (Bases[1].CombatChoices[0] == "RS for Ranged" || Bases[1].CombatChoices[1] == "RS for Ranged" || Bases[1].CombatChoices[2] == "RS for Ranged") && !CombatChecked[3])
                    {
                        Bases[1].SetTech("RS for Ranged", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 3)
                    {
                        CombatChecked[3] = true;
                    }
                    if (LargestIndex2 == 4 && (Bases[1].CombatChoices[0] == "Health for warriors (flat)" || Bases[1].CombatChoices[1] == "Health for warriors (flat)" || Bases[1].CombatChoices[2] == "Health for warriors (flat)") && !CombatChecked[4])
                    {
                        Bases[1].SetTech("Health for warriors (flat)", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 4)
                    {
                        CombatChecked[4] = true;
                    }
                    if (LargestIndex2 == 5 && (Bases[1].CombatChoices[0] == "Health for spearmen (flat)" || Bases[1].CombatChoices[1] == "Health for spearmen (flat)" || Bases[1].CombatChoices[2] == "Health for spearmen (flat)") && !CombatChecked[5])
                    {
                        Bases[1].SetTech("Health for spearmen (flat)", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 5)
                    {
                        CombatChecked[5] = true;
                    }
                    if (LargestIndex2 == 6 && (Bases[1].CombatChoices[0] == "Health for cavalry (flat)" || Bases[1].CombatChoices[1] == "Health for cavalry (flat)" || Bases[1].CombatChoices[2] == "Health for cavalry (flat)") && !CombatChecked[6])
                    {
                        Bases[1].SetTech("Health for cavalry (flat)", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 6)
                    {
                        CombatChecked[6] = true;
                    }
                    if (LargestIndex2 == 7 && (Bases[1].CombatChoices[0] == "Health for warriors (percentage)" || Bases[1].CombatChoices[1] == "Health for warriors (percentage)" || Bases[1].CombatChoices[2] == "Health for warriors (percentage)") && !CombatChecked[7])
                    {
                        Bases[1].SetTech("Health for warriors (percentage)", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 7)
                    {
                        CombatChecked[7] = true;
                    }
                    if (LargestIndex2 == 8 && (Bases[1].CombatChoices[0] == "Health for spearmen (percentage)" || Bases[1].CombatChoices[1] == "Health for spearmen (percentage)" || Bases[1].CombatChoices[2] == "Health for spearmen (percentage)") && !CombatChecked[8])
                    {
                        Bases[1].SetTech("Health for spearmen (percentage)", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 8)
                    {
                        CombatChecked[8] = true;
                    }
                    if (LargestIndex2 == 9 && (Bases[1].CombatChoices[0] == "Health for cavalry (percentage)" || Bases[1].CombatChoices[1] == "Health for cavalry (percentage)" || Bases[1].CombatChoices[2] == "Health for cavalry (percentage)") && !CombatChecked[9])
                    {
                        Bases[1].SetTech("CS for warriors", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 9)
                    {
                        CombatChecked[9] = true;
                    }
                    if (LargestIndex2 == 10 && (Bases[1].CombatChoices[0] == "Improved bonuses" || Bases[1].CombatChoices[1] == "Improved bonuses" || Bases[1].CombatChoices[2] == "Improved bonuses") && !CombatChecked[10])
                    {
                        Bases[1].SetTech("Improved bonuses", true);
                        loop = false;
                    }
                    else if (LargestIndex2 == 10)
                    {
                        CombatChecked[10] = true;
                    }
                }

                loop = true;
                LargestIndex2 = 0;
                LargestValue2 = -9999;
                if (Bases[1].GetBaseTech() != "")
                {
                    loop = false;
                }
                while (loop)
                {
                    LargestValue2 = -999999;
                    for (int I = 0; I < 7; I++)
                    {
                        if (LargestValue2 < BaseTechValues[I] && !BaseChecked[I])
                        {
                            LargestIndex2 = I;
                            LargestValue2 = BaseTechValues[I];
                        }
                    }
                    if (LargestIndex2 == 0 && (Bases[1].BaseChoices[0] == "Prod per worker" || Bases[1].BaseChoices[1] == "Prod per worker" || Bases[1].BaseChoices[2] == "Prod per worker") && !BaseChecked[0])
                    {
                        Bases[1].SetTech("Prod per worker", false);
                        loop = false;
                    }
                    else if (LargestIndex2 == 0)
                    {
                        BaseChecked[0] = true;
                    }
                    if (LargestIndex2 == 1 && (Bases[1].BaseChoices[0] == "Food per farmer" || Bases[1].BaseChoices[1] == "Food per farmer" || Bases[1].BaseChoices[2] == "Food per farmer") && !BaseChecked[1])
                    {
                        Bases[1].SetTech("Food per farmer", false);
                        loop = false;
                    }
                    else if (LargestIndex2 == 1)
                    {
                        BaseChecked[1] = true;
                    }
                    if (LargestIndex2 == 2 && (Bases[1].BaseChoices[0] == "Sci per scientist" || Bases[1].BaseChoices[1] == "Sci per scientist" || Bases[1].BaseChoices[2] == "Sci per scientist") && !BaseChecked[2])
                    {
                        Bases[1].SetTech("Sci per scientist", false);
                        loop = false;
                    }
                    else if (LargestIndex2 == 2)
                    {
                        BaseChecked[2] = true;
                    }
                    if (LargestIndex2 == 3 && (Bases[1].BaseChoices[0] == "Base CS" || Bases[1].BaseChoices[1] == "Base CS" || Bases[1].BaseChoices[2] == "Base CS") && !BaseChecked[3])
                    {
                        Bases[1].SetTech("Base CS", false);
                        loop = false;
                    }
                    else if (LargestIndex2 == 3)
                    {
                        BaseChecked[3] = true;
                    }
                    if (LargestIndex2 == 4 && (Bases[1].BaseChoices[0] == "Prod bonus for units" || Bases[1].BaseChoices[1] == "Prod bonus for units" || Bases[1].BaseChoices[2] == "Prod bonus for units") && !BaseChecked[4])
                    {
                        Bases[1].SetTech("Prod bonus for units", false);
                        loop = false;
                    }
                    else if (LargestIndex2 == 4)
                    {
                        BaseChecked[4] = true;
                    }
                    if (LargestIndex2 == 5 && (Bases[1].BaseChoices[0] == "Prod bonus for buildings" || Bases[1].BaseChoices[1] == "Prod bonus for buildings" || Bases[1].BaseChoices[2] == "Prod bonus for buildings") && !BaseChecked[5])
                    {
                        Bases[1].SetTech("Prod bonus for buildings", false);
                        loop = false;
                    }
                    else if (LargestIndex2 == 5)
                    {
                        BaseChecked[5] = true;
                    }
                    if (LargestIndex2 == 6 && (Bases[1].BaseChoices[0] == "Base support value" || Bases[1].BaseChoices[1] == "Base support value" || Bases[1].BaseChoices[2] == "Base support value") && !BaseChecked[6])
                    {
                        Bases[1].SetTech("Base support value", false);
                        loop = false;
                    }
                    else if (LargestIndex2 == 6)
                    {
                        BaseChecked[6] = true;
                    }
                }

                TurnSinceProd++;
                if (AIUnits.Length != 0)
                {
                    if (AIUnits[AIUnits.Length - 1] != null)
                    {
                        if (AIUnits[AIUnits.Length - 1].TileNum == 134)
                        {
                            Tiles[134].EnemyPresent = true;
                            Tiles[134].SetUnitNum(AIUnits.Length - 1);
                        }
                    }
                }

                OldUnits = PlayerUnits;
                PlayerUnits = Bases[0].AddProduction(PlayerUnits, Content.Load<Texture2D>("Warrior"), Content.Load<Texture2D>("Ranged"), Content.Load<Texture2D>("Spear"), Content.Load<Texture2D>("Cavalry"));
                if(PlayerUnits != OldUnits)
                {
                    Tiles[117].PlayerPresent = true;
                }

                UnitFinished = Bases[0].GetProductionFinished();

                foreach (Unit Player in PlayerUnits)
                {
                    if (Player != null)
                        Player.MP = Player.MaxMP;
                }
                foreach (Unit AI in AIUnits)
                {
                    if (AI != null)
                        AI.MP = AI.MaxMP;
                }
                Bases[0].NextFood();
                InitialJob = "";



                Bases[0].NextSci();


                if (Bases[0].GetCombatProgress() >= Bases[0].GetCombatCost())
                {
                    PlayerUnits = Bases[0].FinishTech(true, PlayerUnits);
                    Bases[0].SetTech("", true);
                }
                if (Bases[0].GetBaseProgress() >= Bases[0].GetBaseCost())
                {
                    AIUnits = Bases[0].FinishTech(false, AIUnits);
                    Bases[0].SetTech("", false);
                }



                if (Bases[0].GetBaseTech() == "")
                    Bases[0].BaseTechChoices();
                if (Bases[0].GetCombatTech() == "")
                    Bases[0].CombatTechChoices();


                unclick = false;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Space) && unclick && UnitFinished)
                ShowProdMessage = true;
            else if (Keyboard.GetState().IsKeyDown(Keys.Space) && unclick && Bases[0].GetUnassigned())
                ShowPopMessage = true;
            else if (Keyboard.GetState().IsKeyDown(Keys.Space) && unclick && (Bases[0].GetCombatTech() == "" || Bases[0].GetBaseTech() == ""))
                ShowSciMessage = true;

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
                if (Enemy != null)
                    Enemy.Draw(spriteBatch, Tiles);
            }

            foreach (Unit Player in PlayerUnits)
            {
                if(Player != null)
                    Player.Draw(spriteBatch, Tiles);
            }

            if (GameEnd && PlayerWin)
            {
                Cam.EndScreen(spriteBatch, Content.Load<Texture2D>("Win Screen"));
            }
            else if (GameEnd)
            {
                Cam.EndScreen(spriteBatch, Content.Load<Texture2D>("Lose Screen"));
            }

            if (!GameEnd)
            {
                Cam.TechBox(spriteBatch, TechBox, Content.Load<Texture2D>("Tech Box"));
            }
            Cam.Update(gameTime, Tiles);

            spriteBatch.End();


            if (!GameEnd)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null);
                

                spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Turn: " + Convert.ToString(TurnNum), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                if (BaseMenu)
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Base Menu Open", new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.1f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Production: " + Bases[0].GetProduction(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.17f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Total Production: " + Bases[0].GetTotalProduction(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.24f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Producing: " + Bases[0].GetChoice(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.31f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Food: " + Bases[0].GetFood(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.38f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Population: " + Bases[0].GetPopulation(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.45f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Population Cost: " + Bases[0].GetPopCost(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.52f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Science: " + Bases[0].GetScience(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.59f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Health: " + Bases[0].Health + "/" + Bases[0].MaxHealth, new Vector2(0.13f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.74f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "CS: " + Bases[0].CS, new Vector2(0.09f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.74f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Support: " + Bases[0].Support, new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.74f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);

                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Z: Spear, X: Warrior, C: Ranged, V: Cavalry, B: Granary, N: Blacksmith, M: Barracks, L: School, K: University, H: Outposts", new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.8f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    if (!ManagePop)
                        spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Press P to manage your population.", new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.82f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    if (ManagePop)
                    {
                        spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Workers: " + Bases[0].GetWorker() + ", Farmers: " + Bases[0].GetFarmer() + ", Researchers: " + Bases[0].GetResearcher(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.67f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                        spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Science Per Researcher = " + Bases[0].GetSciPerPop() + ", Food Per Farmer = " + Bases[0].GetFoodPerPop() + ", Production Per Worker = " + Bases[0].GetProdPerPop(), new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.69f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);


                        spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Press P again to stop managing your population.", new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.82f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    }
                }
                if (ShowProdMessage)
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "You must choose something to be produced", new Vector2(0.7f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.15f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                if (ShowPopMessage)
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "You must assign a new population to a job", new Vector2(0.7f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.15f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                if (ShowSciMessage)
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "You must choose both a combat and base technology to research", new Vector2(0.6f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.15f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "(you can do this by clicking on the tech box in the top right)", new Vector2(0.601f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.17f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                }

                if (TechMenu && Bases[0].GetCombatTech() != "")
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Current Combat Tech: " + Bases[0].GetCombatTech() + ", Science remaining: " + (Bases[0].GetCombatCost() - Bases[0].GetCombatProgress()), new Vector2(0.45f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.02f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                else if (TechMenu)
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Possible Combat Techs: " + Bases[0].CombatChoices[0] + " (" + Bases[0].ComChoice1Cost + ")" + ", " + Bases[0].CombatChoices[1] + " (" + Bases[0].ComChoice2Cost + ")" + " and " + Bases[0].CombatChoices[2] + " (" + Bases[0].ComChoice3Cost + ")", new Vector2(0.3f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.02f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);

                if (TechMenu && Bases[0].GetBaseTech() != "")
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Current Base Tech: " + Bases[0].GetBaseTech() + ", Science remaining: " + (Bases[0].GetBaseCost() - Bases[0].GetBaseProgress()), new Vector2(0.45f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                else if (TechMenu)
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Possible Base Techs: " + Bases[0].BaseChoices[0] + " (" + Bases[0].BaseChoice1Cost + ")" + ", " + Bases[0].BaseChoices[1] + " (" + Bases[0].BaseChoice2Cost + ")" + " and " + Bases[0].BaseChoices[2] + " (" + Bases[0].BaseChoice3Cost + ")", new Vector2(0.3f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                if (TechMenu && Bases[0].BaseFocus)
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Tech focus = Base", new Vector2(0.3f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.04f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Press T to change tech focus to combat", new Vector2(0.3f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.14f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                }
                else if (TechMenu)
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Tech focus = Combat", new Vector2(0.3f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.04f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Press T to change tech focus to base", new Vector2(0.3f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.14f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                }


                if (TechMenu && (Bases[0].GetBaseTech() == "" || Bases[0].GetCombatTech() == ""))
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Press 1 for the first base tech option, 2 for the second and 3 for the third. Press 4 for the first combat tech option, 5 for the second and 6 for", new Vector2(0.3f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.1f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "the third.", new Vector2(0.301f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.12f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                }


                foreach (Unit Troop in PlayerUnits)
                {
                    if (Troop != null)
                    {
                        if (Troop.Selected)
                        {
                            spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Health: " + Troop.Health + "/" + Troop.MaxHealth, new Vector2(0.85f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.85f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                            spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Movement: " + Troop.MP + "/" + Troop.MaxMP, new Vector2(0.775f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.85f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                            spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Combat Strength: " + Troop.CS, new Vector2(0.67f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.85f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                        }
                        if (Troop.Selected && Troop.UnitType == "Ranged")
                        {
                            spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "RS: " + Troop.RS, new Vector2(0.6275f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.85f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                        }
                    }
                }
                foreach (Unit Troop in AIUnits)
                {
                    if (Troop != null)
                    {
                        if (Troop.Selected)
                        {
                            spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Health: " + Troop.Health + "/" + Troop.MaxHealth, new Vector2(0.85f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.85f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                            spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Movement: " + Troop.MaxMP, new Vector2(0.775f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.85f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                            spriteBatch.DrawString(Content.Load<SpriteFont>("Menu Font"), "Combat Strength: " + Troop.CS, new Vector2(0.67f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.85f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
                        }
                    }
                }

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
