namespace SampSharpGamemode.Players
{
    public enum e_AuthState
    {
        PASSWORD,
        TOTP,
        SUCCESS
    }
    public enum e_PlayerInfo
    {
        PINFO_UID = 0,
        PINFO_NICKNAME,
        PINFO_PASSWORD,
        PINFO_SCORE,
        PINFO_ADMINLVL,
        PINFO_HELPERLVL,
        PINFO_SKIN,
        PINFO_MONEY,
        PINFO_TOTPKEY,
        PINFO_LASTIP,
        PINFO_EVENT,
        PINFO_WARNS,
        PINFO_ISBANNED
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
            isTemp = "pTEMP",
            lastip = "pLASTIP",
            authstate = "pAUTHSTATE",
            isevent = "pEVENT",
            isleaving = "PISLEAVING",
            totpkey = "pTOTP";
    }
}