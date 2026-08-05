using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TTCRestAPI.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Route("[controller]")]
    public class TTCController : ControllerBase
    {

        private readonly IWebHostEnvironment _env;


        private readonly JsonService _JsonService;

        private string exePath = AppContext.BaseDirectory;

        private string fileProcessPath = string.Empty;


        Random rnd = new Random();

        string[] messages = new string[] { };
        string[] status = new string[] { };

        double[] temperatures = new double[] { };

        
        string vStatus = string.Empty;
        string vMessage = string.Empty;
        string vProcessName = string.Empty;

        double? iTemperture = Global.temperture;
        double? iHumidity = Global.humidity;

        int num = 0;
        int run = 0;

        public TTCController(IWebHostEnvironment env)
        {
            _env = env;
            _JsonService = new JsonService();

            fileProcessPath = Path.Combine(exePath, Constant.Archive, Constant.Process, $"{Constant.Process}.json");
        }

        [HttpGet(Name = "ttc")]
        public IActionResult Get([FromQuery] string client, [FromQuery] string token, [FromBody] CommandUploadRequest request)
        {
            string? command = request?.command;
            switch (command)
            {
                case "path":
                    return Ok( exePath );

                case "ping":
                    num = rnd.Next(0, 3); // 包含 0，但不包含 3（即產生 0 到 2）

                    status = new string[] { "ERROR", "STANDBY", "RUNNING" };
                    messages = new string[] { "SERVICE_UNAVAILABLE", "TTC REST API is running", "" };

                    vStatus = status[num];
                    vMessage = messages[num];

                    return Ok(new
                    {
                        status = vStatus,
                        version = "v260715c",
                        time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                        temp = iTemperture,
                        humi = iHumidity,
                        message = vMessage
                    });

                case "status":
                    num = rnd.Next(0, 4);               // 包含 0，但不包含 4（即產生 0 到 3）
                    run = num==0 ? 0 : rnd.Next(1, 6);  // 包含 1，但不包含 6（即產生 1 到 5）

                    string xProcess = Global.running[num];

                    vProcessName = num > 1 ? DateTime.Now.ToString("yyyy-MM-dd") + num : string.Empty;
                    string xName = vProcessName; //
                    
                    string xElapsed = num == 0 ? "0" : "00:15:32";
                    string xState = num > 0 ? Global.state[run] : Global.state[0];


                    double? iTemp = iTemperture;
                    double? iHumi = iHumidity;

                    double diff = rnd.NextDouble() * (0.6 - 0.1) + 0.1;
                    double itemp1 = iTemp == null ? 0 : Math.Round(iTemp.Value - diff, 2);
                    double itemp2 = iTemp == null ? 0 : Math.Round(iTemp.Value + (diff * 2), 2);
                    double itemp3 = iTemp == null ? 0 : Math.Round(iTemp.Value + diff, 2);
                    double itemp4 = iTemp == null ? 0 : Math.Round(iTemp.Value - (diff * 2), 2);

                    double[] iTemps = num == 0 ? new double[] { } : new double[] { itemp1, itemp2, itemp4, itemp3, itemp3, itemp4, itemp1, itemp2 };

                    int iPhase = num > 1 ? rnd.Next(1, 8) : 0; 

                    return Ok(new
                    {
                        time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        process = xProcess,
                        name = xName,
                        phase = iPhase,
                        elapsed = xElapsed,
                        temp = iTemp,
                        temps = iTemps,
                        humi = iHumi,
                        state = xState,
                        di = Global.di,
                        countdown = 88
                    });

                case "process":

                    if (!System.IO.File.Exists(fileProcessPath)) return Ok(new { process = "N/A" });

                    //return Ok(new { process = fileProcessPath });

                    var json = System.IO.File.ReadAllText(fileProcessPath);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return Ok(new { process = "N/A" });
                    }

                    //var obj = JsonSerializer.Deserialize<object>(json);
                    //return Ok(obj);

                    // 呼叫外部方法
                    var response = ResponseHelper.BuildProcessResponse(json);
                    return Ok(response);

                default:
                    return BadRequest(new { status = "failure", message = "command error" });
            }
        }


        [HttpPost(Name = "ttc")]
        public IActionResult Post([FromQuery] string client, [FromQuery] string token, [FromBody] JObject request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = Status.failure.ToString(),
                    message = "command"
                });
            }

            string command = request["command"]?.ToString() ?? string.Empty;

            switch (command)
            {
                case "upload":

                    var rqUpload = request.ToObject<CommandUploadRequest>();

                    // 檢查必填欄位
                    if (string.IsNullOrEmpty(rqUpload.process)) return BadRequest(new { status = Status.failure.ToString(), message = "command" });

                    if (rqUpload.steps == null || rqUpload.steps.Length == 0) return BadRequest(new { status = Status.failure.ToString(), message = "command" });

                    // 驗證 steps
                    foreach (var step in rqUpload.steps)
                    {
                        if (step == null)
                            return BadRequest(new { status = Status.failure.ToString(), message = "command: step object missing" });

                        // 檢查 phase
                        if (step.Phase <= 0)
                            return BadRequest(new { status = Status.failure.ToString(), message = "command: invalid phase" });

                        // 檢查 time
                        if (step.Time <= 0)
                            return BadRequest(new { status = Status.failure.ToString(), message = "command: invalid time" });
                    }

                    // 檢查 phase 是否依序排列
                    for (int i = 0; i < rqUpload.steps.Length; i++)
                    {
                        if (rqUpload.steps[i].Phase != i + 1)
                        {
                            return BadRequest(new { status = Status.failure.ToString(), message = "command: phase not in sequence" });
                        }
                    }

                    return Ok(new { command = "upload", status = Status.OK.ToString() });

                //// 儲存

                //_JsonService.SaveUploadJson(rqUpload.process ?? string.Empty, rqUpload.steps, fileProcessPath);

                //Console.WriteLine("6");
                //// 建立回傳物件 (只包含有值的欄位)
                //var response = new Dictionary<string, object>
                //{
                //    { "command", "upload" },
                //    { "process", rqUpload.process }
                //};

                //if (!string.IsNullOrEmpty(rqUpload.issue)) response.Add("issue", rqUpload.issue);

                //if (rqUpload.cycle.HasValue) response.Add("cycle", rqUpload.cycle.Value);

                //if (rqUpload.time.HasValue) response.Add("time", rqUpload.time.Value);

                //response.Add("steps", rqUpload.steps);

                //response.Add("status", Status.OK.ToString());

                //return Ok(response);

                case "program":
                    return Ok(new { command = "program", status = Status.OK.ToString() });

                case "single":
                    return Ok(new { command = "single", status = Status.OK.ToString() });

                case "start":
                    var rqStart = request.ToObject<CommandStartRequest>();
                    var startResult = Validate.CommandStart(rqStart.command, rqStart.temp, rqStart.time);
                    if (!startResult.IsValid)
                        return BadRequest(new { status = "failure", message = startResult.ErrorMessage });

                    return Ok(new { 
                        rqStart.command, 
                        rqStart.temp, 
                        rqStart.time, 
                        status = "OK" 
                    });

                case "stop":
                    return Ok(new { command = "stop", status = Status.OK.ToString() });

                case "reset":
                    return Ok(new { command = "reset", status = Status.OK.ToString() });

                default:
                    return BadRequest(new { status = Status.failure.ToString(), message = "command error" });
            }
        }
    }
}
