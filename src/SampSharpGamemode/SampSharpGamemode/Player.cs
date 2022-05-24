using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using System;
using SampSharp.GameMode;
using System.Text;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampSharpGamemode
{
    [PooledType]
    public class Player : BasePlayer
    {
        public Inventary inventary = new Inventary();
        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);
            Auth();
        }
        public void Auth()
        {
            var dbinfo = GameMode.db.SelectPlayerByNickname(this.Name).data;
            if(dbinfo.Count != 0)
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
            PVars[PvarsInfo.admin]  = PVars.Get<int>(PvarsInfo.adminlevel) > 0;
            PVars[PvarsInfo.isTemp] = false;

            //inventary
            var invinfo = GameMode.db.SelectInventary(PVars.Get<int>(PvarsInfo.uid)).data[0][0];
            string[] inv = invinfo.Split(',');
            int pos = 0;
            foreach(string v in inv)
            {
                string[] d = v.Split(' ');
                int itemid = int.Parse(d[0]), amount = int.Parse(d[1]);
                Item item = GameMode.FindItem(itemid);
                inventary.Set(item, pos);
                pos++;
            }

            SetSpawnInfo(0, 0, new Vector3(1642.0735f, -2239.6826f, 13.4964f), 269.15f);
        }
        public void kick(string s)
        {
            //set reason
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
        private List<int> getAdminIds()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i <= Max; i++)
            {
                var p = Find(i);

                if (p != null && p.IsConnected && p.PVars.Get<bool>(PvarsInfo.admin))
                {
                    ids.Add(i);
                }
            }
            return ids;
        }
        //COMMANDS
        [Command("inv", "stock")]
        private void CMD_inv()
        {
            Inventary.Show(this);
        }
        [Command("admins", PermissionChecker = typeof(AllAdminPermChecker))]
        private void CMD_admins()
        {
            var ids = getAdminIds();
            int admins = ids.Count;

            string namecase;
            string admins_str = admins.ToString();
            switch(admins_str[admins_str.Length - 1])
            {
                case '2': case '3': case '4':
                    namecase = "человека";break;
                default:
                    namecase = "человек"; break;
            }
            if (admins > 10 && admins < 20)
                namecase = "человек";

            this.SendClientMessage(Colors.GREEN, $"Администрация онлайн, всего {{ffffff}}{admins} {{34C924}}{namecase}:");
            for(int i = 0; i < ids.Count; i++)
            {
                var p = Find(ids[i]);
                bool temp = p.PVars.Get<bool>(PvarsInfo.isTemp);
                if(temp)
                    this.SendClientMessage($"Временный администратор {{abcdef}}{p.Name} {{FFFFFF}}ID {{abcdef}}{ids[i]}");
                else
                    this.SendClientMessage($"Администратор {{fbec5d}}{p.PVars.Get<int>(PvarsInfo.adminlevel)} {{ffffff}}уровня {{abcdef}}{p.Name} {{FFFFFF}}ID {{abcdef}}{ids[i]}");
            }
        }
        [Command("a", PermissionChecker = typeof(AllAdminPermChecker), UsageMessage = "/a [Текст сообщения]")]
        private void CMD_a(string text)
        {
            var ids = getAdminIds();
            foreach(int id in ids)
            {
                Find(id).SendClientMessage($"{{ff9966}}[A] {Name}: {text}");
            }
        }
    }
}