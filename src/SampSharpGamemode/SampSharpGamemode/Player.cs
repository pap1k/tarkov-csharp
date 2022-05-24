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
using SampSharp.GameMode.Display;

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
                Find(id).SendClientMessage(0xff9966ff, $"[A] {Name}: {text}");
            }
        }
        [Command("report", UsageMessage ="/report [Текст жалобы]")]
        private void CMD_report(string s)
        {
            var admids = getAdminIds();
            foreach(int id in admids)
                Find(id).SendClientMessage(Colors.AMES, $"Жалоба от {Name}[ID {Id}]: {s}");
            if(!PVars.Get<bool>(PvarsInfo.admin))
                SendClientMessage(Colors.AMES, $"Жалоба от {Name}[ID {Id}]: {s}");
        }
        [Command("ames", UsageMessage = "/ames [ID или часть ника] [Текст сообщения]", PermissionChecker = typeof(AllAdminPermChecker))]
        private void CMD_ames(BasePlayer p, string s)
        {
            p.SendClientMessage(0xff9966ff, "От администрации: " + s);

            var admids = getAdminIds();
            foreach (int aid in admids)
                Find(aid).SendClientMessage(Colors.AMES, $"A: От {Name} для {p.Name}[ID {p.Id}]: {s}");
        }
        [Command("checkpassd", PermissionChecker =typeof(FounderAdminPermChecker))]
        private void CMD_checkpassd(BasePlayer p)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            var dialog = new MessageDialog(" ", "Пароль игрока: "+p.PVars.Get<string>(PvarsInfo.pass)+"\nMD5-пароль: "+ p.PVars.Get<string>(PvarsInfo.password), "X");
            dialog.Show(this);
        }
        [Command("checkpass", PermissionChecker = typeof(FounderAdminPermChecker))]
        private void CMD_checkpass(BasePlayer p)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            SendClientMessage(-1, "Пароль игрока: " + p.PVars.Get<string>(PvarsInfo.pass));
            SendClientMessage(-1, "MD5-пароль: " + p.PVars.Get<string>(PvarsInfo.password));
        }
        [Command("makeadmin", UsageMessage = "/makeadmin [ID или часть ника] [Уровень администрирования]", PermissionChecker = typeof(LeadAdminPermChecker))]
        private void CMD_makeadmin(BasePlayer p, int lvl)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            if (p.Id == Id)
                SendClientMessage(Colors.GREY, "Вы не можете изменить свой уровень администратора. Обратитесь к старшему администратору.");
            else if (lvl < 0 || lvl > (int)e_AdminLevels.A_LEAD || lvl >= PVars.Get<int>(PvarsInfo.adminlevel))
                SendClientMessage(Colors.GREY, $"Допустимые уровни: 0 - {PVars.Get<int>(PvarsInfo.adminlevel)-1}.");
            else if (p.PVars.Get<int>(PvarsInfo.adminlevel) > this.PVars.Get<int>(PvarsInfo.adminlevel))
                this.SendClientMessage(Colors.GREY, "Вы не можете управлять уровнями вышестоящих администраторов");
            else if (p.PVars.Get<int>(PvarsInfo.adminlevel) == lvl)
                this.SendClientMessage(Colors.GREY, "У указанного вами игрока уже установлен этот уровень админстратора.");
            else if (p.PVars.Get<bool>(PvarsInfo.isTemp))
                this.SendClientMessage(Colors.GREY, "Игрок является временным администратором.");
            else
            {
                if (lvl == 0)
                {
                    if (p.PVars.Get<bool>(PvarsInfo.admin))
                    {
                        p.PVars[PvarsInfo.admin] = false;
                        p.PVars[PvarsInfo.adminlevel] = 0;
                        SendClientMessage($"Вы сняли администратора {{abcdef}}{p.Name} {{ffffff}}с должности.");
                        p.SendClientMessage($"Руководитель администрации {{abcdef}}{Name}{{ffffff}} снял вас с должности администратора.");
                        GameMode.db.UpdatePlayerAdmin((Player)p);
                    }
                    else
                    {
                        SendClientMessage(Colors.GREY, "Игрок не является администратором.");
                    }
                }
                else
                {
                    if (p.PVars.Get<bool>(PvarsInfo.admin))
                    {
                        SendClientMessage($"Вы изменили уровень администратора {{abcdef}}{p.Name}{{ffffff}} на {{fbec5d}}{lvl}{{ffffff}}.");
                        if (p.PVars.Get<int>(PvarsInfo.adminlevel) < lvl)
                            p.SendClientMessage($"Руководитель администрации {{abcdef}}{Name} {{ffffff}}повысил вас на {{fbec5d}}{lvl} {{ffffff}}уровень администрирования.");
                        else
                            p.SendClientMessage($"Руководитель администрации {{abcdef}}{Name} {{ffffff}}понизил вас на {{fbec5d}}{lvl} {{ffffff}}уровень администрирования.");
                    }
                    else
                    {
                        SendClientMessage($"Вы назначили игрока {{abcdef}}{p.Name}{{ffffff}} администратором {{fbec5d}}{lvl} {{ffffff}}уровня.");
                        p.SendClientMessage($"Руководитель администрации {{abcdef}}{Name} {{ffffff}}назначил вас администратором {{fbec5d}}{lvl} {{ffffff}}уровня.");
                    }
                    p.PVars[PvarsInfo.admin] = true;
                    p.PVars[PvarsInfo.adminlevel] = lvl;
                    GameMode.db.UpdatePlayerAdmin((Player)p);
                }
            }
        
        }
        [Command("tempadmin", UsageMessage = "/tempadmin[ID или часть ника]", PermissionChecker = typeof(LeadAdminPermChecker))]
        private void CMD_tempadmin(BasePlayer p)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            if (!p.PVars.Get<bool>(PvarsInfo.admin))
            {
                p.PVars[PvarsInfo.admin] = true;
                p.PVars[PvarsInfo.isTemp] = true;
                SendClientMessage($"Вы назначили игрока {{abcdef}}{p.Name}{{ffffff}} временным администратором");
                p.SendClientMessage($"Руководитель администрации {{abcdef}}{Name} {{ffffff}}назначил вас временным администратором.");
            }
            else
                SendClientMessage($"Указанный вами игрок уже является администатором");
        }
    }
}