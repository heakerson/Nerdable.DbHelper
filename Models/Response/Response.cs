using System;
using System.Collections.Generic;
using System.Text;

namespace Nerdable.DbHelper.Models.Response
{
    public class Response<TResponse>
    {
        public string ReturnMessage { get; set; }
        public ReturnCode ReturnCode { get; set; }
        public string ReturnCodeString { get; set; }
        public bool Success { get; set; }
        public TResponse Data { get; set; }

        public static Response<TResponse> BuildResponse(TResponse data, bool success = true, ReturnCode returnCode = ReturnCode.Success, string returnMessage = "Success")
        {
            return new Response<TResponse>()
            {
                Data = data,
                Success = success,
                ReturnCode = returnCode,
                ReturnMessage = returnMessage,
                ReturnCodeString = returnCode.ToString()
            };
        }
    }
}
