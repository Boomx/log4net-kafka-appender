﻿using Confluent.Kafka;
using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log4net.Kafka.Appender
{
    public class KafkaAppender : AppenderSkeleton
    {
        private Producer producer;

        public KafkaSettings KafkaSettings { get; set; }


        public override void ActivateOptions()
        {
            Console.WriteLine("1");
            base.ActivateOptions();
            Console.WriteLine("2");
            Start();

        }
        private void Start()
        {
            try
            {
                var conf = new Dictionary<string, object>
                {
                  { "group.id", "test-consumer-group" },
                  { "bootstrap.servers", "localhost:9092" },
                  { "auto.commit.interval.ms", 5000 },
                  { "auto.offset.reset", "earliest" }
                };
                Console.WriteLine("3");

                if (KafkaSettings == null) throw new LogException("KafkaSettings is missing");

                if (KafkaSettings.Brokers == null || KafkaSettings.Brokers.Count == 0) throw new Exception("Broker is not found");
                Console.WriteLine("4");

                if (producer == null)
                {
                    var brokers = KafkaSettings.Brokers.Select(x => new Uri(x)).ToArray();
                    // var kafkaOptions = new KafkaOptions(brokers);
#if DEBUG
                    //kafkaOptions.Log = new ConsoleLog();
#else
					//kafkaOptions.Log = new KafkaLog();
#endif
                    //producer = new Producer();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("5");
                ErrorHandler.Error("could not stop producer", ex);
            }

        }
        private void Stop()
        {
            Console.WriteLine("5");

            try
            {
                producer?.Dispose();
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("could not start producer", ex);
            }
        }
        private string GetTopic(LoggingEvent loggingEvent)
        {
            string topic = null;
            if (KafkaSettings.Topic != null)
            {
                var sb = new StringBuilder();
                using (var sw = new StringWriter(sb))
                {
                    KafkaSettings.Topic.Format(sw, loggingEvent);
                    topic = sw.ToString();
                }
            }

            if (string.IsNullOrEmpty(topic))
            {
                topic = $"{loggingEvent.LoggerName}.{loggingEvent.Level.Name}";
            }

            return topic;
        }
        private string GetMessage(LoggingEvent loggingEvent)
        {
            var sb = new StringBuilder();
            using (var sr = new StringWriter(sb))
            {
                Layout.Format(sr, loggingEvent);

                if (Layout.IgnoresException && loggingEvent.ExceptionObject != null)
                    sr.Write(loggingEvent.GetExceptionString());

                return sr.ToString();
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            Console.WriteLine("6");

            var message = GetMessage(loggingEvent);
            var topic = GetTopic(loggingEvent);
            Console.WriteLine($"Enabled Status +{KafkaSettings.HashPartitionEnabled}");
            Console.WriteLine($"Max Partition +{KafkaSettings.NumberMaxPartition}");
            Console.WriteLine($"Topic +{topic}");

           // producer.ProduceAsync(topic, null, Encoding.UTF8.GetBytes(message));
        }
        protected override void OnClose()
        {
            Console.WriteLine("7");

            base.OnClose();
            Stop();
        }
    }
}