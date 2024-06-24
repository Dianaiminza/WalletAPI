using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public IEnumerable<ValidationError> Errors { get; set; }

        public ApiException()
        {
            Errors = Enumerable.Empty<ValidationError>();
        }

        public ApiException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, IEnumerable<ValidationError> errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}
