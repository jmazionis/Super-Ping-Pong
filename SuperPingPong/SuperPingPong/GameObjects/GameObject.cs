using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperPingPong.GameObjects
{
    public abstract class GameObject
    {
        public virtual Vector2 Position { get; set; }
        public virtual Vector2 Velocity { get; set; }
        public virtual Vector2 Origin { get; set; }
        public virtual byte SpeedCoefficient { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Position, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);
        }

        protected virtual void Move()
        {
            Position += SpeedCoefficient * Velocity;
        }

        public virtual void Update()
        {
            Move();
        }
    }
}
