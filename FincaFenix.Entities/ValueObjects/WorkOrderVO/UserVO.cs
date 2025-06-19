namespace FincaFenix.UsesCases.ValueObjects.WorkOrderVO
{
    public class UserVO
    {
        public UserVO(string userName, DateTime dateTime)
        {
            UserName = userName;
            DateTime = dateTime;
        }

        public string UserName { get; }
        public DateTime DateTime { get; }
    }
}
