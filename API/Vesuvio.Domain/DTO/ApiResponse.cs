using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vesuvio.Domain.DTO
{


    public class DataTableResponse
    {
        public long RecordsTotal { get; set; }
        public object[] Data { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}