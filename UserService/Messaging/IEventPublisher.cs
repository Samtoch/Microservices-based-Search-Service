namespace UserService.Messaging
{
    public interface IEventPublisher
    {
        void Publish<T>(T message, string queueName);
    }
}
