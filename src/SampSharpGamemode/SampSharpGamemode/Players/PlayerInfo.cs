namespace SampSharpGamemode.Players
{
    public enum e_PlayerInfo
    {
        PINFO_UID = 0,
        PINFO_NICKNAME,
        PINFO_PASSWORD,
        PINFO_SCORE,
        PINFO_ADMINLVL,
        PINFO_HELPERLVL,
        PINFO_SKIN,
        PINFO_MONEY
    }
    public class PvarsInfo
    {
        public static string
            uid = "pUID",
            score = "pSCORE",
            helplevel = "pHELPHEVEL",
            adminlevel = "pADMINLEVEL",
            skin = "pSKIN",
            money = "pMONEY",
            sessionid = "pSESSIONID",
            spawned = "pSPAWNED",
            ingame = "pINGAME",
            admin = "pADMIN",
            helper = "pHELPER",
            password = "pHASHPASSWORD",
            pass = "pPASSWORD",
            isTemp = "pTEMP";
    }
}