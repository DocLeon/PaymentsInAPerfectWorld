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
	    public const string CUSTOMER_PASSWORD = "password";
	    public const string PENNY_EMAIL_ADDRESS = "test@mail.local";
	    
        private ApplicationRunner _application;
	    private Customer _customer;
	    private MailServer _mailServer;
	    private Customer _aDifferentCustomer;
        public const string CUSTOMER_EMAIL_ADDRESS = "customer@mail.local";
        private const string DIFFERENT_CUSTOMER_EMAIL_ADDRESS = "RobDaBank@mail.local";

	    [TestFixtureSetUp]
		public void SetUpFixture()
		{
	        
	        _mailServer = new MailServer(TEST_MAIL_SERVER, CUSTOMER_PASSWORD);
		    _mailServer.ClearMailBox(CUSTOMER_EMAIL_ADDRESS);
            _mailServer.ClearMailBox(PENNY_EMAIL_ADDRESS);
            _mailServer.ClearMailBox(DIFFERENT_CUSTOMER_EMAIL_ADDRESS);
		    _application = new ApplicationRunner();
			_application.Start();
			_customer = new Customer(CUSTOMER_EMAIL_ADDRESS);
            _aDifferentCustomer = new Customer(DIFFERENT_CUSTOMER_EMAIL_ADDRESS);
		}

	    [Test]
		public void should_send_an_acknowledge_order_email()
		{
			_customer.SendsOrder();	
			_customer.HasReceivedOrderAcknowledgment();
		}

        [Test]
        public void should_send_an_acknowledgement_order_to_customer_that_sent_order()
        {
            _aDifferentCustomer.SendsOrder();
            _aDifferentCustomer.HasReceivedOrderAcknowledgment();
        }

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			_application.Stop();			
		}

	    
	}

    public class MailServer
    {
        private readonly string _server;
        private readonly string _password;
        private readonly Pop3Client _pop3Client;

        public MailServer(string server, string password)
        {
            _server = server;
            _password = password;
            _pop3Client = new Pop3Client();
        }

        public void ClearMailBox(string mailBoxToClear)
        {
            _pop3Client.Connect(_server, mailBoxToClear, _password);
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

        public List<Pop3Message> GetMailMessages(string mailBoxToRead, string password)
        {
            var pop3Client = new Pop3Client();
            pop3Client.Connect(_server, mailBoxToRead, password);
            var polls = 0;
            while ((pop3Client.List() as List<Pop3Message>).Count == 0 && polls < 100)
            {
                pop3Client.Disconnect();
                Thread.Sleep(5);
                pop3Client.Connect(_server, mailBoxToRead, password);
                polls++;
            }
            var pop3Messages = pop3Client.List() as List<Pop3Message>;
            return pop3Messages;
        }
    }

    internal class Customer
	{
        private readonly string _customerEmailAddress;

        public Customer(string customerEmailAddress)
        {
            _customerEmailAddress = customerEmailAddress;
        }

        public void SendsOrder()
		{
            var smtpClient = new SmtpClient(PennyEndToEndTest.TEST_MAIL_SERVER);
		    smtpClient.Send(new MailMessage(_customerEmailAddress, PennyEndToEndTest.PENNY_EMAIL_ADDRESS));
		}
		public void HasReceivedOrderAcknowledgment()
		{
		    var mailMessages = new MailServer(PennyEndToEndTest.TEST_MAIL_SERVER, PennyEndToEndTest.CUSTOMER_PASSWORD)
		        .GetMailMessages(_customerEmailAddress, PennyEndToEndTest.CUSTOMER_PASSWORD);
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
