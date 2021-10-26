using System;

namespace MiniMonoGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            GAME game = new GAME();
            game.Run();
        }
    }
}
