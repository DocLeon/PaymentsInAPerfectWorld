using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Threading;
using NUnit.Framework;
using Pop3;

namespace Penny.specs
{
	[TestFixture]
    public class PennyEndToEndTest
	{
	    public const string TEST_MAIL_SERVER = "localhost";
	    private const string PENNY_PASSWORD = "password";
	    public const string PENNY_EMAIL_ADDRESS = "test@mail.local";
	    
        private ApplicationRunner _application;
	    private Customer _customer;
	    private MailServer _mailServer;

	    [TestFixtureSetUp]
		public void SetUpFixture()
		{
	        
	        _mailServer = new MailServer(TEST_MAIL_SERVER, PENNY_EMAIL_ADDRESS, PENNY_PASSWORD);
		    _mailServer.ClearMailBox();
		    _application = new ApplicationRunner();
			_application.Start();
			_customer = new Customer();
		}

	    [Test]
		public void should_send_an_acknowledge_order_email_to_customer()
		{
			_customer.SendsOrder();	
			_customer.HasReceivedOrderAcknowledgment();
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			_application.Stop();			
		}

	    public const string CUSTOMER_EMAIL_ADDRESS = "customer@mail.local";
	}

    public class MailServer
    {
        private readonly string _server;
        private readonly string _userAddress;
        private readonly string _password;
        private readonly Pop3Client _pop3Client;

        public MailServer(string server, string userAddress, string password)
        {
            _server = server;
            _userAddress = userAddress;
            _password = password;
            _pop3Client = new Pop3Client();
        }

        public void ClearMailBox()
        {
            _pop3Client.Connect(_server, _userAddress, _password);
            try
            {
                var messages = _pop3Client.List();
                foreach (var message in messages)
                    _pop3Client.Delete(message);
            }
            finally
            {
                _pop3Client.Disconnect();     
            }
        }

        public List<Pop3Message> GetMailMessages()
        {
            var pop3Client = new Pop3Client();
            pop3Client.Connect(_server, _userAddress, _password);
            var polls = 0;
            while ((pop3Client.List() as List<Pop3Message>).Count == 0 && polls < 100)
            {
                pop3Client.Disconnect();
                Thread.Sleep(5);
                pop3Client.Connect(_server, _userAddress, _password);
                polls++;
            }
            var pop3Messages = pop3Client.List() as List<Pop3Message>;
            return pop3Messages;
        }
    }

    internal class Customer
	{
        public void SendsOrder()
		{
            var smtpClient = new SmtpClient(PennyEndToEndTest.TEST_MAIL_SERVER);
		    smtpClient.Send(new MailMessage(PennyEndToEndTest.CUSTOMER_EMAIL_ADDRESS, PennyEndToEndTest.PENNY_EMAIL_ADDRESS));
		}
		public void HasReceivedOrderAcknowledgment()
		{
		    var mailMessages = new MailServer(PennyEndToEndTest.TEST_MAIL_SERVER,PennyEndToEndTest.CUSTOMER_EMAIL_ADDRESS,"password").GetMailMessages();
		    Assert.That(mailMessages.Count, Is.EqualTo(1), "Messages received") ;
		}
	}

	internal class ApplicationRunner
	{
		private Process _application;
		private const string PATH_TO_APP = @"..\..\..\..\src\Penny\bin\Debug\Penny.exe";

		public void Stop()
		{
			if (!_application.HasExited)
				_application.Kill();
		}

		public void Start()
		{
			_application = Process.Start(Path.Combine(Directory.GetCurrentDirectory(),PATH_TO_APP));
		}
	}
}
