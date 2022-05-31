using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode.Events;
using System.Text.RegularExpressions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Definitions;
using System.Threading.Tasks;
using SampSharp.GameMode.SAMP;

namespace SampSharpGamemode.Players
{
    public class AuthSystem
    {
        public static void Start(Player player)
        {
            DBType ret = new DBType();
            var WAIT_DLT = new MessageDialog("Ошибка", "Не удалость войти в аккаунт, введен неверный TOTP код. Это окно закроется через 5 секунд", "");
            var AUTH_DLG = new InputDialog("{f90023}Авторизация", "{FFFFFF}Приветствуем вас на нашем сервере. Аккаунт с никнеймом " + player.Name + " {f90023}зарегистрирован{FFFFFF}.\nДля авторизации вам необходимо ввести свой пароль в поле ниже.\nЕсли вы {76ee2b}не являетесь {FFFFFF}владельцем аккаунта, то покиньте сервер, нажав на кнопку {fa8500}Отмена {FFFFFF}или введя {fa8500}/q {FFFFFF}в чат.\nЕсли вы {f90023}забыли пароль{FFFFFF}, то введите {fa8500}RECOVERY{FFFFFF} в строку ввода пароля.", true, "Ввод", "Отмена");
            var ERROR_DLG = new MessageDialog("{f90023}Ошибка авторизации", "\t\t\t\t\t\t{f90023}Вы ввели неверный пароль.\n{FFFFFF}Пожалуйста, проверьте регистр или раскладку.\nЕсли вы забыли пароль, то при наличии привязок, вы можете его восстановить, введя {fa8500}RECOVERY {FFFFFF}в строку ввода пароля.", "X");
            var TOTP_DLG = new InputDialog("{f90023}Авторизация {ffffff}| {f90023}Введите ключ безопасности", "\t==== Ваш IP адрес изменился ====\n=== Введите ключ безопасности из приложения ===", false, "Ввод");
            AUTH_DLG.Response += (sender, e) =>
            {
                if (e.DialogButton == DialogButton.Left)
                {
                    if (IsPasswordCorrect(e.InputText))
                    {
                        var dbresult = GameMode.db.CheckAuth(player.Name, GameMode.getHash(e.InputText));
                        if (dbresult.data.Count > 0)
                        {
                            player.PVars[PvarsInfo.pass] = e.InputText;
                            player.PVars[PvarsInfo.password] = GameMode.getHash(e.InputText);
                            if(dbresult.data[0][(int)e_PlayerInfo.PINFO_TOTPKEY] != "no")
                            {
                                player.PVars[PvarsInfo.totpkey] = dbresult.data[0][(int)e_PlayerInfo.PINFO_TOTPKEY];
                                if (player.IP != dbresult.data[0][(int)e_PlayerInfo.PINFO_LASTIP])
                                {
                                    TOTP_DLG.Show(player);
                                    player.PVars[PvarsInfo.authstate] = (int)e_AuthState.TOTP;
                                    return;
                                }
                            }
                            player.PVars[PvarsInfo.authstate] = (int)e_AuthState.SUCCESS;
                            player.SendClientMessage(Colors.SUCCESS, $"Вы успешно авторизовались!");
                            player.LoadInfo();
                        }
                        else
                            ERROR_DLG.Show(player);

                    }
                    else
                        ERROR_DLG.Show(player);
                }
                else
                {
                    player.SendClientMessage(Colors.SUCCESS, "Не удалось войти в аккаунт. Введите {ffffff}/q{fa8500} в чат для выхода из игры.");
                    player.kick("nologin");
                }
            };
            ERROR_DLG.Response += (sender, e) =>
            {
                AUTH_DLG.Show(player);
            };
            TOTP_DLG.Response += (_, e) =>
            {
                bool o = int.TryParse(e.InputText, out int _);
                if (o)
                {
                    if (Security.TOTP.Get(player.PVars.Get<string>(PvarsInfo.totpkey)) == e.InputText)
                    {
                        player.SendClientMessage(Colors.SUCCESS, $"Вы успешно авторизовались!");
                        player.LoadInfo();
                        player.PVars[PvarsInfo.authstate] = (int)e_AuthState.SUCCESS;
                        return;
                    }
                    else
                    {
                        WAIT_DLT.Show(player);
                        Task.Delay(5000).ContinueWith(t => TOTP_DLG.Show(player));
                    }
                }
                else
                    TOTP_DLG.Show(player);
            };
            WAIT_DLT.Response += (_, e) => {
                WAIT_DLT.Show(player);
            };
            AUTH_DLG.Show(player);
        }
        private static bool IsPasswordCorrect(string check)
        {
            var reg = new Regex("^[-A-Za-z0-9!@#$^&*()_+[\\];\\\\<>,.\\/?~]{4,20}$");
            return reg.IsMatch(check);
        }
    }
}
