using Pop3;

namespace Penny
{
    public class MailTranslator
    {
        private readonly IListenForOrders _orderListener;

        public MailTranslator(IListenForOrders orderListener)
        {
            _orderListener = orderListener;
        }

        public void Process(Pop3Message mailMessage)
        {
            _orderListener.OrderReceived();
        }
    }
}