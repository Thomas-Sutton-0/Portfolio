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
    class Camera
    {
        public Vector2 Position;
        Viewport viewport;
        public Matrix transform;
        private float Zoom = 0.5f;

        public Camera(Viewport newviewport)
        {
            Position = new Vector2(100, 100);
            viewport = newviewport;
        }
        public void Update(GameTime gameTime, Tile[] Tiles)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    Position.X = Position.X - 23;
                    foreach (Tile Box in Tiles)
                    {
                        Box.ClickBox = new Rectangle(((Box.GetPosition() * 100) + Convert.ToInt32(0.447f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100)) - (Convert.ToInt32(Position.X) - 100) / 2), Box.ClickBox.Y, Convert.ToInt32(192 * Zoom), Convert.ToInt32(192 * Zoom));
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    Position.X = Position.X + 23;
                    foreach (Tile Box in Tiles)
                    {
                        Box.ClickBox = new Rectangle(((Box.GetPosition() * 100) + Convert.ToInt32(0.447f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100)) + (100 - Convert.ToInt32(Position.X)) / 2), Box.ClickBox.Y, 96, 96);
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    Position.Y = Position.Y - 23;
                    foreach (Tile Box in Tiles)
                    {
                        Box.ClickBox = new Rectangle(Box.ClickBox.X, (((Box.GetRow() * 100) + Convert.ToInt32(0.403f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100))) - (Convert.ToInt32(Position.Y) - 100) / 2), 96, 96);
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    Position.Y = Position.Y + 23;
                    foreach (Tile Box in Tiles)
                    {
                        Box.ClickBox = new Rectangle(Box.ClickBox.X, (((Box.GetRow() * 100) + Convert.ToInt32(0.403f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100))) + (100 - Convert.ToInt32(Position.Y)) / 2), 96, 96);
                    }
                }
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    Position.X = Position.X - 10;
                    foreach (Tile Box in Tiles)
                    {
                        Box.ClickBox = new Rectangle(((Box.GetPosition() * 100) + Convert.ToInt32(0.447f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100)) - (Convert.ToInt32(Position.X) - 100) / 2), Box.ClickBox.Y, Convert.ToInt32(192 * Zoom), Convert.ToInt32(192 * Zoom));
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    Position.X = Position.X + 10;
                    foreach (Tile Box in Tiles)
                    {
                        Box.ClickBox = new Rectangle(((Box.GetPosition() * 100) + Convert.ToInt32(0.447f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100)) + (100 - Convert.ToInt32(Position.X)) / 2), Box.ClickBox.Y, Convert.ToInt32(192 * Zoom), Convert.ToInt32(192 * Zoom));
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    Position.Y = Position.Y - 10;
                    foreach (Tile Box in Tiles)
                    {
                        Box.ClickBox = new Rectangle(Box.ClickBox.X, (((Box.GetRow() * 100) + Convert.ToInt32(0.403f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100))) - (Convert.ToInt32(Position.Y) - 100) / 2), Convert.ToInt32(192 * Zoom), Convert.ToInt32(192 * Zoom));
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    Position.Y = Position.Y + 10;
                    foreach (Tile Box in Tiles)
                    {
                        Box.ClickBox = new Rectangle(Box.ClickBox.X, (((Box.GetRow() * 100) + Convert.ToInt32(0.403f * (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100))) + (100 - Convert.ToInt32(Position.Y)) / 2), Convert.ToInt32(192 * Zoom), Convert.ToInt32(192 * Zoom));
                    }
                }
            }


            transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) * Matrix.CreateRotationZ(0) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        }
        public void TechBox(SpriteBatch spriteBatch, Rectangle TechBox, Texture2D Texture)
        {
            spriteBatch.Draw(Texture, new Vector2(1820 + Position.X, Position.Y - 780), null, Color.White, 0f, new Vector2(Texture.Width, Texture.Height), 1, SpriteEffects.None, 0);
        }
        public void EndScreen(SpriteBatch spriteBatch, Texture2D Texture)
        {
            spriteBatch.Draw(Texture, new Vector2(Position.X + 1820, Position.Y + 1000), null, Color.White, 0f, new Vector2(Texture.Width, Texture.Height), 5.92f, SpriteEffects.None, 0);
        }
    }
}
