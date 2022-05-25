using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Command("hc", UsageMessage = "/hc [Текст сообщения]", PermissionChecker = typeof(HelperPermChecker))]
        private static void CMD_hc(BasePlayer sender, string text)
        {
            var helpers = BasePlayer.All.Where(p => p.PVars.Get<bool>(PvarsInfo.helper));
            foreach(var p in helpers)
                p.SendClientMessage(Colors.HELPER, $"[H] {sender.Name}: {text}");
        }
        [Command("ask", UsageMessage = "/ask [Текст сообщения]")]
        private static void CMD_ask(BasePlayer sender, string text)
        {
            var helpers = BasePlayer.All.Where(p => p.PVars.Get<bool>(PvarsInfo.helper));
            foreach (var player in helpers)
                player.SendClientMessage(Colors.HELPER, $"Вопрос от {sender.Name} ID {sender.Id}: {text}");
            if (!sender.PVars.Get<bool>(PvarsInfo.helper))
                sender.SendClientMessage(Colors.HELPER, $"Вопрос от {sender.Name} ID {sender.Id}: {text}");
        }
        [Command("answ", UsageMessage = "/answ [ID или часть ника] [Текст сообщения]", PermissionChecker = typeof(HelperPermChecker))]
        private static void CMD_answ(BasePlayer sender, BasePlayer p, string text)
        {
            p.SendClientMessage(Colors.HELPER, $"От {sender.Name} для {p.Name} ID {p.Id}: " + text);

            foreach(var helper in BasePlayer.All.Where(p => p.PVars.Get<bool>(PvarsInfo.helper)))
                helper.SendClientMessage(Colors.HELPER, $"Ответ от {sender.Name}: " + text);
        }
    }
}
