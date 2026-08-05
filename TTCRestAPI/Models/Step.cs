using System.ComponentModel.DataAnnotations;

namespace TTCRestAPI
{
    public class Step
    {
        [Required]
        public int Phase { get; set; }        // 階段序號

        [Required]
        public double Temp { get; set; }      // 溫度 (°C, 小數點1位)
        
        [Required]
        public int Time { get; set; }         // 時間 (秒)
    }
}
