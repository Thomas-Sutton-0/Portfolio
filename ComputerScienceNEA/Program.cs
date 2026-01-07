using System;

namespace ComputerScienceNEA
{
#if WINDOWS || LINUX
    public static class Program
    {
        // The main entry point for the application.
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
            
        }
    }
#endif
}
