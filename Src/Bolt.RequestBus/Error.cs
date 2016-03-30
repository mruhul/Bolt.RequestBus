namespace Bolt.RequestBus
{
    public class Error : IError
    {
        public Error(string errorMessage)
            : this(string.Empty, string.Empty, errorMessage)
        {
        }

        public Error(string propertyName, string errorMessage)
            : this(string.Empty, propertyName, errorMessage)
        {
        }

        public Error(string errorCode, string propertyName, string errorMessage)
        {
            ErrorCode = errorCode;
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public string ErrorCode { get; set; }
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
