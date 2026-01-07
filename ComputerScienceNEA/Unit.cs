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
    class Unit
    {
        public int Health;
        public int MaxHealth;
        public int CS; //Stands for Combat Strength
        public int RS; //Stands for ranged strength
        public int TileNum;
        public int MaxMP; //Stands for Maximum Movement Points
        public int MP; //Stands for Movement Points 
        public int ListNum;
        public bool PlayerUnit;
        public bool Selected;
        public string Bonus; //The type of unit it has a bonus against
        public Texture2D Texture;
        public string UnitType;


        public int[] Path(int StartingTile, int EndTile, Tile[] Tiles)
        {
            bool End = false;
            bool[] Visited = new bool[252];
            //This array will store whether or not the tile in the same index in the Tiles array has already been checked, it will be true if it has been checked and false if it hasn't 
            int CurrentTile = StartingTile;
            int Counter = 0;
            int[] Path = new int[1];
            //The Path array will store the indexes of the tiles it has to move through
            int[] TileMoveCost = new int[252];
            int[] TileMoveSteps = new int[252];

            foreach (int TileMove in TileMoveCost)
            {
                TileMoveCost[Counter] = -1;
                Counter++;
            }
            Counter = 0;
            foreach (int TileMove in TileMoveSteps)
            {
                TileMoveSteps[Counter] = -1;
                Counter++;
            }
            Counter = 0;
            foreach (bool Visit in Visited)
            {
                Visited[Counter] = false;
                Counter++;
            }

            TileMoveCost[StartingTile] = 0;
            TileMoveSteps[StartingTile] = StartingTile;
            //This sets the movement cost of the starting tile to 0 and it's previous tile to itself as it doesn’t have to move from anywhere to get to the starting tile 

            while (End == false)
            {
                int[] Adjacent = Tiles[CurrentTile].GetAdTiles();
                int NextMoveCost;
                int NextTile;

                for (int i = 0; i < Adjacent.Length; i++)
                {
                    if (Adjacent[i] < 252 && Adjacent[i] >= 0)
                    {
                        NextMoveCost = Tiles[Adjacent[i]].GetMoveCost() + TileMoveCost[CurrentTile];
                        NextTile = Tiles[Adjacent[i]].GetListNum();
                        if (TileMoveCost[NextTile] == -1)
                        {
                            TileMoveCost[NextTile] = NextMoveCost;
                            TileMoveSteps[NextTile] = CurrentTile;
                        } //This sets the previous tile to the current tile and the movement cost to the movement cost of the new path found if a path to the tile in the index equal to the value in next tile hasn't already been found 

                        else if (NextMoveCost < TileMoveCost[NextTile])
                        {
                            TileMoveCost[NextTile] = NextMoveCost;
                            TileMoveSteps[NextTile] = CurrentTile;
                        } //This sets the previous tile to the current tile and the movement cost to the movement cost of the new path found if the movement cost of the previous path was more than the movement cost of the new path 
                    }
                }

                Visited[CurrentTile] = true;
                End = true;
                foreach (bool Check in Visited)
                {
                    if (Check != true)
                    {
                        End = false;
                    }
                } //This will check if all tiles have been visited or not, if they all have then the while loop will end once the current loop is complete 

                int LowestMoveCost = 999999999;
                foreach (Tile NewCurrent in Tiles)
                {
                    if ((Visited[NewCurrent.GetListNum()] != true) && (TileMoveCost[NewCurrent.GetListNum()] < LowestMoveCost) && (TileMoveCost[NewCurrent.GetListNum()] > 0))
                    {
                        LowestMoveCost = TileMoveCost[NewCurrent.GetListNum()];
                        CurrentTile = NewCurrent.GetListNum();
                    }
                } //This will decide which tile will be checked next by picking the tile with the lowest movement cost from the start that hasn't already been checked 
            }


            Counter = 0;
            End = false;
            Path[0] = EndTile;
            while (End == false)
            {
                Counter++;
                Array.Resize(ref Path, Counter + 1);    //This increases the size of the path array by one each time the loop is run, this ensures that no memory is used up that doesn't need to be 

                Path[Counter] = TileMoveSteps[Path[Counter - 1]];

                if (Path[Counter] == StartingTile)
                {
                    End = true;
                }
            } //This will create an array which stores the tile number of the different tiles that the path found has to go through in order to get from the start tile to the end tile (though it will be stored in reverse order) 
            Path.Reverse(); //Reverses the list so that the tiles are stored in the order they would be traveled through in the path 
            return Path;
        }

        public int[] Combat(Unit Attacker, Unit Defender, Tile[] Tiles, Base[] Bases) //The first number in the array will be the attacker health after the combat, the second will be the defender health after the combat (there will only be two numbers in the array)
        {
            int[] Results = new int[2];
            float AS = Attacker.CS;
            int AttackerSupport = 0;
            float DS = Defender.CS;
            float AttackMultiplier; //Results in a minimum multiplier of 0.8 and a maximum of 1.1 (so there will be a 20% CS penalty for having no health and a 10% bonus for having full health, there should never be a situation where a unit is on 0 health and able to attack however it may get close for example it might have 1 or 2 health remaining)
            float DefendMultiplier;
            float AttackerDamage; //Damage done TO the attacker
            float DefenderDamage; //Damage done TO the defender

            if(Attacker.Bonus == Defender.UnitType && Attacker.PlayerUnit)
            {
                AS = AS + Bases[0].Bonus;
            }
            else if (Attacker.Bonus == Defender.UnitType)
            {
                AS = AS + Bases[1].Bonus;
            }

            if(Defender.Bonus == Attacker.UnitType && Defender.PlayerUnit)
            {
                DS = DS + Bases[0].Bonus;
            }
            else if (Defender.Bonus == Attacker.UnitType)
            {
                DS = DS + Bases[1].Bonus;
            }

            if (Attacker.PlayerUnit && Attacker.InRange(Tiles, Attacker.TileNum, 115 , 2, 0))
            {
                AttackerSupport = Bases[0].GetSupport();
            }
            if (!Attacker.PlayerUnit && Defender.InRange(Tiles, Defender.TileNum, 136, 2, 0))
            {
                AttackerSupport = Bases[0].GetSupport();
            }
            AS = AS + AttackerSupport;

            DefendMultiplier = 0.8f + ((Defender.Health / Defender.MaxHealth) * 0.3f);
            AttackMultiplier = 0.8f + ((Attacker.Health / Attacker.MaxHealth) * 0.3f);
            AttackerDamage = (((DS * DefendMultiplier) + Tiles[Defender.TileNum].GetDefense()) / 2);
            DefenderDamage = (((AS * AttackMultiplier) / 2) - Tiles[Defender.TileNum].GetDefense());

            Results[0] = Convert.ToInt32(AttackerDamage);
            Results[1] = Convert.ToInt32(DefenderDamage);
            return Results;
        }

        public int[] BaseCombat(Unit Attacker, Base Defender)
        {
            int[] Results = new int[2];
            float AS = Attacker.CS;
            float DS = Defender.CS;
            float AttackMultiplier; //Results in a minimum multiplier of 0.8 and a maximum of 1.1 (so there will be a 20% CS penalty for having no health and a 10% bonus for having full health, there should never be a situation where a unit is on 0 health and able to attack however it may get close for example it might have 1 or 2 health remaining)
            float DefendMultiplier;
            float AttackerDamage; //Damage done TO the attacker
            float DefenderDamage; //Damage done TO the defender

            DefendMultiplier = 0.8f + (Defender.Health / Defender.MaxHealth) * 0.3f;
            AttackMultiplier = 0.8f + (Attacker.Health / Attacker.MaxHealth) * 0.3f;
            AttackerDamage = ((DS * DefendMultiplier) / 2);
            DefenderDamage = (((AS + Defender.GetSupport()) * AttackMultiplier) / 2);

            Results[0] = Convert.ToInt32(AttackerDamage);
            Results[1] = Convert.ToInt32(DefenderDamage);

            return Results;
        }
        public int BaseRangedAttack(Unit Attacker, Base Defender)
        {
            int EnemyDamage;
            float AttackMultiplier;

            AttackMultiplier = 0.5f + (Attacker.Health / Attacker.MaxHealth) * 0.6f; //Health of ranged units matters much more when attacking than usual
            EnemyDamage = Convert.ToInt32(((Attacker.RS * AttackMultiplier) - (Defender.GetSupport() / 2)) / 2);

            return EnemyDamage;
        }

        public bool InRange(Tile[] NewTiles, int NewATileNum, int NewDTileNum, int NewRange, int NewIteration) //The A in ATileNum stands for attacker and the D in DTileNum stands for defender
        {
            bool Inrange = false;
            int ATileNum = NewATileNum;
            int DTileNum = NewDTileNum;
            Tile[] Tiles = NewTiles;
            Tile OriginalTile = Tiles[ATileNum];
            int Iteration = NewIteration + 1;
            int Range = NewRange;

            foreach (int Checking in OriginalTile.GetAdTiles())
            {
                if(Checking == DTileNum && !Inrange)
                    Inrange = true;
                else if (!(Range <= Iteration) && !Inrange)
                {
                    Inrange = InRange(Tiles, Tiles[Checking].GetListNum(), DTileNum, Range, Iteration);
                }
            }
            
            return Inrange;
        }
        public int RangedAttack(Unit Attacker, Unit Defender, Tile[] Tiles)
        {
            int EnemyDamage;
            float AttackMultiplier;

            AttackMultiplier = 0.5f + (Attacker.Health / Attacker.MaxHealth) * 0.6f; //Health of ranged units matters much more when attacking than usual
            EnemyDamage = Convert.ToInt32((Attacker.RS * AttackMultiplier) - Tiles[Defender.TileNum].GetDefense());

            if (EnemyDamage < 10)
                EnemyDamage = 10;

            return EnemyDamage;
        }
        public void UnitUI(SpriteBatch spriteBatch, SpriteFont Font)    //Add UI when a unit is selected which shows max movement points against available movement points, combat strength, etc 
        {
            spriteBatch.DrawString(Font, "Movement points: " + MP + "/" + MaxMP, new Vector2(0.03f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0.31f * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.Black);
        }
        public void SetTexture(Texture2D NewTexture)
        {
            Texture = NewTexture;
        }
        public void Draw(SpriteBatch spriteBatch, Tile[] Tiles)
        {
            spriteBatch.Draw(Texture, new Vector2(Tiles[TileNum].VisualPosition.X - 21, Tiles[TileNum].VisualPosition.Y - 14), null, Color.White, 0f, Tiles[TileNum].Origin, 0.75f, SpriteEffects.None, 0);
        }
    }

    class Warrior : Unit
    {
        public Warrior(bool NewPlayerUnit, int NewHealth, int NewMaxHealth, int NewCS, int NewTileNum, Texture2D NewTexture, int NewMP, int NewListNum)
        {
            UnitType = "Warrior"; //Remove this later once not using selected texture 
            Bonus = "Spear";
            PlayerUnit = NewPlayerUnit;
            Health = NewHealth;
            CS = NewCS;
            TileNum = NewTileNum;
            Texture = NewTexture;
            MaxMP = NewMP;
            MP = NewMP;
            Selected = false;
            MaxHealth = NewMaxHealth;
            ListNum = NewListNum;
        }
    }

    class Ranged : Unit
    {
        public Ranged(bool NewPlayerUnit, int NewHealth, int NewMaxHealth, int NewCS, int NewTileNum, Texture2D NewTexture, int NewMP, int NewRS, int NewListNum)
        {
            UnitType = "Ranged"; //Remove this later once not using selected texture
            PlayerUnit = NewPlayerUnit;
            Health = NewHealth;
            CS = NewCS;
            TileNum = NewTileNum;
            Texture = NewTexture;
            MaxMP = NewMP;
            MP = NewMP;
            Selected = false;
            MaxHealth = NewMaxHealth;
            RS = NewRS; //Stands for ranged strength, will be used when attacking other units but not when being attacked
            ListNum = NewListNum;
        }
    }

    class Spear : Unit
    {
        public Spear(bool NewPlayerUnit, int NewHealth, int NewMaxHealth, int NewCS, int NewTileNum, Texture2D NewTexture, int NewMP, int NewListNum)
        {
            UnitType = "Spear"; //Remove this later once not using selected texture
            Bonus = "Cavalry";
            PlayerUnit = NewPlayerUnit;
            Health = NewHealth;
            CS = NewCS;
            TileNum = NewTileNum;
            Texture = NewTexture;
            MaxMP = NewMP;
            MP = NewMP;
            Selected = false;
            MaxHealth = NewMaxHealth;
            ListNum = NewListNum;
        }
    }

    class Cavalry : Unit
    {
        public Cavalry(bool NewPlayerUnit, int NewHealth, int NewMaxHealth, int NewCS, int NewTileNum, Texture2D NewTexture, int NewMP, int NewListNum)
        {
            UnitType = "Cavalry"; //Remove this later once not using selected texture
            Bonus = "Warrior";
            PlayerUnit = NewPlayerUnit;
            Health = NewHealth;
            CS = NewCS;
            TileNum = NewTileNum;
            Texture = NewTexture;
            MaxMP = NewMP;
            MP = NewMP;
            Selected = false;
            MaxHealth = NewMaxHealth;
            ListNum = NewListNum;
        }
    }
}