namespace FincaFenix.Validations.Guards
{
    public class GuardBuilderInt
    {
        private readonly int _value;
        private readonly string _paramName;
        public GuardBuilderInt(int value, string paramName)
        {
            _value = value;
            _paramName = paramName;
        }

        public GuardBuilderInt IsEqualTo(int comparisionValue, string? message = null)
        {
            if (_value == comparisionValue)
                throw new Exception(message ?? $"El {_paramName} debe ser distinto a {comparisionValue}");
            return this;
        }
    }
}
