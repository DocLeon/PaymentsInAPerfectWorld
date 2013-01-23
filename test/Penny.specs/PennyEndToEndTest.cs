using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

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
		public void should_send_an_order_received_email_to_customer()
		{
			_customer.SendsOrder();	
			_customer.HasReceivedOrderReceivedMessage();
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			_application.Stop();			
		}
    }

	internal class Customer
	{		
		public object HasReceivedOrderReceivedMessageToCustomerFromApplication
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public void SendsOrder()
		{
			throw new System.NotImplementedException();
		}

		public void HasReceivedOrderReceivedMessage()
		{
			throw new System.NotImplementedException();
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
