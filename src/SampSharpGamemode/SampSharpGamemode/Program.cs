using SampSharp.Core;

namespace SampSharpGamemode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new GameModeBuilder()
                .UseEncodingCodePage("cp1251")
                .Use<GameMode>()
                .Run();
        }
    }
}
