using System.Text.Json;

namespace TTCRestAPI
{
    public static class ResponseHelper
    {

        // <summary>
        /// 將存檔的 JSON 轉成 Response，移除 uploaded 欄位
        /// </summary>
        public static Dictionary<string, object> BuildProcessResponse(string json)
        {
            // 反序列化後移除 uploaded
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var response = new Dictionary<string, object>
            {
                { "process", root.GetProperty("process").GetString() },
                { "steps", JsonSerializer.Deserialize<object>(root.GetProperty("steps").GetRawText()) }
            };

            if (root.TryGetProperty("issue", out var issue) && issue.ValueKind != JsonValueKind.Null)
                response.Add("issue", issue.GetString());

            if (root.TryGetProperty("cycle", out var cycle) && cycle.ValueKind == JsonValueKind.Number)
                response.Add("cycle", cycle.GetInt32());

            if (root.TryGetProperty("time", out var time) && time.ValueKind == JsonValueKind.Number)
                response.Add("time", time.GetInt32());

            return response;
        }
    }
}
