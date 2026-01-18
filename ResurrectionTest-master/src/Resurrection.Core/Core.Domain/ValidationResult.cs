using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core
{
    public class ValidationResult
    {
        private readonly List<ValidationError> _errors;

        public string Message { get; set; }
        public bool IsValid => _errors?.Count == 0;
        public bool Invalid => _errors?.Count > 0;

        public IEnumerable<ValidationError> Errors => _errors.AsEnumerable();


        public ValidationResult()
        {
            _errors = new List<ValidationError>();
        }

        public void Add(ValidationError error) => _errors.Add(error);
        public void Add(int code, string message, string fieldWithError = null) => _errors.Add(new ValidationError(code, message, fieldWithError));
        public void Add(string message) => _errors.Add(new ValidationError(message));

    }
}
