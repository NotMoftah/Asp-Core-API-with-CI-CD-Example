using System;

namespace SlsApi.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; } = new List<string>();     
    }
}
