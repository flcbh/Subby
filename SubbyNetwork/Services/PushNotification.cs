using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using SubbyNetwork.Interfaces;

namespace SubbyNetwork.Services
{
    public class PushNotification : IPushNotification
    {
        private readonly FirebaseMessaging messaging;

        public PushNotification()
        {
            var app = FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile("serviceAccountKey.json").CreateScoped("https://www.googleapis.com/auth/firebase.messaging")});           
            messaging = FirebaseMessaging.GetMessaging(app);
        }
        
        
        public void Send(string title, string body, string token)
        {
            var result = messaging.SendAsync(CreateNotification(title, body, token)).Result; 
        }
        
        private Message CreateNotification(string title, string notificationBody, string token)
        {    
            return new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Body = notificationBody,
                    Title = title
                }
            };
        }
    }
}