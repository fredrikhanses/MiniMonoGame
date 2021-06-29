using System;

namespace MiniMonoGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Game game = new Game();
            game.Run();
        }
    }
}
