namespace Fcg.Identity.Infrastructure.Kafka.Settings;

public sealed class KafkaSettings
{
    public const string SectionName = "KafkaSettings";

    public string BootstrapServers { get; set; } = "localhost:9092";
    public string TopicName { get; set; } = "item-created";
}
