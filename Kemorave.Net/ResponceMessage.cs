namespace Kemorave.Net
{
    public class ResponceMessage
    {

        public ResponceMessage()
        {
        }
        public string Message { get; set; }
        public long Code { get; set; }
        public override string ToString()
        {
            return Message ?? "No message";
        }
    }
}