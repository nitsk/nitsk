using System.Collections.Generic;

namespace RevitMcpAddin
{
    public class RevitRequest
    {
        public string Command { get; set; }
        public Dictionary<string, object> Params { get; set; } = new Dictionary<string, object>();
    }

    public class RevitResponse
    {
        public bool Success { get; set; }
        public object Result { get; set; }
        public string Error { get; set; }

        public static RevitResponse Ok(object result) => new RevitResponse { Success = true, Result = result };
        public static RevitResponse Fail(string message) => new RevitResponse { Success = false, Error = message };
    }
}
