namespace FincaFenix.Validations.Guards
{
    public static class Guard
    {
        public static GuardBuilderInt Against(int value, string paramName)
        {
            return new GuardBuilderInt(value, paramName);
        }
        public static GuardBuilderDateTime Against(DateTime? value, string paramName)
        { 
            return new GuardBuilderDateTime(value, paramName);
        }
    }
}
