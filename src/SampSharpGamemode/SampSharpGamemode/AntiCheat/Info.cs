namespace SampSharpGamemode.AntiCheat
{
    public class Codes
    {
        public enum e_codes
        {
            MONEY,
            GUN,
            AMMO,
            HP,
            TP,
            JETPACK
        }
        public static string GetString(e_codes code)
        {
            switch (code)
            {
                case e_codes.MONEY:
                    return "Чит на деньги";
                case e_codes.GUN:
                    return "Чит на оружие";
                case e_codes.AMMO:
                    return "Чит на патроны";
                case e_codes.HP:
                    return "Чит на хп";
                case e_codes.TP:
                    return "Телепорт";
                case e_codes.JETPACK:
                    return "Чит на джетпак";
                default:
                    return "<Не удалось выполнить расшифровку кода>";
            }
        }
    }

}
