namespace TTCRestAPI
{
    public static class Validate
    {


        public static (bool IsValid, string ErrorMessage) CommandStart(string command, object temp, object time)
        {

            // 檢查 temp 是否為數字
            if (!(temp is int || temp is double || temp is float || temp is decimal))
                return (false, "command: invalid temp") ;

            // 檢查 time 是否為數字且 >= 0
            if (!(time is int) || (int)time < 0)
                return (false, "command: invalid time") ;

            // 成功回傳
            return (true, string.Empty);
        }
    }
}
