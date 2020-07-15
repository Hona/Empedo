namespace Empedo.Models.TempusHub.Activity
{
    public class CachedTime
    {
        public int MapId { get; set; }
        public int ClassId { get; set; }
        public string ZoneType { get; set; }
        public string ZoneId { get; set; }
        public double? CurrentWRDuration { get; set; }
        public double? OldWRDuration { get; set; }
    }
}