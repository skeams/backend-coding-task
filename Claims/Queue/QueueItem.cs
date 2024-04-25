namespace Claims.Queue
{
    public class QueueItem
    {
        public string Action { get; set; }
        public string Id { get; set; }
        public bool IsClaim { get; set; }
    }
}
