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
            _mailTranslator = new MailTranslator();
            _orderListener = MockRepository.GenerateMock<IListenForOrders>();
        }
        
        [Test]
        public void should_notify_order_received_when_mail_arrives()
        {
            var mailMessage = new Pop3Message();
            _mailTranslator.Process(mailMessage);
            _orderListener.AssertWasCalled(l=>l.orderReceived());
        }
    }

    internal class MailTranslator
    {
        public void Process(Pop3Message mailMessage)
        {
        }
    }

    public interface IListenForOrders
    {
        void orderReceived();
    }
}