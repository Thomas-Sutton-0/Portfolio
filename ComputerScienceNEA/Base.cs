using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerScienceNEA
{
    class Base
    {
        private bool Player;
        private int Row;
        private int Position;

        private int Production = 5;
        private int Food = 2;
        private int Population = 1;
        private int ProdPop = 0;
        private int FoodPop = 0;
        private int SciPop = 0;
        private double UnitProduction = 0;
        private float Science = 1;

        private int ProdPerPop = 3;
        private int SciPerPop = 1;
        private int FoodPerPop = 2;
        private int PopCost = 10;
        private int TotalFood = 0;

        private int WarriorCost;
        private int RangedCost;
        private int SpearCost;
        private int CavalryCost;
        private int UnitCost;

        private int MP = 3;
        private int WarriorCS = 50;
        private int SpearCS = 50;
        private int CavalryCS = 50;
        private int RangedRS = 40;
        public int WarriorHealth = 100;
        public int SpearHealth = 100;
        public int CavalryHealth = 100;
        public int Bonus = 20;

        private int GranaryCost = 20;
        private int BlacksmithCost = 20;
        private int BarracksCost = 20;
        private int SchoolCost = 20;
        private int UniCost = 20;
        private int WallCost = 20;
        private int OutpostCost = 20;

        public int Health = 100;
        public int MaxHealth = 100;
        public int CS = 80;
        public int Support = 0; //This is the extra CS all nearby units will get from outposts 
        private int TileListNum;
        private double ExtraProduction;
        private double TotalProduction;
        private double UnitProdBonus = 1;
        private double BuildProdBonus = 1;
        private string ProduceChoice;
        private bool Unassigned = true;
        private bool UnitProd;
        private bool ProductionFinished;
        public bool BaseFocus = true;
        private Texture2D BaseTexture;
        private Vector2 Origin;
        private Vector2 VisualPosition;

        public string[] BaseChoices = new string[3];
        public string[] CombatChoices = new string[3];
        private string BaseTechChoice = ""; //Techs: Increase prod per pop, increase food per pop, increase sci per pop, increase CS of base, decrease prod cost of units, decrease prod cost of buildings, increase support value 
        private string CombatTechChoice = ""; //Techs: Increase CS and health for each unit (but no health increase to ranged and no range increase as replacement as that would be OP), increase movement points of all units, increase the advantage that the bonus gives against other units (eg spears do particularly better against cavalry) 
        private double BaseCost;
        private double CombatCost;
        private double BaseProgress;
        private double CombatProgress;
        public double ComChoice1Cost;
        public double ComChoice2Cost;
        public double ComChoice3Cost;
        public double BaseChoice1Cost;
        public double BaseChoice2Cost;
        public double BaseChoice3Cost;
        private double[] CombatCosts = new double[11]; //Used to store the cost of each combat tech
        private double[] BaseCosts = new double[7]; //Used to store the cost of each base tech


        public Base(bool NewPlayer, int NewListNum, Texture2D NewTexture)
        {
            Player = NewPlayer;
            Row = 5;
            if (Player == true)
                Position = 4;
            if (Player == false)
                Position = 25;
            BaseTexture = NewTexture;
            Origin = new Vector2(BaseTexture.Width / 2, BaseTexture.Height / 2);
            VisualPosition = new Vector2(Position * 200, Row * 200);
            CavalryCost = 20;
            SpearCost = 20;
            RangedCost = 20;
            WarriorCost = 20;
            TileListNum = NewListNum;
            if (Player == true)
                ProduceChoice = "Nothing";
            if (Player == false)
                ProduceChoice = "Warrior";
            UnitCost = WarriorCost;
            ProductionFinished = false;
            for (int i = 0; i < CombatCosts.Length; i++)
            {
                CombatCosts[i] = 20;
            }
            for (int i = 0; i < BaseCosts.Length; i++)
            {
                BaseCosts[i] = 20;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BaseTexture, VisualPosition, null, Color.LightGreen, 0f, Origin, 1, SpriteEffects.None, 0);
        }
        public void BaseMenu(string NextProduce)
        {
            ProduceChoice = NextProduce;
            if (!ProductionFinished)
                ExtraProduction = TotalProduction;
            TotalProduction = ExtraProduction;
            ProductionFinished = false;
        }
        public Unit[] AddProduction(Unit[] PlayerUnits, Texture2D WarriorTex, Texture2D RangedTex, Texture2D SpearTex, Texture2D CavalryTex)
        {
            if (UnitProd)
                TotalProduction = TotalProduction + (Production + UnitProduction) * UnitProdBonus;

            else
                TotalProduction = TotalProduction + Production * BuildProdBonus;

            if (TotalProduction >= SpearCost && ProduceChoice == "Spear")
            {
                Array.Resize(ref PlayerUnits, PlayerUnits.Length + 1);
                if (Player)
                {
                    PlayerUnits[PlayerUnits.Length - 1] = new Spear(true, SpearHealth, SpearHealth, SpearCS, 117, SpearTex, MP, PlayerUnits.Length - 1);
                }
                else
                {
                    PlayerUnits[PlayerUnits.Length - 1] = new Spear(false, SpearHealth, SpearHealth, SpearCS, 134, SpearTex, MP, PlayerUnits.Length - 1);
                }
                ExtraProduction = TotalProduction - SpearCost;
                ProductionFinished = true;
                UnitProd = true;
                SpearCost = Convert.ToInt32(SpearCost * 1.25);
            }

            if (TotalProduction >= WarriorCost && ProduceChoice == "Warrior")
            {
                Array.Resize(ref PlayerUnits, PlayerUnits.Length + 1);
                if (Player)
                {
                    PlayerUnits[PlayerUnits.Length - 1] = new Warrior(true, WarriorHealth, WarriorHealth, WarriorCS, 117, WarriorTex, MP, PlayerUnits.Length - 1);
                }
                else
                {
                    PlayerUnits[PlayerUnits.Length - 1] = new Warrior(false, WarriorHealth, WarriorHealth, WarriorCS, 134, WarriorTex, MP, PlayerUnits.Length - 1);
                }
                ExtraProduction = TotalProduction - WarriorCost;
                ProductionFinished = true;
                UnitProd = true;
                WarriorCost = Convert.ToInt32(WarriorCost * 1.25);
            }

            if (TotalProduction >= RangedCost && ProduceChoice == "Ranged")
            {
                Array.Resize(ref PlayerUnits, PlayerUnits.Length + 1);
                if (Player)
                {
                    PlayerUnits[PlayerUnits.Length - 1] = new Ranged(true, 50, 50, 25, 117, RangedTex, MP, RangedRS, PlayerUnits.Length - 1);
                }
                else
                {
                    PlayerUnits[PlayerUnits.Length - 1] = new Ranged(false, 50, 50, 25, 134, RangedTex, MP, RangedRS, PlayerUnits.Length - 1);
                }
                ExtraProduction = TotalProduction - RangedCost;
                ProductionFinished = true;
                UnitProd = true;
                RangedCost = Convert.ToInt32(RangedCost * 1.25);
            }

            if (TotalProduction >= CavalryCost && ProduceChoice == "Cavalry")
            {
                Array.Resize(ref PlayerUnits, PlayerUnits.Length + 1);
                if (Player)
                {
                    PlayerUnits[PlayerUnits.Length - 1] = new Cavalry(true, CavalryHealth, CavalryHealth, CavalryCS, 117, CavalryTex, MP, PlayerUnits.Length - 1);
                }
                else
                {
                    PlayerUnits[PlayerUnits.Length - 1] = new Cavalry(false, CavalryHealth, CavalryHealth, CavalryCS, 134, CavalryTex, MP, PlayerUnits.Length - 1);
                }
                ExtraProduction = TotalProduction - CavalryCost;
                ProductionFinished = true;
                UnitProd = true;
                CavalryCost = Convert.ToInt32(CavalryCost * 1.25);
            }

            if (TotalProduction >= GranaryCost && ProduceChoice == "Granary")
            {
                Food = Food + 4;
                ExtraProduction = TotalProduction - GranaryCost;
                ProductionFinished = true;
                UnitProd = false;
                GranaryCost = Convert.ToInt32(GranaryCost * 1.25);
            }

            if (TotalProduction >= BlacksmithCost && ProduceChoice == "Blacksmith")
            {
                Production = Production + 3;
                ExtraProduction = TotalProduction - BlacksmithCost;
                ProductionFinished = true;
                UnitProd = false;
                BlacksmithCost = Convert.ToInt32(BlacksmithCost * 1.25);
            }


            if (TotalProduction >= BarracksCost && ProduceChoice == "Barracks")
            {
                UnitProduction = UnitProduction + 6;
                ExtraProduction = TotalProduction - BarracksCost;
                ProductionFinished = true;
                UnitProd = false;
                BarracksCost = Convert.ToInt32(BarracksCost * 1.25);
            }

            if (TotalProduction >= SchoolCost && ProduceChoice == "School")
            {
                Science = Science + 2;
                ExtraProduction = TotalProduction - SchoolCost;
                ProductionFinished = true;
                UnitProd = false;
                SchoolCost = Convert.ToInt32(SchoolCost * 1.25);
            }

            if (TotalProduction >= UniCost && ProduceChoice == "University")
            {
                Science = Science + 6;
                ExtraProduction = TotalProduction - UniCost;
                ProductionFinished = true;
                UnitProd = false;
                UniCost = Convert.ToInt32(UniCost * 1.25);
            }

            if (TotalProduction >= WallCost && ProduceChoice == "Walls")
            {
                Health = Health + 40;
                CS = CS + 10;
                ExtraProduction = TotalProduction - WallCost;
                ProductionFinished = true;
                UnitProd = false;
                WallCost = Convert.ToInt32(WallCost * 1.25);
            }

            if (TotalProduction >= OutpostCost && ProduceChoice == "Outpost")
            {
                Support = Support + 10;
                ExtraProduction = TotalProduction - OutpostCost;
                ProductionFinished = true;
                UnitProd = false;
                OutpostCost = Convert.ToInt32(OutpostCost * 1.25);
            }

            if (ProductionFinished)
                ProduceChoice = "Nothing";

            return PlayerUnits;
        }
        public void MovePop(string InitialJob, string NewJob)
        {
            bool Cancel = false;
            if (InitialJob == "Worker" && ProdPop > 0)
            {
                ProdPop--;
                Production = Production - ProdPerPop;
            }
            else if (InitialJob == "Farmer" && FoodPop > 0)
            {
                FoodPop--;
                Food = Food - FoodPerPop;
            }
            else if (InitialJob == "Researcher" && SciPop > 0)
            {
                SciPop--;
                Science = Science - SciPerPop;
            }
            else
            {
                Cancel = true;
            }

            if (NewJob == "Worker" && !Cancel)
            {
                ProdPop++;
                Production = Production + ProdPerPop;
            }
            else if (NewJob == "Farmer" && !Cancel)
            {
                FoodPop++;
                Food = Food + FoodPerPop;
            }
            else if (NewJob == "Researcher" && !Cancel)
            {
                SciPop++;
                Science = Science + SciPerPop;
            }
        }
        public void NextFood()
        {
            TotalFood = TotalFood + Food;
            if (TotalFood >= PopCost)
            {
                Population++;
                TotalFood = TotalFood - PopCost;
                PopCost = Convert.ToInt32(PopCost * 1.2);
                Unassigned = true;
            }
        }
        public void NextSci()
        {
            if (BaseFocus)
            {
                BaseProgress = BaseProgress + (Science * 1.25);
                CombatProgress = CombatProgress + Science;
            }
            else
            {
                BaseProgress = BaseProgress + Science;
                CombatProgress = CombatProgress + (Science * 1.25);
            }

        }
        public void BaseTechChoices()
        {
            Random random = new Random();
            int[] RandNums = new int[3];
            int iteration = 0;

            RandNums[0] = random.Next(1, 8);
            RandNums[1] = random.Next(1, 8);
            while (RandNums[1] == RandNums[0])
                RandNums[1] = random.Next(1, 8);
            RandNums[2] = random.Next(1, 8);
            while (RandNums[2] == RandNums[0] || RandNums[2] == RandNums[1])
                RandNums[2] = random.Next(1, 8);


            if (RandNums[0] == 1)
            {
                BaseChoices[iteration] = "Prod per worker";
                BaseChoice1Cost = BaseCosts[0];
            }

            else if (RandNums[0] == 2)
            {
                BaseChoices[iteration] = "Food per farmer";
                BaseChoice1Cost = BaseCosts[1];
            }

            else if (RandNums[0] == 3)
            {
                BaseChoices[iteration] = "Sci per scientist";
                BaseChoice1Cost = BaseCosts[2];
            }

            else if (RandNums[0] == 4)
            {
                BaseChoices[iteration] = "Base CS";
                BaseChoice1Cost = BaseCosts[3];
            }

            else if (RandNums[0] == 5)
            {
                BaseChoices[iteration] = "Prod bonus for units";
                BaseChoice1Cost = BaseCosts[4];
            }

            else if (RandNums[0] == 6)
            {
                BaseChoices[iteration] = "Prod bonus for buildings";
                BaseChoice1Cost = BaseCosts[5];
            }

            else if (RandNums[0] == 7)
            {
                BaseChoices[iteration] = "Base support value"; //Increased combat strength for units near the base (useful for defence if enemy is winning, keep this in mind for AI)
                BaseChoice1Cost = BaseCosts[6];
            }
            iteration++;


            if (RandNums[1] == 1)
            {
                BaseChoices[iteration] = "Prod per worker";
                BaseChoice2Cost = BaseCosts[0];
            }

            else if (RandNums[1] == 2)
            {
                BaseChoices[iteration] = "Food per farmer";
                BaseChoice2Cost = BaseCosts[1];
            }

            else if (RandNums[1] == 3)
            {
                BaseChoices[iteration] = "Sci per scientist";
                BaseChoice2Cost = BaseCosts[2];
            }

            else if (RandNums[1] == 4)
            {
                BaseChoices[iteration] = "Base CS";
                BaseChoice2Cost = BaseCosts[3];
            }

            else if (RandNums[1] == 5)
            {
                BaseChoices[iteration] = "Prod bonus for units";
                BaseChoice2Cost = BaseCosts[4];
            }

            else if (RandNums[1] == 6)
            {
                BaseChoices[iteration] = "Prod bonus for buildings";
                BaseChoice2Cost = BaseCosts[5];
            }

            else if (RandNums[1] == 7)
            {
                BaseChoices[iteration] = "Base support value"; //Increased combat strength for units near the base (useful for defence if enemy is winning, keep this in mind for AI)
                BaseChoice2Cost = BaseCosts[6];
            }
            iteration++;


            if (RandNums[2] == 1)
            {
                BaseChoices[iteration] = "Prod per worker";
                BaseChoice3Cost = BaseCosts[0];
            }

            else if (RandNums[2] == 2)
            {
                BaseChoices[iteration] = "Food per farmer";
                BaseChoice3Cost = BaseCosts[1];
            }

            else if (RandNums[2] == 3)
            {
                BaseChoices[iteration] = "Sci per scientist";
                BaseChoice3Cost = BaseCosts[2];
            }

            else if (RandNums[2] == 4)
            {
                BaseChoices[iteration] = "Base CS";
                BaseChoice3Cost = BaseCosts[3];
            }

            else if (RandNums[2] == 5)
            {
                BaseChoices[iteration] = "Prod bonus for units";
                BaseChoice3Cost = BaseCosts[4];
            }

            else if (RandNums[2] == 6)
            {
                BaseChoices[iteration] = "Prod bonus for buildings";
                BaseChoice3Cost = BaseCosts[5];
            }

            else if (RandNums[2] == 7)
            {
                BaseChoices[iteration] = "Base support value"; //Increased combat strength for units near the base (useful for defence if enemy is winning, keep this in mind for AI)
                BaseChoice3Cost = BaseCosts[6];
            }

        }

        public void CombatTechChoices()
        {
            Random random = new Random();
            int[] RandNums = new int[3];
            int iteration = 0;
            bool End = false;

            RandNums[0] = random.Next(1, 111);
            if (RandNums[0] <= 10)
            {
                CombatChoices[iteration] = "CS for warriors";
                ComChoice1Cost = CombatCosts[0];
            }

            else if (RandNums[0] <= 20)
            {
                CombatChoices[iteration] = "CS for cavalry";
                ComChoice1Cost = CombatCosts[1];
            }

            else if (RandNums[0] <= 30)
            {
                CombatChoices[iteration] = "CS for spearmen";
                ComChoice1Cost = CombatCosts[2];
            }

            else if (RandNums[0] <= 40)
            {
                CombatChoices[iteration] = "RS for Ranged";
                ComChoice1Cost = CombatCosts[3];
            }

            else if (RandNums[0] <= 50)
            {
                CombatChoices[iteration] = "Health for warriors (flat)";
                ComChoice1Cost = CombatCosts[4];
            }

            else if (RandNums[0] <= 60)
            {
                CombatChoices[iteration] = "Health for spearmen (flat)";
                ComChoice1Cost = CombatCosts[5];
            }

            else if (RandNums[0] <= 70)
            {
                CombatChoices[iteration] = "Health for cavalry (flat)";
                ComChoice1Cost = CombatCosts[6];
            }

            else if (RandNums[0] <= 80)
            {
                CombatChoices[iteration] = "Health for warriors (percentage)";
                ComChoice1Cost = CombatCosts[7];
            }

            else if (RandNums[0] <= 90)
            {
                CombatChoices[iteration] = "Health for spearmen (percentage)";
                ComChoice1Cost = CombatCosts[8];
            }

            else if (RandNums[0] <= 100)
            {
                CombatChoices[iteration] = "Health for cavalry (percentage)";
                ComChoice1Cost = CombatCosts[9];
            }

            else if (RandNums[0] <= 110)
            {
                CombatChoices[iteration] = "Improved bonuses";
                ComChoice1Cost = CombatCosts[10];
            }

            iteration++;
            End = false;
            RandNums[1] = random.Next(1, 111);
            while (!End)
            {
                if (RandNums[1] <= 10)
                {
                    CombatChoices[iteration] = "CS for warriors";
                    ComChoice2Cost = CombatCosts[0];
                }

                else if (RandNums[1] <= 20)
                {
                    CombatChoices[iteration] = "CS for cavalry";
                    ComChoice2Cost = CombatCosts[1];
                }

                else if (RandNums[1] <= 30)
                {
                    CombatChoices[iteration] = "CS for spearmen";
                    ComChoice2Cost = CombatCosts[2];
                }

                else if (RandNums[1] <= 40)
                {
                    CombatChoices[iteration] = "RS for Ranged";
                    ComChoice2Cost = CombatCosts[3];
                }

                else if (RandNums[1] <= 50)
                {
                    CombatChoices[iteration] = "Health for warriors (flat)";
                    ComChoice2Cost = CombatCosts[4];
                }

                else if (RandNums[1] <= 60)
                {
                    CombatChoices[iteration] = "Health for spearmen (flat)";
                    ComChoice2Cost = CombatCosts[5];
                }

                else if (RandNums[1] <= 70)
                {
                    CombatChoices[iteration] = "Health for cavalry (flat)";
                    ComChoice2Cost = CombatCosts[6];
                }

                else if (RandNums[1] <= 80)
                {
                    CombatChoices[iteration] = "Health for warriors (percentage)";
                    ComChoice2Cost = CombatCosts[7];
                }

                else if (RandNums[1] <= 90)
                {
                    CombatChoices[iteration] = "Health for spearmen (percentage)";
                    ComChoice2Cost = CombatCosts[8];
                }

                else if (RandNums[1] <= 100)
                {
                    CombatChoices[iteration] = "Health for cavalry (percentage)";
                    ComChoice2Cost = CombatCosts[9];
                }

                else if (RandNums[1] <= 110)
                {
                    CombatChoices[iteration] = "Improved bonuses";
                    ComChoice2Cost = CombatCosts[10];
                }

                if (CombatChoices[1] != CombatChoices[0])
                    End = true;
                else
                    RandNums[1] = random.Next(1, 116);
            }

            iteration++;
            End = false;
            RandNums[2] = random.Next(1, 111);
            while (!End)
            {
                if (RandNums[2] <= 10)
                {
                    CombatChoices[iteration] = "CS for warriors";
                    ComChoice3Cost = CombatCosts[0];
                }

                else if (RandNums[2] <= 20)
                {
                    CombatChoices[iteration] = "CS for cavalry";
                    ComChoice3Cost = CombatCosts[1];
                }

                else if (RandNums[2] <= 30)
                {
                    CombatChoices[iteration] = "CS for spearmen";
                    ComChoice3Cost = CombatCosts[2];
                }

                else if (RandNums[2] <= 40)
                {
                    CombatChoices[iteration] = "RS for Ranged";
                    ComChoice3Cost = CombatCosts[3];
                }

                else if (RandNums[2] <= 50)
                {
                    CombatChoices[iteration] = "Health for warriors (flat)";
                    ComChoice3Cost = CombatCosts[4];
                }

                else if (RandNums[2] <= 60)
                {
                    CombatChoices[iteration] = "Health for spearmen (flat)";
                    ComChoice3Cost = CombatCosts[5];
                }

                else if (RandNums[2] <= 70)
                {
                    CombatChoices[iteration] = "Health for cavalry (flat)";
                    ComChoice3Cost = CombatCosts[6];
                }

                else if (RandNums[2] <= 80)
                {
                    CombatChoices[iteration] = "Health for warriors (percentage)";
                    ComChoice3Cost = CombatCosts[7];
                }

                else if (RandNums[2] <= 90)
                {
                    CombatChoices[iteration] = "Health for spearmen (percentage)";
                    ComChoice3Cost = CombatCosts[8];
                }

                else if (RandNums[2] <= 100)
                {
                    CombatChoices[iteration] = "Health for cavalry (percentage)";
                    ComChoice3Cost = CombatCosts[9];
                }

                else if (RandNums[2] <= 110)
                {
                    CombatChoices[iteration] = "Improved bonuses";
                    ComChoice3Cost = CombatCosts[10];
                }

                if ((CombatChoices[2] != CombatChoices[0]) && (CombatChoices[2] != CombatChoices[1]))
                    End = true;
                else
                    RandNums[2] = random.Next(1, 116);
            }

        }
        public void SetTech(string Choice, bool Combat)
        {
            if (Combat)
            {
                CombatTechChoice = Choice;

                if (CombatTechChoice == "CS for warriors")
                {
                    CombatCost = CombatCosts[0];
                }
                if (CombatTechChoice == "CS for cavalry")
                {
                    CombatCost = CombatCosts[1];
                }
                if (CombatTechChoice == "CS for spearmen")
                {
                    CombatCost = CombatCosts[2];
                }
                if (CombatTechChoice == "RS for Ranged")
                {
                    CombatCost = CombatCosts[3];
                }
                if (CombatTechChoice == "Health for warriors (flat)")
                {
                    CombatCost = CombatCosts[4];
                }
                if (CombatTechChoice == "Health for spearmen (flat)")
                {
                    CombatCost = CombatCosts[5];
                }
                if (CombatTechChoice == "Health for cavalry (flat)")
                {
                    CombatCost = CombatCosts[6];
                }
                if (CombatTechChoice == "Health for warriors (percentage)")
                {
                    CombatCost = CombatCosts[7];
                }
                if (CombatTechChoice == "Health for spearmen (percentage)")
                {
                    CombatCost = CombatCosts[8];
                }
                if (CombatTechChoice == "Health for cavalry (percentage)")
                {
                    CombatCost = CombatCosts[9];
                }
                if (CombatTechChoice == "Improved bonuses")
                {
                    CombatCost = CombatCosts[10];
                }
            }
            else
            {
                BaseTechChoice = Choice;

                if (BaseTechChoice == "Prod per worker")
                {
                    BaseCost = BaseCosts[0];
                }
                if (BaseTechChoice == "Food per farmer")
                {
                    BaseCost = BaseCosts[1];
                }
                if (BaseTechChoice == "Sci per scientist")
                {
                    BaseCost = BaseCosts[2];
                }
                if (BaseTechChoice == "Base CS")
                {
                    BaseCost = BaseCosts[3];
                }
                if (BaseTechChoice == "Prod bonus for units")
                {
                    BaseCost = BaseCosts[4];
                }
                if (BaseTechChoice == "Prod bonus for buildings")
                {
                    BaseCost = BaseCosts[5];
                }
                if (BaseTechChoice == "Base support value")
                {
                    BaseCost = BaseCosts[6];
                }
            }
        }
        public Unit[] FinishTech(bool Combat, Unit[] Units)
        {
            if (!Combat && BaseTechChoice == "Prod per worker")
            {
                BaseCosts[0] = BaseCosts[0] * 1.5;
                ProdPerPop++;
                Production = Production + ProdPop;
            }
            if (!Combat && BaseTechChoice == "Food per farmer")
            {
                BaseCosts[1] = BaseCosts[1] * 1.5;
                FoodPerPop++;
                Food = Food + FoodPop;
            }
            if (!Combat && BaseTechChoice == "Sci per scientist")
            {
                BaseCosts[2] = BaseCosts[2] * 1.5;
                SciPerPop++;
                Science = Science + SciPop;
            }
            if (!Combat && BaseTechChoice == "Base CS")
            {
                BaseCosts[3] = BaseCosts[3] * 1.5;
                CS = CS + 25;
            }
            if (!Combat && BaseTechChoice == "Prod bonus for units")
            {
                BaseCosts[4] = BaseCosts[4] * 1.5;
                UnitProdBonus = UnitProdBonus + 0.15;
            }
            if (!Combat && BaseTechChoice == "Prod bonus for buildings")
            {
                BaseCosts[5] = BaseCosts[5] * 1.5;
                UnitProdBonus = BuildProdBonus + 0.15;
            }
            if (!Combat && BaseTechChoice == "Base support value")
            {
                BaseCosts[6] = BaseCosts[6] * 1.5;
                Support = Support + 20;
            }

            if (Combat && CombatTechChoice == "CS for warriors")
            {
                CombatCosts[0] = CombatCosts[0] * 1.5;
                WarriorCS = WarriorCS + 10;
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Warrior")
                        {
                            unit.CS = unit.CS + 10;
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "CS for cavalry")
            {
                CombatCosts[1] = CombatCosts[1] * 1.5;
                CavalryCS = CavalryCS + 10;
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Cavalry")
                        {
                            unit.CS = unit.CS + 10;
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "CS for spearmen")
            {
                CombatCosts[2] = CombatCosts[2] * 1.5;
                SpearCS = SpearCS + 10;
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Spear")
                        {
                            unit.CS = unit.CS + 10;
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "RS for Ranged")
            {
                CombatCosts[3] = CombatCosts[3] * 1.5;
                RangedRS = RangedRS + 15;
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Ranged")
                        {
                            unit.RS = unit.RS + 15;
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "Health for warriors (flat)")
            {
                CombatCosts[4] = CombatCosts[4] * 1.5;
                WarriorHealth = WarriorHealth + 40;
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Warrior")
                        {
                            unit.MaxHealth = unit.MaxHealth + 40;
                            unit.Health = unit.Health + 40;
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "Health for spearmen (flat)")
            {
                CombatCosts[5] = CombatCosts[5] * 1.5;
                SpearHealth = SpearHealth + 40;
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Spear")
                        {
                            unit.MaxHealth = unit.MaxHealth + 40;
                            unit.Health = unit.Health + 40;
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "Health for cavalry (flat)")
            {
                CombatCosts[6] = CombatCosts[6] * 1.5;
                CavalryHealth = CavalryHealth + 40;
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Cavalry")
                        {
                            unit.MaxHealth = unit.MaxHealth + 40;
                            unit.Health = unit.Health + 40;
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "Health for warriors (percentage)")
            {
                CombatCosts[7] = CombatCosts[7] * 1.5;
                WarriorHealth = Convert.ToInt32(WarriorHealth * 1.25);
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Warrior")
                        {
                            unit.MaxHealth = Convert.ToInt32(unit.MaxHealth * 1.25);
                            unit.Health = Convert.ToInt32(unit.Health * 1.25);
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "Health for spearmen (percentage)")
            {
                CombatCosts[8] = CombatCosts[8] * 1.5;
                SpearHealth = Convert.ToInt32(SpearHealth * 1.25);
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Spear")
                        {
                            unit.MaxHealth = Convert.ToInt32(unit.MaxHealth * 1.25);
                            unit.Health = Convert.ToInt32(unit.Health * 1.25);
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "Health for cavalry (percentage)")
            {
                CombatCosts[9] = CombatCosts[9] * 1.5;
                CavalryHealth = Convert.ToInt32(CavalryHealth * 1.25);
                foreach (Unit unit in Units)
                {
                    if (unit != null)
                    {
                        if (unit.UnitType == "Cavalry")
                        {
                            unit.MaxHealth = Convert.ToInt32(unit.MaxHealth * 1.25);
                            unit.Health = Convert.ToInt32(unit.Health * 1.25);
                        }
                    }
                }
            }
            if (Combat && CombatTechChoice == "Improved bonuses")
            {
                CombatCosts[10] = CombatCosts[10] * 1.5;
                Bonus = Bonus + 20;
            }

            if (Combat)
                CombatProgress = 0;
            else
                BaseProgress = 0;

            return Units;
        }
        public string GetChoice()
        {
            return ProduceChoice;
        }
        public int GetUnitCost()
        {
            return UnitCost;
        }
        public int GetProduction()
        {
            return Production;
        }
        public int GetPopulation()
        {
            return Population;
        }
        public int GetFood()
        {
            return Food;
        }
        public float GetScience()
        {
            return Science;
        }
        public int GetListNum()
        {
            return TileListNum;
        }
        public int GetPopCost()
        {
            return PopCost;
        }
        public double GetTotalProduction()
        {
            return TotalProduction;
        }
        public bool GetProductionFinished()
        {
            return ProductionFinished;
        }
        public bool GetUnit()
        {
            return UnitProd;
        }
        public double GetUnitProduction()
        {
            return UnitProduction;
        }
        public int GetResearcher()
        {
            return SciPop;
        }
        public int GetFarmer()
        {
            return FoodPop;
        }
        public int GetWorker()
        {
            return ProdPop;
        }
        public int GetProdPerPop()
        {
            return ProdPerPop;
        }
        public int GetFoodPerPop()
        {
            return FoodPerPop;
        }
        public int GetSciPerPop()
        {
            return SciPerPop;
        }
        public bool GetUnassigned()
        {
            return Unassigned;
        }
        public void AssignWorker()
        {
            ProdPop++;
            Production = Production + ProdPerPop;
            Unassigned = false;
        }
        public void AssignFarmer()
        {
            FoodPop++;
            Food = Food + FoodPerPop;
            Unassigned = false;
        }
        public void AssignResearcher()
        {
            SciPop++;
            Science = Science + SciPerPop;
            Unassigned = false;
        }
        public string GetBaseTech()
        {
            return BaseTechChoice;
        }
        public string GetCombatTech()
        {
            return CombatTechChoice;
        }
        public double GetBaseProgress()
        {
            return BaseProgress;
        }
        public double GetCombatProgress()
        {
            return CombatProgress;
        }
        public double GetCombatCost()
        {
            return CombatCost;
        }
        public double GetBaseCost()
        {
            return BaseCost;
        }
        public void ChangeFocus()
        {
            if (BaseFocus)
                BaseFocus = false;
            else
                BaseFocus = true;
        }
        public int GetSupport()
        {
            return Support;
        }
    }
}

