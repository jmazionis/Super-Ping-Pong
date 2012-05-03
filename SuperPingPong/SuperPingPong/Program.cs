using System;
using System.Threading;

namespace SuperPingPong
{
    public enum GamerNetworkType {Client, Server}

#if WINDOWS || XBOX
    static class Program
    {
        public static GamerNetworkType GamerNetwork;
      
        static void Main(string[] args)
        {
            string c;
            bool correctInputFound = false;
            Console.Write("Enter S to start server or C to start client: ");
            while (!correctInputFound)
            {
                c = Console.ReadLine();
                switch (c)
                {
                    case "S": 
                        GamerNetwork = GamerNetworkType.Server;
                        correctInputFound = true;
                        break;
                    case "C":
                        GamerNetwork = GamerNetworkType.Client;
                        correctInputFound = true;
                        break;
                    default:
                        Console.Write("Invalid input, try again: ");
                        break;
                }
            }
            
            using (PingPong game = new PingPong())
            {
                game.Run();
            }
        }
    }
#endif
}

