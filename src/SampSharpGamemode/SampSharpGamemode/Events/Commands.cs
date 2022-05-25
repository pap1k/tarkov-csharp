using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.ParameterTypes;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using SampSharpGamemode.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampSharpGamemode.Events
{
    class Commands
    {
        [Command("aworld", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_forcetotp(BasePlayer sender)
        {
            sender.Position = new Vector3(1142.9049f, 1357.7727f, 10.8203f);
            sender.VirtualWorld = (int)VW.EVENT;
            sender.SendClientMessage(Colors.GREY, "Вы были телепортированы");
        }
        [Command("esay", PermissionChecker = typeof(EventPermChecker))]
        private static void CMD_esay(BasePlayer sender, string s)
        {
            foreach (var p in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.ingame)))
                p.SendClientMessage("Event Admin: " + s);
        }
    }
}
