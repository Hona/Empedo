namespace Empedo.Models.TempusHub
{
    public class TopPlayerOnline
    {
        public string SteamName { get; set; }
        public string RealName { get; set; }
        public ServerInfoShort ServerInfo { get; set; }
        public int TempusId { get; set; }
        public int Rank { get; set; }
        public int RankClass { get; set; }
    }
}