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
            var mailMessage = new OrderMessage();
            _mailTranslator.Process(mailMessage);
            _orderListener.AssertWasCalled(l=>l.OrderReceived(Arg<string>.Is.Anything));
        }

        [Test]
        public void should_notify_order_received_with_mail_sender_address()
        {
            var mailMessage = new OrderMessage
                {
                    From = "customer"
                };
            _mailTranslator.Process(mailMessage);
            _orderListener.AssertWasCalled(l=>l.OrderReceived(mailMessage.From));
        }
    }
}