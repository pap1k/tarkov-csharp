using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using System;
using System.Text;

namespace SampSharpGamemode
{
    [PooledType]
    public class Player : BasePlayer
    {
        public string password;

        public int  uid,
                    score,
                    helplevel,
                    skin,
                    money,
                    sessionid;

        public bool spawned,
                    ingame,
                    admin,
                    helper;
        public e_AdminLevels admlevel;
        public Inventary inventary;
        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);

            for(int i = 0; i < 10; i++)
                SendClientMessage("");
            LoadInfo();
        }
        public void LoadInfo()
        {
            var dbinfo = GameMode.db.SelectPlayerByNickname(this.Name).data;
            if(dbinfo.Count != 0)
            {
                //auth
                
            }
            else
            {
                //register
                RegisterationSystem.Start(this);
            }
        }
    }
}