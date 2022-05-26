using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using System;
using SampSharp.GameMode;
using SampSharpGamemode.Ipfunc;
using System.Text.Json;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using System.Collections.Generic;
using System.Threading.Tasks;
using SampSharp.GameMode.Display;
using System.Linq;
using System.Net;
using System.IO;

namespace SampSharpGamemode.Players
{
    public class IPresponse
    {
        public string status;
        public string country;
        public string countryCode;
        public string region;
        public string regionName;
        public string city;
        public string zip;
        public float lat;
        public float lon;
        public string timezone;
        public string isp;
        public string org;
        public string _as;
        public string query;
    }
    [PooledType]
    public class Player : BasePlayer
    {
        private string leavingreason = String.Empty;
        public Inventary inventary = new Inventary();
        private string realip;
        public override void OnConnected(EventArgs e)
        {
            PVars[PvarsInfo.isleaving] = false;
            foreach (var p in BasePlayer.All.Where(p => p.PVars.Get<bool>(PvarsInfo.ingame)))
            {
                if (p.PVars.Get<bool>(PvarsInfo.admin))
                    p.SendClientMessage(Colors.GREY, $"{Name} [ID {Id}] подключился к серверу. (IP: {IP})");
                else
                    p.SendClientMessage(Colors.GREY, $"{Name} [ID {Id}] подключился к серверу.");
            }
            base.OnConnected(e);

            string geo;
            WebRequest request = WebRequest.Create("http://ip-api.com/json/"+this.IP+"?lang=ru");
            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                geo = reader.ReadToEnd();
            }
            IPresponse data = JsonSerializer.Deserialize<IPresponse>(geo);
            if (data.status == "success")
                geo = data.country + "/" + data.city;
            else
                geo = "nogeo";
            GameMode.db.InsertSessions(this.Name, this.IP, geo);
            PVars[PvarsInfo.sessionid] = int.Parse(GameMode.db.LAST_INSERT_ID().data[0][0]);
            PVars[PvarsInfo.authstate] = (int)e_AuthState.PASSWORD;
            realip = IP;
            Auth();
        }
        public override void OnDisconnected(DisconnectEventArgs e)
        {
            PVars[PvarsInfo.isleaving] = true;
            //TODO: при бане определять что это бан
            e_IP action = e_IP.log_left;
            if (e.Reason == DisconnectReason.Left)
            {
                if (PVars.Get<bool>(PvarsInfo.ingame))
                    action = e_IP.log_left;
                else
                    action = e_IP.log_nlleft;
            }
            GameMode.db.UpdateSessions_action(PVars.Get<int>(PvarsInfo.sessionid), (int)action);

            string serverreason;
            switch (e.Reason)
            {
                case DisconnectReason.Left:
                    serverreason = "Leaving";
                    break;
                case DisconnectReason.TimedOut:
                    serverreason = "Time Out";
                    break;
                case DisconnectReason.Kicked:
                    serverreason = "Kicked/Banned";
                    break;
                default:
                    serverreason = "Wtf?!?!?!!";
                    break;
            }
            leavingreason = leavingreason == String.Empty ? serverreason : leavingreason;
            foreach(var p in BasePlayer.All)
            {
                if (p.PVars.Get<bool>(PvarsInfo.admin))
                    p.SendClientMessage(Colors.GREY, $"{Name} [ID {Id}] покинул сервер. (IP: {realip}) ({leavingreason})");
                else
                    p.SendClientMessage(Colors.GREY, $"{Name} [ID {Id}] покинул сервер. ({serverreason})");
            }
            base.OnDisconnected(e);
        }
        public void Auth()
        {
            var dbinfo = GameMode.db.SelectPlayerByNickname(this.Name).data;
            if (dbinfo.Count != 0)
            {
                GameMode.db.UpdateSessions_uid(PVars.Get<int>(PvarsInfo.sessionid), int.Parse(dbinfo[0][(int)e_PlayerInfo.PINFO_UID]));
                AuthSystem.Start(this);
            }  
            else
                RegisterationSystem.Start(this);
        }
        public void LoadInfo()
        {
            //pvars
            var pinfo = GameMode.db.SelectPlayerByNickname(this.Name).data[0];

            PVars[PvarsInfo.uid] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_UID]);
            PVars[PvarsInfo.score] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_SCORE]);
            PVars[PvarsInfo.helplevel] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_HELPERLVL]);
            PVars[PvarsInfo.adminlevel] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_ADMINLVL]);
            PVars[PvarsInfo.skin] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_SKIN]);
            PVars[PvarsInfo.money] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_MONEY]);
            PVars[PvarsInfo.lastip] = pinfo[(int)e_PlayerInfo.PINFO_LASTIP];
            PVars[PvarsInfo.totpkey] = pinfo[(int)e_PlayerInfo.PINFO_TOTPKEY];
            PVars[PvarsInfo.isevent] = Convert.ToBoolean(int.Parse(pinfo[(int)e_PlayerInfo.PINFO_EVENT]));

            PVars[PvarsInfo.ingame] = true;
            PVars[PvarsInfo.helper] = PVars.Get<int>(PvarsInfo.helplevel) > 0;
            PVars[PvarsInfo.admin] = PVars.Get<int>(PvarsInfo.adminlevel) > 0;
            PVars[PvarsInfo.isTemp] = false;


            GameMode.db.UpdatePlayerLastIP(this);

            //inventary
            var invinfo = GameMode.db.SelectInventary(PVars.Get<int>(PvarsInfo.uid)).data[0][0];
            string[] inv = invinfo.Split(',');
            int pos = 0;
            foreach (string v in inv)
            {
                string[] d = v.Split(' ');
                int itemid = int.Parse(d[0]), amount = int.Parse(d[1]);
                Item item = GameMode.FindItem(itemid);
                inventary.Set(item, pos);
                pos++;
            }

            SetSpawnInfo(0, 0, new Vector3(1642.0735f, -2239.6826f, 13.4964f), 269.15f);
            Score = PVars.Get<int>(PvarsInfo.score);
            Money = PVars.Get<int>(PvarsInfo.money);
        }
        public void ban(string s, string reason, bool isconst)
        {
            PVars[PvarsInfo.isleaving] = true;
            string info = "Причина выдачи блокировки: "+reason;
            if (isconst)
                info = "Тип блокировки: Постоянный. Аккаунт не подлежир разбану.\n";
            else
                info = "Тип блокировки: Временный. Блокировка будет автоматически снята. .... .. ХУЙ ХУЙ ХУЙ \n";
            var dialog = new MessageDialog("Ваш аккаунт заблокирован", info + "Тут всякая инфа мне лень писать\nОООЧЕНЬ МНОГО ИНФЫ НАЗАР ТЫ МОЛОДЕЦ\nТы так постарался чтобы игрокам было не так обидно\nда и в целом чтобы играть было интересно\nХотя с другой стороны ты просто зарабатываешь бабки\nЭто бизнес, мэнчик....\nМда пизлец чето меня в час ночи понесло\nСоооообственно, что я хотел сказать.... Ничего, точно!", "ТЕРПАНУТЬ");
            dialog.Show(this);
            kick(s);
        }
        public void kick(string s = "")
        {
            PVars[PvarsInfo.isleaving] = true;
            //set reason
            leavingreason = s;
            Task.Delay(1000).ContinueWith(t => base.Kick());
        }
        public override void OnRequestSpawn(RequestSpawnEventArgs e)
        {
            if (PVars.Get<bool>(PvarsInfo.ingame))
                base.OnRequestSpawn(e);

        }
        public override void OnSpawned(SpawnEventArgs e)
        {
            Position = new Vector3(1642.0735f, -2239.6826f, 13.4964f);
            Skin = PVars.Get<int>(PvarsInfo.skin);
            base.OnSpawned(e);
        }
        public override void OnText(TextEventArgs e)
        {
            var near = BasePlayer.All.Where(p => (GetDistanceFromPoint(p.Position) <= 10 && VirtualWorld == p.VirtualWorld && Interior == p.Interior));
            foreach (var p in near) p.SendClientMessage(Colors.GREEN, $"{Name} сказал: {e.Text}");
            return;
        }
    }
}