using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Pop3;

namespace Penny
{
    class MailListener
    {
        private readonly string _server;
        private readonly string _userAddress;
        private readonly string _password;

        public Action ProcessMessage { get; set; }

        public MailListener(string password, string userAddress, string server)
        {
            _password = password;
            _userAddress = userAddress;
            _server = server;            
        }

        public void WaitForMail()
        {
            var pop3Client = new Pop3Client();
            pop3Client.Connect(_server, _userAddress, _password);
            while ((pop3Client.List() as List<Pop3Message>).Count == 0)
            {
                pop3Client.Disconnect();
                Thread.Sleep(5);
                pop3Client.Connect(_server, _userAddress, _password);
                ProcessMessage();
            }
        }
    }

    class Program	{
	    private static string _server = "localhost";
        private static string _userAddress = "test@mail.local";
	    private static string _password = "password";

        static void Main(string[] args)
        {
            var mailTranslator = new MailTranslator(new MailProcessor());
            new MailListener(_password, _userAddress, _server)
                {
                   ProcessMessage = ()=>mailTranslator.Process(new Pop3Message())
                }.WaitForMail();
	    }

        public static void ProcessMessage()
	    {
	        var smtpClient = new SmtpClient(_server);
	        smtpClient.Send(new MailMessage(_userAddress, "customer@mail.local"));
	    }

	}

    internal class MailProcessor : IListenForOrders
    {
        public void OrderReceived()
        {
            Program.ProcessMessage();
        }
    }
}
