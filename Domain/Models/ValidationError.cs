using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ValidationError
    {
        public string Key { get; set; }

        public string Message { get; set; }

        public ValidationError(string key, string message)
        {
            Key = key;
            Message = message;
        }
    }
}
