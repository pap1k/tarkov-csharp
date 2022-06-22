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
        DATE,
        STATE
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
        private static bool banfunc(BasePlayer caller, Player target, int days, int type, string reason)
        {
            var Datenow = DateTime.Now;

            string ptype = BAN_TYPE.IdToString(type);
            if (ptype != "<ERR>")
            {
                //Починить кодировку
                GameMode.db.DoRequest("SET NAMES utf8");
                int uid = target.PVars.Get<int>(PvarsInfo.uid);
                GameMode.db.InsertBan(caller, uid, ptype, days, reason);
                GameMode.db.UpdatePlayerIsBanned(uid);
                //GameMode.db.DoRequest("SET NAMES cp1251");
                if (ptype == "ban")
                {
                    var d = new MessageDialog("{ffffff}Аккаунт заблокирован", $"{{be2626}}Доступ к аккаунту приостановлен за нарушения правил сервера.\nЕсли вы хотите обжаловать блокировку, обратитесь на форум {{ffffff}}НАЗВАНИЕ ФОРУМА {{be2626}}в соответствующий раздел.\n\n{{453dbf}}Ник аккаунта: {{ffffff}}{target.Name}\n{{453dbf}}Ник администратора: {{ffffff}}{caller.Name}\n{{453dbf}}Дата выдачи блокировки: {{ffffff}}{Datenow.ToString("dd.MM.yyyy t")}\n{{453dbf}}{(days == 0 ? "{453dbf}Аккаунт не подлежит разбану" : ($"Дата снятия блокировки:{{ffffff}} {Datenow.AddDays(days).ToString("dd.MM.yyyy t")}"))}\n{{453dbf}}Причина блокировки: {{ffffff}}{reason}", "X");
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
            else if(target.PVars.Get<bool>(PvarsInfo.isleaving))
                caller.SendClientMessage(Colors.GREY, "Этот игрок уже покидает сервер.");
            else
            {

                string savename = target.Name;
                bool res = banfunc(caller, target, days, 1, reason);
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
            else if (target.PVars.Get<bool>(PvarsInfo.isleaving))
                caller.SendClientMessage(Colors.GREY, "Этот игрок уже покидает сервер.");
            else
            {
                string savename = target.Name;
                bool res = banfunc(caller, target, days, 2, reason);
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
            //else if (target.Id == caller.Id)
            //    caller.SendClientMessage(Colors.GREY, "Вы не можете заблокировать самого себя.");
            else if (target.PVars.Get<bool>(PvarsInfo.isleaving))
                caller.SendClientMessage(Colors.GREY, "Этот игрок уже покидает сервер.");
            else
            {
                string savename = target.Name;
                bool res = banfunc(caller, target, days, 3, reason);
                if (res)
                {
                    foreach (var admin in BasePlayer.All.Where(p => (e_AdminLevels)p.PVars.Get<int>(PvarsInfo.adminlevel) >= e_AdminLevels.A_FOUNDER))
                        admin.SendClientMessage(Colors.RED, $"Администратор: {caller.Name} скрытно забанил {savename} {(days == 0 ? "навсегда" : $"на {days} дней")}. Причина: {reason}");
                        caller.SendClientMessage(Colors.GREY, $"Сообщение видно только тем, кто имеет доступ к скрытым блокировкам.");
                }
            }
        }
        [Command("unban", UsageMessage = "/unban [Полный ник игрового аккаунта]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_wban(BasePlayer caller, string nick)
        {
            var r = GameMode.db.SelectPlayerByNickname(nick).data;
            if (r.Count == 1)
            {
                if(int.Parse(r[0][(int)e_PlayerInfo.PINFO_ISBANNED]) == 1)
                {
                    int banneduid = int.Parse(r[0][(int)e_PlayerInfo.PINFO_UID]);
                    //Выбираем только активные баны (по идее такой должен быть только 1)
                    var baninfo = GameMode.db.SelectBanByUID(banneduid).data.Where(x => x[(int)e_BANINFO.STATE] == "1" && x[(int)e_BANINFO.TYPE].Contains("ban")).ToList();
                    if(baninfo.Count == 1)
                    {
                        if((e_AdminLevels)caller.PVars.Get<int>(PvarsInfo.adminlevel) >= e_AdminLevels.A_LEAD)
                        {
                            GameMode.db.UpdatePlayerIsBanned(banneduid, 0);
                            GameMode.db.UnbanPlayer(int.Parse(baninfo[0][(int)e_BANINFO.ID]));
                            foreach (var admin in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                                admin.SendClientMessage(Colors.RED, $"Администратор: {caller.Name} разбанил аккаунт {r[0][(int)e_PlayerInfo.PINFO_NICKNAME]}");
                        }
                        else
                        {
                            if(int.Parse(baninfo[0][(int)e_BANINFO.ADMINUID]) == caller.PVars.Get<int>(PvarsInfo.uid))
                            {
                                DateTime bandate = DateTime.Parse(baninfo[0][(int)e_BANINFO.DATE]);
                                DateTime cur = DateTime.Now;
                                int hourstowban = 48;
                                if(cur.AddHours(hourstowban) <= bandate)
                                {
                                    GameMode.db.UpdatePlayerIsBanned(banneduid, 0);
                                    GameMode.db.UnbanPlayer(int.Parse(baninfo[0][(int)e_BANINFO.ID]));
                                    foreach (var admin in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                                        admin.SendClientMessage(Colors.RED, $"Администратор: {caller.Name} разбанил аккаунт {r[0][(int)e_PlayerInfo.PINFO_NICKNAME]}.");
                                }
                                else
                                    caller.SendClientMessage(Colors.GREY, $"После блокировки указанного аккаунта прошло более {hourstowban} часов.");
                            }
                            else
                                caller.SendClientMessage(Colors.GREY, "Указанный аккаунт заблокирован не вами.");
                        }
                    }
                    else
                        caller.SendClientMessage(Colors.GREY, "Неизвестная ошибка базы данных. Сообщите создателю ник аккаунта.");
                }
                else
                    caller.SendClientMessage(Colors.GREY, "Указанный аккаунт не забанен.");
            }
            else
                caller.SendClientMessage(Colors.GREY, "Не найден такой аккаунт.");
        }
        [Command("kick", UsageMessage = "/kick [ID или часть ника] [Причина кика]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void cmd_kick(BasePlayer caller, Player target, string reason)
        {
            if (!target.PVars.Get<bool>(PvarsInfo.isleaving))
            {
                    Player.SendClientMessageToAll(Colors.RED, $"Администратор: {caller.Name} кикнул {target.Name}. Причина: {reason}");
                target.kick("Кикнут администратором");
            }
            else
                caller.SendClientMessage(Colors.GREY, "Этот игрок уже покидает сервер.");
        }
        [Command("akick", UsageMessage = "/akick [ID или часть ника] [Причина кика]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void cmd_akick(BasePlayer caller, Player target, string reason)
        {
            if (!target.PVars.Get<bool>(PvarsInfo.isleaving))
            {
                foreach (var admin in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                    admin.SendClientMessage(Colors.RED, $"Администратор: {caller.Name} тихо кикнул {target.Name}. Причина: {reason}");
                target.SendClientMessage(Colors.RED, $"A: Вы были кикнуты с сервера. Причина: {reason}");
                target.kick("Кикнут администратором");
            }
            else
                caller.SendClientMessage(Colors.GREY, "Этот игрок уже покидает сервер.");
        }
        [Command("skick", UsageMessage = "/skick [ID или часть ника]", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void cmd_skick(BasePlayer caller, Player target)
        {
            if (!target.PVars.Get<bool>(PvarsInfo.isleaving))
            {
                    caller.SendClientMessage(Colors.RED, $"Игрок {target.Name} кикнут.");
                    target.kick("Kicked/Banned");
            }
            else
                caller.SendClientMessage(Colors.GREY, "Этот игрок уже покидает сервер.");
        }
    }
}