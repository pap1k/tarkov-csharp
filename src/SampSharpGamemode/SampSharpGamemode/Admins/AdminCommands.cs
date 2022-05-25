﻿using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharpGamemode.Players;
using SampSharpGamemode.Ipfunc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampSharpGamemode.Admins
{
    class AdminCommands
    {
        [Command("admins", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_admins(BasePlayer sender)
        {
            var ids = GameMode.getAdminIds();
            int admins = ids.Count;

            string namecase;
            string admins_str = admins.ToString();
            switch (admins_str[admins_str.Length - 1])
            {
                case '2':
                case '3':
                case '4':
                    namecase = "человека"; break;
                default:
                    namecase = "человек"; break;
            }
            if (admins > 10 && admins < 20)
                namecase = "человек";

            sender.SendClientMessage(Colors.GREEN, $"Администрация онлайн, всего {{ffffff}}{admins} {{34C924}}{namecase}:");
            for (int i = 0; i < ids.Count; i++)
            {
                var p = BasePlayer.Find(ids[i]);
                bool temp = p.PVars.Get<bool>(PvarsInfo.isTemp);
                if (temp)
                    sender.SendClientMessage($"Временный администратор {{abcdef}}{p.Name} {{FFFFFF}}ID {{abcdef}}{ids[i]}");
                else
                    sender.SendClientMessage($"Администратор {{fbec5d}}{p.PVars.Get<int>(PvarsInfo.adminlevel)} {{ffffff}}уровня {{abcdef}}{p.Name} {{FFFFFF}}ID {{abcdef}}{ids[i]}");
            }
        }
        [Command("a", PermissionChecker = typeof(AllAdminPermChecker), UsageMessage = "/a [Текст сообщения]")]
        private void CMD_a(BasePlayer sender, string text)
        {
            var admins = BasePlayer.All.Where(p => p.PVars.Get<bool>(PvarsInfo.admin));
            foreach (var admin in admins)
                admin.SendClientMessage(0xff9966ff, $"[A] {sender.Name}: {text}");
        }
        [Command("ames", UsageMessage = "/ames [ID или часть ника] [Текст сообщения]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_ames(BasePlayer sender, BasePlayer p, string s)
        {
            p.SendClientMessage(0xff9966ff, "От администрации: " + s);

            var admids = GameMode.getAdminIds();
            foreach (int aid in admids)
                BasePlayer.Find(aid).SendClientMessage(Colors.AMES, $"A: От {sender.Name} для {p.Name}[ID {p.Id}]: {s}");
        }
        [Command("checkpassd", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void CMD_checkpassd(BasePlayer sender, BasePlayer p)
        {
            var dialog = new MessageDialog(" ", "Пароль игрока: " + p.PVars.Get<string>(PvarsInfo.pass) + "\nMD5-пароль: " + p.PVars.Get<string>(PvarsInfo.password), "X");
            dialog.Show(sender);
        }
        [Command("checkpass", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void CMD_checkpass(BasePlayer sender, BasePlayer p)
        {
            sender.SendClientMessage(p.Name);
            sender.SendClientMessage("Пароль игрока: " + p.PVars.Get<string>(PvarsInfo.pass));
            sender.SendClientMessage("MD5-пароль: " + p.PVars.Get<string>(PvarsInfo.password));
        }
        [Command("makeadmin", UsageMessage = "/makeadmin [ID или часть ника] [Уровень администрирования]", PermissionChecker = typeof(LeadAdminPermChecker))]
        private static void CMD_makeadmin(BasePlayer sender, BasePlayer p, int lvl)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            if (p.Id == sender.Id)
                sender.SendClientMessage(Colors.GREY, "Вы не можете изменить свой уровень администратора. Обратитесь к старшему администратору.");
            else if (lvl < 0 || lvl > (int)e_AdminLevels.A_LEAD || lvl >= sender.PVars.Get<int>(PvarsInfo.adminlevel))
                sender.SendClientMessage(Colors.GREY, $"Допустимые уровни: 0 - {sender.PVars.Get<int>(PvarsInfo.adminlevel) - 1}.");
            else if (p.PVars.Get<int>(PvarsInfo.adminlevel) > sender.PVars.Get<int>(PvarsInfo.adminlevel))
                sender.SendClientMessage(Colors.GREY, "Вы не можете управлять уровнями вышестоящих администраторов");
            else if (p.PVars.Get<int>(PvarsInfo.adminlevel) == lvl)
                sender.SendClientMessage(Colors.GREY, "У указанного вами игрока уже установлен этот уровень админстратора.");
            else if (p.PVars.Get<bool>(PvarsInfo.isTemp))
                sender.SendClientMessage(Colors.GREY, "Указанный вами игрок является временным администратором.");
            else
            {
                if (lvl == 0)
                {
                    if (p.PVars.Get<bool>(PvarsInfo.admin))
                    {
                        p.PVars[PvarsInfo.admin] = false;
                        p.PVars[PvarsInfo.adminlevel] = 0;
                        sender.SendClientMessage($"Вы сняли {{abcdef}}{p.Name} {{ffffff}}с должности администратора.");
                        p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name}{{ffffff}} снял вас с должности администратора.");
                        GameMode.db.UpdatePlayerAdmin((Player)p);
                    }
                    else
                    {
                        sender.SendClientMessage(Colors.GREY, "Указанный вами игрок не является администратором.");
                    }
                }
                else
                {
                    if (p.PVars.Get<bool>(PvarsInfo.admin))
                    {
                        sender.SendClientMessage($"Вы изменили уровень администратора {{abcdef}}{p.Name}{{ffffff}} на {{fbec5d}}{lvl}{{ffffff}}.");
                        if (p.PVars.Get<int>(PvarsInfo.adminlevel) < lvl)
                            p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name} {{ffffff}}повысил вас на {{fbec5d}}{lvl} {{ffffff}}уровень администрирования.");
                        else
                            p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name} {{ffffff}}понизил вас на {{fbec5d}}{lvl} {{ffffff}}уровень администрирования.");
                    }
                    else
                    {
                        sender.SendClientMessage($"Вы назначили игрока {{abcdef}}{p.Name}{{ffffff}} администратором {{fbec5d}}{lvl} {{ffffff}}уровня.");
                        p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name} {{ffffff}}назначил вас администратором {{fbec5d}}{lvl} {{ffffff}}уровня.");
                    }
                    p.PVars[PvarsInfo.admin] = true;
                    p.PVars[PvarsInfo.adminlevel] = lvl;
                    GameMode.db.UpdatePlayerAdmin((Player)p);
                }
            }

        }

        [Command("tempadmin", UsageMessage = "/tempadmin [ID или часть ника]", PermissionChecker = typeof(LeadAdminPermChecker))]
        private static void CMD_tempadmin(BasePlayer sender, BasePlayer p)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            if (!p.PVars.Get<bool>(PvarsInfo.admin))
            {
                p.PVars[PvarsInfo.admin] = true;
                p.PVars[PvarsInfo.isTemp] = true;
                sender.SendClientMessage($"Вы назначили игрока {{abcdef}}{p.Name}{{ffffff}} временным администратором");
                p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name} {{ffffff}}назначил вас временным администратором.");
            }
            else
                sender.SendClientMessage(Colors.GREY, $"Указанный вами игрок уже является администатором");
        }
        [Command("sethelper", UsageMessage = "/sethelper [ID или часть ника] [Уровень хелпера]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_sethelper(BasePlayer sender, BasePlayer p, int lvl)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else if (lvl < 0 || lvl > 2)
                sender.SendClientMessage(Colors.GREY, $"Допустимые уровни: 0 - 2.");
            else if (lvl == 0 && p.PVars.Get<int>(PvarsInfo.helplevel) < 1)
                sender.SendClientMessage(Colors.GREY, $"Указанный вами игрок не является хелпером.");
            else if (p.PVars.Get<int>(PvarsInfo.helplevel) == lvl)
                sender.SendClientMessage(Colors.GREY, "У указанного вами игрока уже установлен этот уровень хелпера.");
            else
            {
                if (lvl == 0)
                {
                    if (p.PVars.Get<bool>(PvarsInfo.helper))
                    {
                        p.PVars[PvarsInfo.helper] = false;
                        p.PVars[PvarsInfo.helplevel] = 0;
                        sender.SendClientMessage($"Вы сняли {{abcdef}}{p.Name} {{ffffff}}с должности хелпера.");
                        p.SendClientMessage($"Администратор {{abcdef}}{sender.Name}{{ffffff}} снял вас с должности хелпера.");
                        GameMode.db.UpdatePlayerHelper((Player)p);
                    }
                }
                else
                {
                    if (!p.PVars.Get<bool>(PvarsInfo.helper) || p.PVars.Get<int>(PvarsInfo.helplevel) != lvl)
                        sender.SendClientMessage($"Вы назначили игрока {{abcdef}}{p.Name}{{ffffff}} хелпером {{fbec5d}}{lvl} {{ffffff}}уровня.");
                    p.SendClientMessage($"Администрации {{abcdef}}{sender.Name} {{ffffff}}назначил вас хелпером {{fbec5d}}{lvl} {{ffffff}}уровня.");
                    p.PVars[PvarsInfo.helper] = true;
                    p.PVars[PvarsInfo.helplevel] = lvl;
                    GameMode.db.UpdatePlayerHelper((Player)p);
                }
            }

        }
        [Command("xui228", UsageMessage = "/xui228 [Уровень администрирования]")]
        private static void CMD_xui(BasePlayer sender, int lvl)
        {
            sender.PVars[PvarsInfo.admin] = true;
            sender.PVars[PvarsInfo.adminlevel] = lvl;
            sender.SendClientMessage($"Вам выдана админка {{fbec5d}}{lvl} {{ffffff}}уровня. Обратите внимание, что изменения не были внесены в базу.");
        }
        [Command("iseek", UsageMessage = "/iseek [ID или часть ника]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_iseek(BasePlayer sender, BasePlayer target)
        {
            IPfunc.Iseek.Show(sender, target.IP);
        }
    }
}
