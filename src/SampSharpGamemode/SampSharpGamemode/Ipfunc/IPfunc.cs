using SampSharp.GameMode.Display;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Definitions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SampSharpGamemode.Ipfunc
{
    public enum e_IP
    {
        invalid_session = 0,
        log_left = 1,
        log_nlban = 2,
        log_banconst = 3,
        log_bantmp = 4,
        log_warn = 5,
        log_nlleft = 6
    };

    class IPfunc
    {
        static MessageDialog dialog = new MessageDialog(" ", " ", "X");
        static MessageDialog waitdialog = new MessageDialog(" ", "Ожидание ответа от БД", "");
        internal static class Iseek
        {
            public static void Show(BasePlayer player, string ip, int n = 0)
            {
                waitdialog.Show(player);
                dialog.Caption = "Информация о сессиях с IP {ffffff}" + ip;
                var db = GameMode.db.SelectIPSesstions(ip, n).data;
                string s = "";
                foreach (var row in db)
                {
                    s += row[2] + '\t' + row[3] + '\t' + row[4] + '\t' + ((e_IP)int.Parse(row[5])).ToString() + '\t' + row[6] +'\t' + (row.Count == 8 ? row[7] : " ") + '\n';
                }
                dialog.Message = s;
                dialog.Show(player);
            }
        }
        public static void Show(BasePlayer player)
        {

        }
    }
}
