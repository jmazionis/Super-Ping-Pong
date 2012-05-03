using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperPingPong.GameObjects;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace SuperPingPong.Networking
{
    public class GameMessageManager
    {
        private Paddle paddle1;
        private Paddle paddle2;
        private Ball ball;

        public GameMessageManager(Paddle paddle1, Paddle paddle2, Ball ball)
        {
            this.paddle1 = paddle1;
            this.paddle2 = paddle2;
            this.ball = ball;
        }

        public void DecodeClientMessage(NetIncomingMessage im)
        {
            Vector2 pos;
            pos.X = this.paddle2.Position.X;
            pos.Y = im.ReadFloat();
            this.paddle2.Position = pos;
        }

        public void EncodeMessageToClient(NetOutgoingMessage om)
        {
            om.Write(this.ball.Position.X);
            om.Write(this.ball.Position.Y);
            om.Write(this.paddle1.Position.Y);
        }

        public void DecodeServerMessage(NetIncomingMessage im)
        {
            Vector2 pos;
            pos.X = im.ReadFloat();
            pos.Y = im.ReadFloat();
            this.ball.Position = pos;
            pos.X = this.paddle1.Position.X;
            pos.Y = im.ReadFloat();
            this.paddle1.Position = pos;  
        }

        public void EncodeMessageToServer(NetOutgoingMessage om)
        {
            om.Write(this.paddle2.Position.Y);
        }
    }
}
