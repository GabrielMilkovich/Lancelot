namespace Core
{
    public abstract class BaseEntity
    {
        public bool Valido { get; private set; }
        public bool Invalido { get; private set; }
        public ValidationResult ValidationResult { get; private set; }

        protected BaseEntity()
        {
            Valido = true;
            Invalido = false;
            ValidationResult = new ValidationResult();
        }

        public abstract void RealizaValidacoes();

        public void AdicionaNotificacao(ValidationError validationError)
        {
            Valido = false;
            Invalido = true;
            ValidationResult.Add(validationError);
        }
    }
}