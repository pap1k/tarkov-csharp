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
            Console.WriteLine("1234");
            //LoadInfo();
            //base.OnConnected(e);
        }
        public void LoadInfo()
        {
            var dbinfo = GameMode.db.SelectPlayerByNickname(this.Name).data;
            if(dbinfo.Count != 0)
            {
                //login
                AuthSystem.Start(this);
            }
            else
            {
                //register
                RegisterationSystem.Start(this);
            }
        }
    }
}