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

    class LocalChats
    {
        private static void SendTOAllInRange(int range, BasePlayer from, string message, uint color = 0xffffffff)
        {
            var near = BasePlayer.All.Where(p => (from.GetDistanceFromPoint(p.Position) <= 100 && from.VirtualWorld == p.VirtualWorld && from.Interior == p.Interior));
            foreach (var p in near)
                p.SendClientMessage(color, message);
        }
        [Command("as", UsageMessage = "/as [����� ���������]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_as(BasePlayer sender, string text)
        {
            SendTOAllInRange(100, sender, $"������������� {sender.Name} �������: " + text, 0xff0000ff);
        }
        [Command("me", UsageMessage = "/me [�������� ��������]")]
        private static void CMD_me(BasePlayer sender, string text)
        {
            SendTOAllInRange(7, sender, $"* {sender.Name} " + text, Colors.ME);
            sender.SetChatBubble(text, Colors.ME, 10f, 7_000);
        }
        [Command("ame", UsageMessage = "/ame [�������� ��������]")]
        private static void CMD_ame(BasePlayer sender, string text)
        {
            sender.SetChatBubble(text, Colors.ME, 10f, 7000);
            sender.SendClientMessage(Colors.ME, $"! {sender.Name} " + text);
        }
        [Command("b", UsageMessage = "/b [����� ���������]")]
        private static void CMD_b(BasePlayer sender, string text)
        {
            SendTOAllInRange(10, sender, $"(( {sender.Name}: {text} ))");
        }
        [Command("do", UsageMessage = "/do [����� ���������]")]
        private static void CMD_do(BasePlayer sender, string text)
        {
            SendTOAllInRange(10, sender, $"{text} (( {sender.Name} ))", Colors.ME);
        }
        [Command("s", UsageMessage = "/s [����� ���������]")]
        private static void CMD_s(BasePlayer sender, string text)
        {
            sender.ApplyAnimation("ON_LOOKERS", "SHOUT_01", 4.1f, false, true, true, false, 2000);
            SendTOAllInRange(30, sender, $"{sender.Name} �������: {text}", Colors.S);
        }
        [Command("cdo", UsageMessage = "/cdo [�������� ��������*����� ���������]")]
        private static void CMD_cdo(BasePlayer sender, string text)
        {
            string[] info = text.Split("*");
            if (info.Length == 2)
            {
                SendTOAllInRange(10, sender, $"{info[0]}, {sender.Name} ������: {info[1]}", Colors.CHAT);
            }
            else
            {
                sender.SendClientMessage(Colors.GREY, "/cdo [�������� ��������*����� ���������]");
            }
        }
        [Command("sdo", UsageMessage = "/sdo [�������� ��������*����� ���������]")]
        private static void CMD_sdo(BasePlayer sender, string text)
        {
            string[] info = text.Split("*");
            if (info.Length == 2)
            {
                SendTOAllInRange(30, sender, $"{info[0]}, {sender.Name} �������: {info[1]}", Colors.S);
            }
            else
            {
                sender.SendClientMessage(Colors.GREY, "/sdo [�������� ��������*����� ���������]");
            }
        }
        [Command("try", UsageMessage = "/try [�������� ��������]")]
        private static void CMD_try(BasePlayer sender, string text)
        {
            string kak = "";
            Random r = new Random();
            kak = r.Next(0, 11) > 5 ? "������" : "��������";
            SendTOAllInRange(10, sender, $"* * {sender.Name} {text} ({kak})", Colors.ME);
        }
        //test
    }
}