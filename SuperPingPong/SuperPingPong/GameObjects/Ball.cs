using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperPingPong.GameObjects
{
    public class Ball : GameObject
    {
        Random random;
        Texture2D ballTexture;
        GraphicsDeviceManager graphics;
        const int BallRadius = 10;
        const int PaddleWidth = 8;
        Vector2 screenCenter;

        public int Player1Score { get; set; }
        public int Player2Score { get; set; }

        public Ball(GraphicsDeviceManager graphics, Texture2D ballTexture)
        {
            this.graphics = graphics;
            this.ballTexture = ballTexture;
            this.random = new Random();
            this.screenCenter = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            this.ResetPosition();
            Player1Score = Player2Score = 0;
            Origin = new Vector2(ballTexture.Width / 2, ballTexture.Height / 2);
            SpeedCoefficient = 5;
            Velocity = SpeedCoefficient * new Vector2((float) ((random.NextDouble() * 2 - 1) / 2), (float) ((random.NextDouble() * 2 - 1)) / 2);
        }

        private void ResetPosition()
        {
            Position = this.screenCenter;
            Velocity = SpeedCoefficient * new Vector2((float) ((random.NextDouble() * 2 - 1) / 2), (float) ((random.NextDouble() * 2 - 1)) / 2);
        }

        public void CheckForReflection(Paddle paddle1, Paddle paddle2)
        {
            Vector2 normal;

            if (Position.X < BallRadius + PaddleWidth)
            {
                if ((Position.Y > paddle1.Position.Y - 40) && (Position.Y < paddle1.Position.Y + 40))
                {
                    normal = new Vector2(1, 0);
                    Velocity = Vector2.Reflect(Velocity, normal);
                }
                else
                {
                    Player2Score++;
                    this.ResetPosition();
                }
               
            }

            if ((Position.X > graphics.PreferredBackBufferWidth - BallRadius - PaddleWidth))
            {
                if ((Position.Y > paddle2.Position.Y - 40) && (Position.Y < paddle2.Position.Y + 40))
                {
                    normal = new Vector2(-1, 0);
                    Velocity = Vector2.Reflect(Velocity, normal);
                }
                else
                {
                    Player1Score++;
                    this.ResetPosition();
                }
            }

            if (Position.Y < BallRadius)
            {
                normal = new Vector2(0, -1);
                Velocity = Vector2.Reflect(Velocity, normal);
                return;
            }

            if (Position.Y > graphics.PreferredBackBufferHeight - BallRadius)
            {
                normal = new Vector2(0, 1);
                Velocity = Vector2.Reflect(Velocity, normal);
                return;
            }
        }
    }
}
