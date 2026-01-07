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
    class Tile
    {
        private int[] AdjacentTiles;
        public int Row;
        public int Position;
        private int MovementCost;
        private int ListNum;
        private int Defense; //Stores the bonus to combat strength when defending on this tile 
        private int UnitNum; //Stores the index (for whatever array the unit's in) of the unit currently on it 
        public Vector2 VisualPosition;
        public Vector2 Origin;
        private Texture2D Texture;
        private string Feature;
        public bool PlayerPresent = false; //Keeps track of if a player unit is currently on the tile or not
        public bool EnemyPresent = false; //Keeps track of if an AI unit is currently on the tile or not
        private bool TopEdge;
        private bool LeftEdge;
        private bool RightEdge;
        private bool BottomEdge;
        public Rectangle ClickBox;


        public Tile(int NewRow, int NewPosition, int NewMovementCost, Vector2 VisPos, Texture2D NewTexture, string NewFeature, int NewListNum)
        {
            ClickBox = new Rectangle(Convert.ToInt32(((NewPosition * 100)) + (0.447f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100))), Convert.ToInt32((NewRow * 100) + (0.403f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100))), 96, 96);
            Row = NewRow;
            Position = NewPosition;
            VisualPosition = VisPos;
            Texture = NewTexture;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Feature = NewFeature;    //Features: regular (flat), hills, stone, wheat, forest 

            MovementCost = 1;
            ListNum = NewListNum;
            AdjacentTiles = new int[0];
            if (Row == 1)
                TopEdge = true;
            if (Row == 9)
                BottomEdge = true;
            if (Position == 1)
                LeftEdge = true;
            if (Position == 28)
                RightEdge = true;

            for (int i = 0; i < 8; i++)
            {
                if (i == 0 && !TopEdge && !RightEdge)
                {
                    Array.Resize(ref AdjacentTiles, AdjacentTiles.Length + 1);
                    AdjacentTiles[AdjacentTiles.Length - 1] = ListNum - 27;
                }
                else if (i == 1 && !TopEdge)
                {
                    Array.Resize(ref AdjacentTiles, AdjacentTiles.Length + 1);
                    AdjacentTiles[AdjacentTiles.Length - 1] = ListNum - 28;
                }
                else if (i == 2 && !TopEdge && !LeftEdge)
                {
                    Array.Resize(ref AdjacentTiles, AdjacentTiles.Length + 1);
                    AdjacentTiles[AdjacentTiles.Length - 1] = ListNum - 29;
                }
                else if (i == 3 && !LeftEdge)
                {
                    Array.Resize(ref AdjacentTiles, AdjacentTiles.Length + 1);
                    AdjacentTiles[AdjacentTiles.Length - 1] = ListNum - 1;
                }
                else if (i == 4 && !RightEdge)
                {
                    Array.Resize(ref AdjacentTiles, AdjacentTiles.Length + 1);
                    AdjacentTiles[AdjacentTiles.Length - 1] = ListNum + 1;
                }
                else if (i == 5 && !BottomEdge && !LeftEdge)
                {
                    Array.Resize(ref AdjacentTiles, AdjacentTiles.Length + 1);
                    AdjacentTiles[AdjacentTiles.Length - 1] = ListNum + 27;
                }
                else if (i == 6 && !BottomEdge)
                {
                    Array.Resize(ref AdjacentTiles, AdjacentTiles.Length + 1);
                    AdjacentTiles[AdjacentTiles.Length - 1] = ListNum + 28;
                }
                else if (i == 7 && !BottomEdge && !RightEdge)
                {
                    Array.Resize(ref AdjacentTiles, AdjacentTiles.Length + 1);
                    AdjacentTiles[AdjacentTiles.Length - 1] = ListNum + 29;
                }
            }
        }

        public string FeatureGenerate(float WheatNum, float StoneNum, float ForestNum, float HillNum, float Iteration, int ForrestRandomNum, int HillRandomNum, int StoneRandomNum, int WheatRandomNum)
        {
            float WheatChance;
            float StoneChance;
            float ForestChance;
            float HillChance;
            bool FeatureChosen = false;

            WheatChance = ((2f * Iteration) - (WheatNum * 40f) + 12f);
            StoneChance = ((2f * Iteration) - (StoneNum * 40f) + 12f);
            ForestChance = ((2f * Iteration) - (ForestNum * 40f) + 12f);
            HillChance = ((2f * Iteration) - (HillNum * 40f) + 12f);

            if (StoneRandomNum <= StoneChance)
            {
                Feature = "Stone";
                Defense = 0;
                MovementCost = 2;
                FeatureChosen = true;
            }

            if (WheatRandomNum <= WheatChance && !FeatureChosen)
            {
                Feature = "Wheat";
                Defense = 2;
                MovementCost = 1;
                FeatureChosen = true;
            }

            if (HillRandomNum <= HillChance && !FeatureChosen)
            {
                Feature = "Hill";
                Defense = 10;
                MovementCost = 2;
                FeatureChosen = true;
            }

            if (ForrestRandomNum <= ForestChance && !FeatureChosen)
            {
                Feature = "Forest";
                Defense = 5;
                MovementCost = 2;
                FeatureChosen = true;
            }

            if (!FeatureChosen)
            {
                Feature = "Regular";
                Defense = 0;
                MovementCost = 1;
            }

            return Feature;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Feature == "Regular")
            {
                spriteBatch.Draw(Texture, VisualPosition, null, Color.LightGreen, 0f, Origin, 1, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(Texture, VisualPosition, null, Color.White, 0f, Origin, 1, SpriteEffects.None, 0);
            }
        }

        public int[] GetAdTiles()
        {
            return AdjacentTiles;
        }

        public void SetTexture(Texture2D NewTexture)
        {
            Texture = NewTexture;
        }
        public int GetRow()
        {
            return Row;
        }
        public int GetPosition()
        {
            return Position;
        }
        public int GetMoveCost()
        {
            return MovementCost;
        }
        public int GetListNum()
        {
            return ListNum;
        }
        public string GetFeature()
        {
            return Feature;
        }
        public int GetDefense()
        {
            return Defense;
        }
        public void SetUnitNum(int NewUnitNum)
        {
            UnitNum = NewUnitNum;
        }
        public int GetUnitNum()
        {
            return UnitNum;
        }
        public void SetMoveCost(int NewMoveCost)
        {
            MovementCost = NewMoveCost;
        }
    }
}
