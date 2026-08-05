using System.Text.Json;

namespace TTCRestAPI
{
    public class JsonService
    {
        public void SaveUploadJson(string processName, object[] steps, string filePath)
        {
            var payload = new
            {
                uploaded = LocalTime.CurrentDateTime,
                process = processName,
                steps = steps
            };

            // 序列化成 JSON 字串 (縮排方便閱讀)
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // 存成文字檔
            File.WriteAllText(filePath, json);

            Console.WriteLine($"JSON 已存檔: {filePath}");
        }
    }
}
