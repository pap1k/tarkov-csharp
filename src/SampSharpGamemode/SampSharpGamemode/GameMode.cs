using SampSharp.GameMode;
using System;
using IniParser;
using IniParser.Model;
using System.Collections.Generic;
using System.Security.Cryptography;
using SampSharp.GameMode.SAMP.Commands;
using SampSharpGamemode.Players;
using System.Text;
using SampSharp.GameMode.World;
using SampSharp.Core.Callbacks;
using System.Linq;
using SampSharpGameMode;
using SampSharp.Core.Natives.NativeObjects;
using SampSharpGamemode.Parkings;
using System.Globalization;

namespace SampSharpGamemode
{
    public class GameMode : BaseMode
    {
        public static FileIniDataParser ini = new FileIniDataParser();
        public static IniData data = ini.ReadFile("./server.ini");
        public static DBWorker db = new DBWorker(data["database"]["host"], data["database"]["username"], data["database"]["password"], data["database"]["dbname"]);
        public static MyCustomNatives Native = NativeObjectProxyFactory.CreateInstance<MyCustomNatives>();
        private const int _SERVER_ITEMS = 10;
        public static int SERVER_ITEMS { get => _SERVER_ITEMS; }
        public static List<Parking> ServerParkings = new List<Parking>();
        public static Item[] ServerItems;
        public static Item ErrorItem = new Item(-2, 0, "<Ошибка>", " ", false, false, false, 1);
        public static Item EmptyItem = new Item(0, 0, "[пусто]", "", false, false, false, 1);
        protected override void OnInitialized(EventArgs e)
        {
            Console.WriteLine("\n----------------------------------");
            Console.WriteLine(data["server"]["GMName"]);
            Console.WriteLine("----------------------------------\n");

            SetGameModeText(data["server"]["GMName"]);
            LimitGlobalChatRadius(10);
            SetNameTagDrawDistance(20);
            EnableStuntBonusForAll(false);
            ShowPlayerMarkers(SampSharp.GameMode.Definitions.PlayerMarkersMode.Off);
            LoadDBItems();
            LoadDBParkings();
            base.OnInitialized(e);
        }
        public static List<int> getAdminIds()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i <= BasePlayer.Max; i++)
            {
                var p = BasePlayer.Find(i);

                if (p != null && p.IsConnected && p.PVars.Get<bool>(PvarsInfo.admin))
                    ids.Add(i);
            }
            return ids;
        }
        public static string getHash(string s)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(s);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
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
        private void LoadDBParkings()
        {
            var dbpark = db.SelectAllParkings().data;
            foreach (var row in dbpark)
                ServerParkings.Add(new Parking(
                        int.Parse(row[(int)e_DBParking.UID]),
                        float.Parse(row[(int)e_DBParking.POSX]),
                        float.Parse(row[(int)e_DBParking.POSY]),
                        float.Parse(row[(int)e_DBParking.POSZ]),
                        float.Parse(row[(int)e_DBParking.ROTATION]),
                        int.Parse(row[(int)e_DBParking.OWNER]),
                        int.Parse(row[(int)e_DBParking.ATTACHEDHOUSE]),
                        int.Parse(row[(int)e_DBParking.CARID])
                    ));
            Console.WriteLine($"[INFO] Loaded {dbpark.Count} parkings");
        }
        [Callback]
        internal bool OnClientCheckResponseFix(int id, int type, int arg, int response)
        {
            var player = Player.Find(id);
            if (player != null)
            {
                switch (type)
                {
                    //col
                    case 0x47:
                        player.SendClientMessage($"Model {arg} has {response} checksum");
                        break;
                }
            }
            else
                foreach(var adm in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                    adm.SendClientMessage(Colors.RED, $"[BUG](код {(int)e_BugCodes.CLIENTCHECKRESPONSE}): Не удалось обработать клиентчек игрока. Сообщите создателю время и никнейм последнего подключившегося игрока.");
            return true;
        }
    }
}