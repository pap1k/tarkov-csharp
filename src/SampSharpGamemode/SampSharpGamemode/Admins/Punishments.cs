using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharpGamemode.Players;
using SampSharpGamemode.Ipfunc;
using SampSharp.GameMode.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampSharpGamemode.Admins
{
    public enum e_BANINFO
    {
        ID = 0,
        TYPE,
        PLAYERUID,
        ADMINUID,
        TERM,
        REASON,
        DATE
    }
    public class BAN_TYPE
    {
        public static int ban = 1, aban = 2, sban = 3, warn = 4, awarn = 5;
        public static string IdToString(int i)
        {
            if (i == 1)
                return "ban";
            else if (i == 2)
                return "aban";
            else if (i == 3)
                return "sban";
            else if (i == 4)
                return "warn";
            else if (i == 5)
                return "awarn";
            else return "<ERR>";
        }
    }
    class Punishments
    {
        private static bool func(BasePlayer caller, Player target, int days, int type, string reason)
        {
            var Datenow = DateTime.Now;

            string ptype = BAN_TYPE.IdToString(type);
            if (ptype != "<ERR>")
            {
                //Починить кодировку
                GameMode.db.InsertBan(caller, target.PVars.Get<int>(PvarsInfo.uid), ptype, days, reason);
                GameMode.db.UpdatePlayerIsBanned(target);
                if (ptype == "ban")
                {
                    var d = new MessageDialog("{ffffff}Аккаунт заблокирован", $"{{be2626}}Доступ к аккаунту приостановлен за нарушения правил сервера.\nЕсли вы хотите обжаловать блокировку, обратитесь на форум {{ffffff}}НАЗВАНИЕ ФОРУМА {{be2626}}в соответствующий раздел.\n\n{{453dbf}}Ник аккаунта: {{ffffff}}{target.Name}\n{{453dbf}}Ник администратора: {{ffffff}}{caller.Name}\n{{453dbf}}Дата выдачи блокировки: {{ffffff}}{Datenow.ToString("dd.MM.yyyy t")}\n{{453dbf}}{(days == 0 ? "{ffffff}Аккаунт не подлежит разбану" : ($"Дата снятия блокировки:{{ffffff}} {Datenow.AddDays(days).ToString("dd.MM.yyyy t")}"))}\n{{453dbf}}Причина блокировки: {{ffffff}}{reason}", "X");
                    d.Show(target);
                }
                else
                {
                    var d = new MessageDialog("{ffffff}Аккаунт заблокирован", $"{{be2626}}Доступ к аккаунту приостановлен за нарушения правил сервера.\nЕсли вы хотите обжаловать блокировку, обратитесь на форум {{ffffff}}НАЗВАНИЕ ФОРУМА {{be2626}}в соответствующий раздел.\n\n{{453dbf}}Ник аккаунта: {{ffffff}}{target.Name}\n{{453dbf}}Дата выдачи блокировки: {{ffffff}}{Datenow.ToString("dd.MM.yyyy t")}\n{{453dbf}}{(days == 0 ? "Аккаунт не подлежит разбану" : ($"Дата снятия блокировки: {{ffffff}}{Datenow.AddDays(days).ToString("dd.MM.yyyy t")}"))}\n{{453dbf}}Причина блокировки: {{ffffff}}{reason}", "X");
                    d.Show(target);
                }
                target.kick("Заблокирован " + (days == 0 ? "навсегда" : "на " + days + " дней"));
                return true;
            }
            else
            {
                caller.SendClientMessage(Colors.GREY, "Ошибка определения типа наказания. Проверьте введенные данные.");
                return false;
            }
        }
        [Command("ban", UsageMessage = "/ban [ID или чать ника] [Количество дней, 0 - навсегда] [Причина]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_ban(BasePlayer caller, Player target, int days, string reason)
        {
            if (caller.PVars.Get<bool>(PvarsInfo.isTemp))
                caller.SendClientMessage(Colors.GREY, "Вы являетесь временным администратором. Для выдачи наказания используйте /aban.");
            else if (target.PVars.Get<int>(PvarsInfo.adminlevel) > caller.PVars.Get<int>(PvarsInfo.adminlevel))
                caller.SendClientMessage(Colors.GREY, "Вы не можете наказывать вышестоящих администраторов.");
            else if (target.Id == caller.Id)
                caller.SendClientMessage(Colors.GREY, "Вы не можете заблокировать самого себя.");
            else
            {
                string savename = target.Name;
                bool res = func(caller, target, days, 1, reason);
                if (res)
                    foreach (var admin in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                        Player.SendClientMessageToAll(Colors.RED, $"Администратор: {caller.Name} забанил {savename} {(days == 0 ? "навсегда" : $"на {days} дней")}. Причина: {reason}");
            }
        }
        [Command("aban", UsageMessage = "/aban [ID или чать ника] [Количество дней, 0 - навсегда] [Причина]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_aban(BasePlayer caller, Player target, int days, string reason)
        {
            if (target.PVars.Get<int>(PvarsInfo.adminlevel) > caller.PVars.Get<int>(PvarsInfo.adminlevel))
                caller.SendClientMessage(Colors.GREY, "Вы не можете наказывать вышестоящих администраторов.");
            else if (target.Id == caller.Id)
                caller.SendClientMessage(Colors.GREY, "Вы не можете заблокировать самого себя.");
            else
            {
                string savename = target.Name;
                bool res = func(caller, target, days, 2, reason);
                if (res)
                    foreach (var admin in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                        admin.SendClientMessage(Colors.RED, $"Администратор: {caller.Name} тихо забанил {savename} {(days == 0 ? "навсегда" : $"на {days} дней")}. Причина: {reason}");
            }
        }
        [Command("sban", UsageMessage = "/sban [ID или чать ника] [Количество дней, 0 - навсегда] [Причина]", PermissionChecker = typeof(LeadAdminPermChecker))]
        private static void CMD_sban(BasePlayer caller, Player target, int days, string reason)
        {
            if (target.PVars.Get<int>(PvarsInfo.adminlevel) > caller.PVars.Get<int>(PvarsInfo.adminlevel))
                caller.SendClientMessage(Colors.GREY, "Вы не можете наказывать вышестоящих администраторов.");
            else if (target.Id == caller.Id)
                caller.SendClientMessage(Colors.GREY, "Вы не можете заблокировать самого себя.");
            else
            {
                string savename = target.Name;
                bool res = func(caller, target, days, 3, reason);
                if (res)
                    foreach (var admin in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                        caller.SendClientMessage(Colors.RED, $"Аккаунт {savename} заблокирован {(days == 0 ? "навсегда" : $"на {days} дней")}. Причина: {reason}");
                        caller.SendClientMessage(Colors.RED, $"[Примечание] В целях конфиденциальности сообщение с блокировкой видно только вам.");
            }
        }
    }
}