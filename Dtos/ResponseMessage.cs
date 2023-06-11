
namespace api_aspnetcore6.Dtos
{
    public class ResponseMessage
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }

        public ResponseMessage(bool IsSuccess, string Message)
        {
            this.IsSuccess = IsSuccess;
            this.Message = Message;
        }

        public ResponseMessage(bool IsSuccess, string Message, object Data)
        {
            this.IsSuccess = IsSuccess;
            this.Message = Message;
            this.Data = Data;
        }

    }
}