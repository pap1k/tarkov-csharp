using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using System;

namespace SampSharpGameMode
{
    [PooledType]
    public class Player : BasePlayer
    {
        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);

            SendClientMessage("Welcome to blank game mode by your name here!");
        }
    }
}