namespace SubbyNetwork.Interfaces
{
    public interface IPushNotification
    {
        void Send(string title, string body, string token);
    }
}