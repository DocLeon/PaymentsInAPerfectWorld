using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Pop3;

namespace Penny
{
	class Program
	{
	    private static string _server = "localhost";
        private static string _userAddress = "test@mail.local";
	    private static string _password = "password";

	    static void Main(string[] args)
		{
            var pop3Client = new Pop3Client();
            pop3Client.Connect(_server, _userAddress, _password);
            while ((pop3Client.List() as List<Pop3Message>).Count == 0)
            {
                pop3Client.Disconnect();
                Thread.Sleep(5);
                pop3Client.Connect(_server, _userAddress, _password);
            }
            var smtpClient = new SmtpClient(_server);
            smtpClient.Send(new MailMessage(_userAddress,"customer@mail.local"));
        }
	}
}
