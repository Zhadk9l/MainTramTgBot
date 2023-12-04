namespace ModernTramTgBot.Models
{
    public class Stops
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int RouteID { get; set; }
        public TimeSpan Time { get; set; }

    }
}
