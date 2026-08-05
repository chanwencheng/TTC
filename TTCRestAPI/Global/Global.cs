namespace TTCRestAPI
{
    public class Global
    {

        //# Array
        public static string[] running = { string.Empty, "running", "single", "program" };

        public static string[] state = { "Idle", "Finish", "Stop", "Pause", "Counting Down", "Temperature Adjusting", "", "", "", "Error" };

        public static int[] di = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        //# String
        public static string process = "";

        public static string process_name = string.Empty;

        //# Double

        public static double temperture = 55.5;     //0

        public static double humidity = 35;         //0


        // Int
        public static int phase = 0; 

    }
}
