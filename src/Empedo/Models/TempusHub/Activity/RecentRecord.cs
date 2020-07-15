namespace Empedo.Models.TempusHub.Activity
{
    public class RecentRecord
    {
        public CachedTime CachedTime { get; set; }
        public RecordInfo RecordInfo { get; set; }
        public ZoneInfo ZoneInfo { get; set; }
        public MapInfo MapInfo { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
    }
}