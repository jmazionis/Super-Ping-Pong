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
using SuperPingPong.GameObjects;
using SuperPingPong.Networking;
using Lidgren.Network;
using System.Text;

namespace SuperPingPong
{
    public class PingPong : Game
    {
        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;

        Texture2D ballTexture;
        Texture2D greenPaddleTexture;
        Texture2D bluePaddleTexture;

        RenderTarget2D blurHr;
        RenderTarget2D blurVr;
        RenderTarget2D wholeScene;

        Effect horizontalBlur;
        Effect verticalBlur;

        Ball ball;
        Paddle paddle1;
        Paddle paddle2;

        SpriteFont font;

        PresentationParameters pp;

        NetworkManager networkManager;
        GameMessageManager gameMessage;

        public PingPong()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            networkManager = new NetworkManager(Program.GamerNetwork);
            networkManager.SetUpConnection();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ballTexture = Content.Load<Texture2D>("ballNew");
            greenPaddleTexture = Content.Load<Texture2D>("newPaddleGreen");
            bluePaddleTexture = Content.Load<Texture2D>("newPaddleBlue");

            horizontalBlur = Content.Load<Effect>("BlurHorizontally");
            verticalBlur = Content.Load<Effect>("BlurVertically");

            font = Content.Load<SpriteFont>("ScoreFont");

            pp = GraphicsDevice.PresentationParameters;

            blurHr = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth / 2, pp.BackBufferHeight / 2);
            blurVr = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth / 2, pp.BackBufferHeight / 2);
            wholeScene = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            ball = new Ball(graphics, ballTexture);
            paddle1 = new Paddle(graphics,
                                greenPaddleTexture, 
                                new Vector2((float) greenPaddleTexture.Width / 2, (float) graphics.PreferredBackBufferHeight / 2), 
                                Keys.A, 
                                Keys.Z);
            paddle2 = new Paddle(graphics,
                                 bluePaddleTexture,
                                 new Vector2((float) graphics.PreferredBackBufferWidth - bluePaddleTexture.Width / 2, (float) graphics.PreferredBackBufferHeight / 2),
                                 Keys.Up,
                                 Keys.Down);
            gameMessage = new GameMessageManager(paddle1, paddle2, ball);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {    
            if (Program.GamerNetwork == GamerNetworkType.Server)
            {
                this.ProcessServerNetworkMessages();
                paddle1.Update();
                ball.Update();
                ball.CheckForReflection(paddle1, paddle2);
            }
            else
            {
                this.ProcessClientNetworkMessages();
                paddle2.Update();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(wholeScene);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            ball.Draw(spriteBatch, ballTexture);
            paddle1.Draw(spriteBatch, greenPaddleTexture);
            paddle2.Draw(spriteBatch, bluePaddleTexture);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(blurHr);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            horizontalBlur.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(wholeScene, new Rectangle(0, 0, wholeScene.Width / 2, wholeScene.Height / 2), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(blurVr);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            verticalBlur.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(blurHr, new Rectangle(0, 0, wholeScene.Width / 2, wholeScene.Height / 2), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(blurVr, new Rectangle(0, 0, wholeScene.Width, wholeScene.Height), Color.White);
            ball.Draw(spriteBatch, ballTexture);
            paddle1.Draw(spriteBatch, greenPaddleTexture);
            paddle2.Draw(spriteBatch, bluePaddleTexture);
            spriteBatch.DrawString(font, ball.Player1Score.ToString(), new Vector2(200, 40), Color.LimeGreen);
            spriteBatch.DrawString(font, ball.Player2Score.ToString(), new Vector2(600, 40), Color.Cyan);
            spriteBatch.End();


            base.Draw(gameTime);
        }

        private void ProcessServerNetworkMessages()
        {
            NetIncomingMessage im;

            this.networkManager.SendToClient(gameMessage);

            while ((im = this.networkManager.Server.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus) im.ReadByte())
                        {
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                im.SenderConnection.Approve();
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        gameMessage.DecodeClientMessage(im);
                        break;

                }

                this.networkManager.Server.Recycle(im);
            }
        }

        private void ProcessClientNetworkMessages()
        {
            NetIncomingMessage im;

            this.networkManager.SendToServer(gameMessage);

            while ((im = this.networkManager.Client.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        gameMessage.DecodeServerMessage(im);
                        break;
                }
                this.networkManager.Client.Recycle(im);
            }
        }
    }
}
