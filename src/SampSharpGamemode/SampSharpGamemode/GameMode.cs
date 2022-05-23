using SampSharp.GameMode;
using System;
using IniParser;
using IniParser.Model;
using System.Collections.Generic;
using SampSharp.GameMode.SAMP.Commands;

namespace SampSharpGamemode
{
    public class GameMode : BaseMode
    {
        public static FileIniDataParser ini = new FileIniDataParser();
        public static IniData data = ini.ReadFile("./server.ini");
        public static DBWorker db = new DBWorker(data["database"]["host"], data["database"]["username"], data["database"]["password"], data["database"]["dbname"]);

        private const int _SERVER_ITEMS = 10;
        public static int SERFVER_ITEMS { get => _SERVER_ITEMS; }
        public static Item[] ServerItems;
        public static Item ErrorItem = new Item(-2, 0, "<Ошибка>", " ", false, false, false, 1);
        public static Item EmptyItem = new Item(0, 0, "", "", false, false, false, 1);
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            
            Console.WriteLine("\n----------------------------------");
            Console.WriteLine(data["server"]["GMName"]);
            Console.WriteLine("----------------------------------\n");

            SetGameModeText(data["server"]["GMName"]);
            LoadDBItems();
        }
        public static Item FindItem(int id)
        {
            if (id == 0)
                return EmptyItem;
            foreach (var i in ServerItems)
            {
                if (i.id == id)
                    return i;
            }
            return ErrorItem;
        }
        private void LoadDBItems()
        {
            Console.WriteLine($"Loading server items...");
            var items = db.SelectAllItems().data;
            ServerItems = new Item[_SERVER_ITEMS];
            int i = 0;
            foreach(List<string> col in items)
                ServerItems[i++] = new Item(
                        int.Parse(col[0]),
                        int.Parse(col[1]),
                        col[2], col[3],
                        Convert.ToBoolean(int.Parse(col[4])),
                        Convert.ToBoolean(int.Parse(col[5])),
                        Convert.ToBoolean(int.Parse(col[6])),
                        int.Parse(col[7])
                    );
            Console.WriteLine($"Total loaded {i} items.");
        }
    }
}