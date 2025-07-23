namespace FincaFenix.Validations.Guards
{
    public class GuardBuilderDateTime
    {
        private readonly DateTime? _value;
        private readonly string _paramName;

        public GuardBuilderDateTime(DateTime? value, string paramName) 
        {
            _value = value;
            _paramName = paramName;
        }
        public GuardBuilderDateTime IsNull(string? message = null)
        {
            if (_value == null)
                throw new Exception(message ?? $"{_paramName}: debe seleccionar una fecha valida.");
            return this;
        }
        public GuardBuilderDateTime LessThan(DateTime? comparisionDate, string? message = null)
        {
            if (_value < comparisionDate)
                throw new Exception(message ?? $"{_paramName} no puede ser menor a {comparisionDate}");
            return this;
        }
    }
}
