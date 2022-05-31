using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using SampSharp.GameMode.World;
using SampSharpGamemode.Players;

namespace SampSharpGamemode
{
    public class DBType
    {
        public List<List<string>> data { get; }
        public DBType(List<List<string>> data) { this.data = data; }
        public DBType() { this.data = new List<List<string>>(); }
    }
    public class DBWorker
    {
        public MySqlConnection conn;
        public DBWorker(string host, string uname, string password, string db)
        {
            string conString = $"server={host};uid={uname};pwd={password};database={db}";
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = conString;
                conn.Open();
                Console.WriteLine("Успешное подключение к БД");
                DoRequest("SET NAMES cp1251");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка подключения к БД:");
                Console.WriteLine(e.ToString());
            }
        }
        private DBType DoRequest(string req)
        {
            List<List<string>> result = new List<List<string>>();

            var sql = new MySqlCommand(req, conn);
            using (MySqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                {
                    bool flag = true;
                    int i = 0;
                    List<string> tmp = new List<string>();
                    while (flag)
                    {
                        try
                        {
                            tmp.Add(reader[i].ToString());
                            i++;
                        }
                        catch
                        {
                            flag = false;
                        }
                    }
                    result.Add(tmp);
                }
                if (result.Count == 0)
                    return new DBType();
            }
            return new DBType(result);
        }
        public DBType SelectAllItems(){ return DoRequest("SELECT * FROM items");}
        public DBType SelectPlayerByNickname(string nick){ return DoRequest($"SELECT * FROM players WHERE nickname = '{nick}'");}
        public DBType SelectPlayerByUID(int uid){ return DoRequest($"SELECT * FROM players WHERE id = '{uid}'");}
        public DBType InsertPlayer(Player player){ return DoRequest($"INSERT INTO players(nickname, password) VALUES('{player.Name}', '{player.PVars.Get<string>(PvarsInfo.password)}')");}
        public DBType CheckAuth(string nick, string password) { return DoRequest($"SELECT * FROM players WHERE nickname = '{nick}' AND password = '{password}'");}
        public DBType SelectInventary(int uid) { return DoRequest($"SELECT items FROM player_inventary WHERE uid = {uid}"); }
        public DBType UpdatePlayerAdmin(Player player) { return DoRequest($"UPDATE players SET admin_lvl = {player.PVars.Get<int>(PvarsInfo.adminlevel)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerScore(Player player) { return DoRequest($"UPDATE players SET lvl = {player.PVars.Get<int>(PvarsInfo.score)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerMoney(Player player) { return DoRequest($"UPDATE players SET money = {player.PVars.Get<int>(PvarsInfo.money)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerSkin(Player player) { return DoRequest($"UPDATE players SET skin = {player.PVars.Get<int>(PvarsInfo.skin)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerEvent(Player player) { return DoRequest($"UPDATE players SET event = {Convert.ToInt32(player.PVars.Get<bool>(PvarsInfo.isevent))} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerTotp(BasePlayer player) { return DoRequest($"UPDATE players SET totpkey = '{player.PVars.Get<string>(PvarsInfo.totpkey)}' WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerHelper(Player player) { return DoRequest($"UPDATE players SET helplevel = {player.PVars.Get<int>(PvarsInfo.helplevel)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }

        public DBType UpdatePlayerLastIP(BasePlayer player, bool flush = false) { return DoRequest($"UPDATE players SET lastloginip = {(flush ? "NULL" : ("'"+player.IP+"'"))} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerIsBanned(BasePlayer player, int ban = 1) { return DoRequest($"UPDATE players SET banned = {ban} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerLastIP(BasePlayer player, bool flush = false) { return DoRequest($"UPDATE players SET lastloginip = {(flush ? "NULL" : ("'" + player.IP + "'"))} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }

        public DBType InsertSessions(string nick, string ip, string geo) { return DoRequest($"INSERT INTO sessions(nickname, ip, geo) VALUES('{nick}', '{ip}', '{geo}')"); }
        public DBType UpdateSessions_action(int sid, int action) { return DoRequest($"UPDATE sessions SET action = {action}, t_logout = CURRENT_TIMESTAMP WHERE id = {sid}"); }
        public DBType UpdateSessions_uid(int sid, int uid) { return DoRequest($"UPDATE sessions SET uid = {uid} WHERE id = {sid}"); }
        public DBType LAST_INSERT_ID() { return DoRequest("SELECT LAST_INSERT_ID()"); }
        public DBType SelectIPSesstions(string ip, int offset) { return DoRequest($"SELECT * FROM sessions WHERE ip = '{ip}' ORDER BY id DESC LIMIT 30 OFFSET {offset}"); }
        public DBType SelectNameSesstions(string nick, int offset) { return DoRequest($"SELECT * FROM sessions WHERE nickname = '{nick}' LIMIT {30} OFFSET {offset} ORDER BY id desc"); }
        public DBType InsertBan(BasePlayer sender, int puid, string type, int term, string reason) { return DoRequest($"INSERT INTO banlist(type, playeruid, adminuid, term, reason) VALUES('{type}', {puid}, {sender.PVars.Get<int>(PvarsInfo.uid)}, {term}, '{reason}')"); }
        public DBType SelectBanByUID(int uid) { return DoRequest($"SELECT * FROM banlist WHERE playeruid = {uid} AND isActive = 1"); }

        public DBType SelectNameSesstions(string nick, int offset) { return DoRequest($"SELECT * FROM sessions WHERE nickname = '{nick}' ORDER BY id DESC LIMIT 30 OFFSET {offset}"); }
        public DBType CreatePromo(string promo, int reward) { return DoRequest($"INSERT INTO promocodes(promoname, reward) VALUES('{promo}', '{reward}')"); }
        public DBType SetPlayerPromo(Player player) { return DoRequest($"INSERT INTO player_promocode(nickname, promocode) VALUES('{player.Name}', '{player.PVars.Get<string>(PvarsInfo.promocode)}')"); }
        public DBType CheckPromo(string promoname) { return DoRequest($"SELECT * FROM promocodes WHERE promoname = ('{promoname}')"); }
        public DBType DeletePromo(string promoname) { return DoRequest($"DELETE FROM promocodes WHERE promoname = ('{promoname}')"); }
        public DBType AdminCheckPromo(string promoname, int reward) { return DoRequest($"SELECT * FROM promocodes WHERE promoname = ('{promoname}') AND reward = ('{reward}')"); }

    }
}