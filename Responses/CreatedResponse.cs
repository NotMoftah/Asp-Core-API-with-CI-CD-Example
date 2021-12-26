using System;

namespace SlsApi.Responses
{
    public class CreatedResponse : BaseResponse
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
    }
}
