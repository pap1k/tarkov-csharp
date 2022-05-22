using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

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
        public DBType InsertPlayer(Player player){ return DoRequest($"INSERT INTO players(nickname, password) VALUES('{player.Name}', '{player.password}')");}
    }
}
