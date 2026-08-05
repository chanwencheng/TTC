namespace TTCRestAPI
{
    public class CommandUploadRequest
    {

        public string? command { get; set; } = "upload";    // 指令，例如 upload, start, stop
        public string? process { get; set; }                // 程序名稱

        // Step 陣列
        public Step[]? steps { get; set; }                  // 階段內容
        
        //* 可選欄位
        public string? issue { get; set; }                  // 可能有 issue
        public int? cycle { get; set; }                     // 可能有 cycle
        public int? time { get; set; }                  // 可能有 time
    }
}
