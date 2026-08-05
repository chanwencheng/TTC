namespace TTCRestAPI
{
    public class CommandStartRequest
    {
        public string command { get; set; } = "start";
        public double temp { get; set; }
        public int time { get; set; }
    }
}
