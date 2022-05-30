using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharpGamemode;
using SampSharpGamemode.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampSharpGameMode.Chats
{
    internal class LocalChats
    {
        [Command("me", UsageMessage = "/me [Описание действия]")]
        private static void CMD_me(BasePlayer sender, string text)
        {
            var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 7 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
            if (sender.PVars.Get<bool>(PvarsInfo.ingame))
            {
                foreach (var p in near)
                    p.SendClientMessage(Colors.ME, $"* {sender.Name} " + text);
                sender.SetChatBubble(text, Colors.ME, 10f, 1000);
            }
            else return;
        }
        [Command("ame", UsageMessage = "/ame [Описание действия]")]
        private static void CMD_ame(BasePlayer sender, string text)
        {
            var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 7 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
            if (sender.PVars.Get<bool>(PvarsInfo.ingame))
            {
                foreach (var p in near)
                    sender.SendClientMessage(Colors.ME, $"! {sender.Name} " + text);
                sender.SetChatBubble(text, Colors.ME, 10f, 1000);
            }
            else return;
        }
        [Command("b", UsageMessage = "/b [Текст сообщения]")]
        private static void CMD_b(BasePlayer sender, string text)
        {
            var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 10 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
            if (sender.PVars.Get<bool>(PvarsInfo.ingame))
            {
                foreach (var p in near)
                    sender.SendClientMessage(-1, $"(( {sender.Name}: {text} ))");
            }
            else return;
        }
        [Command("do", UsageMessage = "/do [Текст сообщения]")]
        private static void CMD_do(BasePlayer sender, string text)
        {
            var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 7 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
            if (sender.PVars.Get<bool>(PvarsInfo.ingame))
            {
                foreach (var p in near)
                    sender.SendClientMessage(Colors.ME, $"{text} (({sender.Name}))");
            }
            else return;
        }
        [Command("s", UsageMessage = "/s [Текст сообщения]")]
        private static void CMD_s(BasePlayer sender, string text)
        {
            var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 30 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
            if (sender.PVars.Get<bool>(PvarsInfo.ingame))
            {
                foreach (var p in near)
                    sender.SendClientMessage(Colors.S, $"{sender.Name} крикнул: {text}");
            }
            else return;
        }
    }
}
