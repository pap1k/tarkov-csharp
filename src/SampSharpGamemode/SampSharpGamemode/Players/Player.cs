using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using System;
using SampSharp.GameMode;
using System.Text;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using System.Collections.Generic;
using System.Threading.Tasks;
using SampSharp.GameMode.Display;
using System.Linq;

namespace SampSharpGamemode.Players
{
    [PooledType]
    public class Player : BasePlayer
    {
        private string leavingreason = String.Empty;
        public Inventary inventary = new Inventary();
        public override void OnConnected(EventArgs e)
        {
            foreach (var p in BasePlayer.All)
            {
                if (p.PVars.Get<bool>(PvarsInfo.admin))
                    p.SendClientMessage(Colors.GREY, $"[ID {Id}] [{IP}] {Name} подключился к серверу.");
                else
                    p.SendClientMessage(Colors.GREY, $"{Name} подключился к серверу.");
            }
            base.OnConnected(e);
            Auth();
        }
        public override void OnDisconnected(DisconnectEventArgs e)
        {
            string serverreason;
            switch (e.Reason)
            {
                case DisconnectReason.Left:
                    serverreason = "left";
                    break;
                case DisconnectReason.TimedOut:
                    serverreason = "TimeOut";
                    break;
                case DisconnectReason.Kicked:
                    serverreason = "kicked/banned";
                    break;
                default:
                    serverreason = "Wtf?!?!?!!";
                    break;
            }
            leavingreason = leavingreason == String.Empty ? serverreason : leavingreason;
            foreach(var p in BasePlayer.All)
            {
                if (p.PVars.Get<bool>(PvarsInfo.admin))
                    p.SendClientMessage(Colors.GREY, $"[ID {Id}] [{IP}] {Name} покинул сервер. ({leavingreason})");
                else
                    p.SendClientMessage(Colors.GREY, $"{Name} покинул сервер. ({serverreason})");
            }
            base.OnDisconnected(e);
        }
        public void Auth()
        {
            var dbinfo = GameMode.db.SelectPlayerByNickname(this.Name).data;
            if (dbinfo.Count != 0)
                AuthSystem.Start(this);
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

            PVars[PvarsInfo.ingame] = true;
            PVars[PvarsInfo.helper] = PVars.Get<int>(PvarsInfo.helplevel) > 0;
            PVars[PvarsInfo.admin] = PVars.Get<int>(PvarsInfo.adminlevel) > 0;
            PVars[PvarsInfo.isTemp] = false;

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
            Skin = PVars.Get<int>(PvarsInfo.skin);
            Score = PVars.Get<int>(PvarsInfo.score);
            Money = PVars.Get<int>(PvarsInfo.money);
        }
        public void kick(string s = "")
        {
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