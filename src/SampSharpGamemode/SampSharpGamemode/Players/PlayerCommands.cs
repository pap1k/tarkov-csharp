using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharpGamemode.Players
{
    class PlayerCommands
    {
        [Command("inv", "stock")]
        private static void CMD_inv(Player caller)
        {
            Inventary.Show(caller);
        }
        [Command("report", UsageMessage = "/report [Текст жалобы]")]
        private static void CMD_report(BasePlayer caller, string s)
        {
            var admids = GameMode.getAdminIds();
            foreach (int id in admids)
                BasePlayer.Find(id).SendClientMessage(Colors.AMES, $"Жалоба от {caller.Name}[ID {caller.Id}]: {s}");
            if (!caller.PVars.Get<bool>(PvarsInfo.admin))
                caller.SendClientMessage(Colors.AMES, $"Жалоба от {caller.Name}[ID {caller.Id}]: {s}");
        }
    }
}
