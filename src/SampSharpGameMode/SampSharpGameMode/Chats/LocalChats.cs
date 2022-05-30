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
        [Command("as", UsageMessage = "/as [Текст сообщения]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_as(BasePlayer sender, string text)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else
            {
                var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 100 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
                foreach (var p in near)
                    p.SendClientMessage(0xff0000ff, $"Администратор {sender.Name} крикнул: " + text);
            }
        }
        [Command("me", UsageMessage = "/me [Описание действия]")]
        private static void CMD_me(BasePlayer sender, string text)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else
            {
                var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 10 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
                foreach (var p in near)
                    p.SendClientMessage(Colors.ME, $"* {sender.Name} " + text);
                sender.SetChatBubble(text, Colors.ME, 10f, 1000);
            }
        }
        [Command("ame", UsageMessage = "/ame [Описание действия]")]
        private static void CMD_ame(BasePlayer sender, string text)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else
            {
                var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 10 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
                foreach (var p in near)
                    sender.SendClientMessage(Colors.ME, $"! {sender.Name} " + text);
                sender.SetChatBubble(text, Colors.ME, 10f, 1000);
            }
        }
        [Command("b", UsageMessage = "/b [Текст сообщения]")]
        private static void CMD_b(BasePlayer sender, string text)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else
            {
                var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 10 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
                foreach (var p in near)
                    p.SendClientMessage(-1, $"(( {sender.Name}: {text} ))");
            }
        }
        [Command("do", UsageMessage = "/do [Текст сообщения]")]
        private static void CMD_do(BasePlayer sender, string text)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else
            {
                var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 10 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
                foreach (var p in near)
                    p.SendClientMessage(Colors.ME, $"{text} (( {sender.Name} ))");
            }
        }
        [Command("s", UsageMessage = "/s [Текст сообщения]")]
        private static void CMD_s(BasePlayer sender, string text)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else
            {
                var near = BasePlayer.All.Where(p => (sender.GetDistanceFromPoint(p.Position) <= 10 && sender.VirtualWorld == p.VirtualWorld && sender.Interior == p.Interior));
                foreach (var p in near)
                    p.SendClientMessage(Colors.S, $"{sender.Name} крикнул: {text}");
                sender.ApplyAnimation("ON_LOOKERS", "SHOUT_01", 4.1f, false, true, true, false, 2000);
            }
        }
    }
}
