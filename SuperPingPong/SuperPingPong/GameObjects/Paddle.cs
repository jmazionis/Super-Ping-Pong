using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperPingPong.GameObjects
{
    public class Paddle : GameObject
    {
        Texture2D texture;
        KeyboardState keyboardState;
        Vector2 upVector;
        Vector2 downVector;
        Keys keyUp;
        Keys keyDown;
        GraphicsDeviceManager graphics;

        public Paddle(GraphicsDeviceManager graphics, Texture2D texture, Vector2 position, Keys keyUp, Keys keyDown)
        {
            this.graphics = graphics;
            this.texture = texture;
            this.keyUp = keyUp;
            this.keyDown = keyDown;
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Position = position;
            SpeedCoefficient = 6;
            upVector = new Vector2(0, -1) * SpeedCoefficient;
            downVector = new Vector2(0, 1) * SpeedCoefficient;
        }

        protected override void Move()
        {
            keyboardState = Keyboard.GetState();

            if (!(Position.Y <= texture.Height / 2))
	        {
		        if (keyboardState.IsKeyDown(keyUp))
                {
                    Position += upVector;
                }
	        }

            if (!(Position.Y >= graphics.PreferredBackBufferHeight - texture.Height / 2))
            {
                if (keyboardState.IsKeyDown(keyDown))
                {
                    Position += downVector;
                }
            }    
        }
    }
}
