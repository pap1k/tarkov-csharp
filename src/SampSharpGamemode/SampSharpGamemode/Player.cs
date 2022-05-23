using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using System;
using System.Text;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;

namespace SampSharpGamemode
{
    [PooledType]
    public class Player : BasePlayer
    {
        public Inventary inventary = new Inventary();
        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);
            Console.WriteLine("1234");
            SendClientMessage("Приятного аппетита!");
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
            var pinfo = GameMode.db.SelectPlayerByNickname(this.Name).data[0];
            PVars[PvarsInfo.uid] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_UID]);

            PVars[PvarsInfo.score] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_SCORE]);
            PVars[PvarsInfo.helplevel] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_HELPERLVL]);
            PVars[PvarsInfo.adminlevel] = (e_AdminLevels)int.Parse(pinfo[(int)e_PlayerInfo.PINFO_ADMINLVL]);
            PVars[PvarsInfo.skin] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_SKIN]);
            PVars[PvarsInfo.money] = int.Parse(pinfo[(int)e_PlayerInfo.PINFO_MONEY]);

            PVars[PvarsInfo.ingame] = true;
            PVars[PvarsInfo.helper] = PVars.Get<int>(PvarsInfo.helplevel) > 0;
            PVars[PvarsInfo.admin]  = PVars.Get<int>(PvarsInfo.adminlevel) > 0;

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
        }
        public void kick(string s)
        {
            //set reason
            base.Kick();
        }
        public override void OnRequestSpawn(RequestSpawnEventArgs e)
        {
            if(PVars.Get<bool>(PvarsInfo.ingame))
                base.OnRequestSpawn(e);
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
            for(int i = 0; i <= BasePlayer.Max; i++)
            {
                var p = BasePlayer.Find(i);
                if (p != null && p.IsConnected)
                {
                    this.SendClientMessage(p.PVars.Get<bool>("admin").ToString());
                }
            }
        }
    }
}