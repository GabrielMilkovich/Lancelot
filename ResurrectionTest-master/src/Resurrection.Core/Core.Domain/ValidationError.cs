using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ValidationError
    {
        public ValidationError(string message)
        {
            Code = 0;
            Message = message;
        }

        public ValidationError(int code, string message, string fieldWithError = null)
        {
            Code = code;
            Message = message;
            FieldWithError = fieldWithError;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public int Code { get; private set; }
        public string Message { get; private set; }
        public string FieldWithError { get; private set; }
    }
}
