using System;

namespace NeuralNetworkToolkit
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (WolvesAndSheep game = new WolvesAndSheep())
            {
                game.Run();
            }
        }
    }
#endif
}

