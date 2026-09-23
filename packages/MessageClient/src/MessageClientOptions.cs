namespace MessageClient.Configuration;

public enum MessagingProvider {
    RabbitMQ,
    Kafka
}

public class MessageClientOptions {
    public MessagingProvider MessagingProvider { get; set;} = MessagingProvider.RabbitMQ;
    public string ConnectionString { get; set; } = "";
}