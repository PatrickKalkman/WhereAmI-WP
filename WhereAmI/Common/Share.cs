using Microsoft.Phone.Tasks;

namespace WhereAmI
{
    public class Share
    {
        public void ShareEmail(string messageToShare)
        {
            var emailComposeTask = new EmailComposeTask()
            {
                Subject = "just a message",
                Body = messageToShare,
                To = "",
            };

            emailComposeTask.Show();
        }

        public void ShareSMS(string messageToShare)
        {
            var smsComposeTask = new SmsComposeTask { Body = messageToShare };
            smsComposeTask.Show();
        }

        public void ShareStatus(string loveMessage)
        {
            var shareStatusTask = new ShareStatusTask { Status = loveMessage };
            shareStatusTask.Show();
        }
    }
}
