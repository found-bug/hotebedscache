namespace Hotebedscache.Domain.Models.Responses
{
    public class APIResponse<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }

        public APIResponse()
        {
            Status = true;
            Message = null;
        }
        public APIResponse<T> FailResult(string ErrorMessage)
        {
            this.Status = false;
            this.Message = ErrorMessage;
            this.Result = default(T);
            return this;
        }

        public APIResponse<T> FailResult(Exception e)
        {
            string msg;
            if (e.InnerException != null)
                msg = e.InnerException.ToString();
            else
                msg = e.Message.ToString();

            this.Status = false;
            this.Message = msg;
            this.Result = default(T);
            return this;
        }
    }
}
