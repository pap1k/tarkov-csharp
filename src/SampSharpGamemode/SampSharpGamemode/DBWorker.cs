using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
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
            catch(Exception e)
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
        public DBType InsertPlayer(Player player){ return DoRequest($"INSERT INTO players(nickname, password) VALUES('{player.Name}', '{player.PVars.Get<string>(PvarsInfo.password)}')");}
        public DBType CheckAuth(string nick, string password) { return DoRequest($"SELECT * FROM players WHERE nickname = '{nick}' AND password = '{password}'");}
        public DBType SelectInventary(int uid) { return DoRequest($"SELECT items FROM player_inventary WHERE uid = {uid}"); }
        public DBType UpdatePlayerAdmin(Player player) { return DoRequest($"UPDATE players SET admin_lvl = {player.PVars.Get<int>(PvarsInfo.adminlevel)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerScore(Player player) { return DoRequest($"UPDATE players SET lvl = {player.PVars.Get<int>(PvarsInfo.score)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerMoney(Player player) { return DoRequest($"UPDATE players SET lvl = {player.PVars.Get<int>(PvarsInfo.money)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerSkin(Player player) { return DoRequest($"UPDATE players SET lvl = {player.PVars.Get<int>(PvarsInfo.skin)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType UpdatePlayerHelper(Player player) { return DoRequest($"UPDATE players SET lvl = {player.PVars.Get<int>(PvarsInfo.helplevel)} WHERE id = {player.PVars.Get<int>(PvarsInfo.uid)}"); }
        public DBType InsertSessions(string nick, string ip, string geo) { return DoRequest($"INSERT INTO sessions(nickname, ip, geo) VALUES('{nick}', '{ip}', '{geo}')"); }
        public DBType UpdateSessions_action(int sid, int action) { return DoRequest($"UPDATE sessions SET action = {action}, t_logout = CURRENT_TIMESTAMP WHERE id = {sid}"); }
        public DBType UpdateSessions_uid(int sid, int uid) { return DoRequest($"UPDATE sessions SET uid = {uid} WHERE id = {sid}"); }
        public DBType LAST_INSERT_ID() { return DoRequest("SELECT LAST_INSERT_ID()"); }
        public DBType SelectIPSesstions(string ip, int offset) { return DoRequest($"SELECT * FROM sessions WHERE ip = '{ip}' ORDER BY id DESC LIMIT 30 OFFSET {offset}"); }
        public DBType SelectNameSesstions(string nick, int offset) { return DoRequest($"SELECT * FROM sessions WHERE nickname = '{nick}' LIMIT {30} OFFSET {offset} ORDER BY id desc"); }

    }
}
