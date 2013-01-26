using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using NUnit.Framework;
using Pop3;

namespace Penny.specs
{
	[TestFixture]
    public class PennyEndToEndTest
	{
		private ApplicationRunner _application;
		private Customer _customer;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
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
    }

	internal class Customer
	{		
		public void SendsOrder()
		{
            var smtpClient = new SmtpClient("localhost");
		    smtpClient.Send(new MailMessage("customer@mail.local", "test@mail.local"));
		}
		public void HasReceivedOrderAcknowledgment()
		{
		    var pop3Client = new Pop3Client();
            pop3Client.Connect("localhost","test@mail.local","password");
            Assert.That(pop3Client.IsConnected,"connected to mail server");
		}
	}

	internal class ApplicationRunner
	{
		private Process _application;
		private const string PATH_TO_APP = @"..\..\..\..\src\Penny\bin\Debug\Penny.exe";

		public void HasReceivedOrder()
		{
			throw new System.NotImplementedException();
		}

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
