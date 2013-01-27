using NUnit.Framework;
using Pop3;
using Rhino.Mocks;

namespace Penny.specs.unit
{
    [TestFixture]
    public class MailTranslatorTest
    {
        private MailTranslator _mailTranslator;
        private IListenForOrders _orderListener;

        [SetUp]
        public void SetUp()
        {
            _orderListener = MockRepository.GenerateMock<IListenForOrders>();
            _mailTranslator = new MailTranslator(_orderListener);
        }
        
        [Test]
        public void should_notify_order_received_when_mail_arrives()
        {
            var mailMessage = new Pop3Message();
            _mailTranslator.Process(mailMessage);
            _orderListener.AssertWasCalled(l=>l.OrderReceived());
        }
    }
}