﻿using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log4net.Kafka.Appender
{
    public class KafkaSettings
    {
        public List<string> Brokers { get; set; }

        /// <summary>
        /// Get or set the message topic (routing key).  Default value is "%level". 
        /// </summary>
        public PatternLayout Topic { get; set; }
        public PatternLayout Partition { get; set; }
        public bool HashPartitionEnabled { get; set; }
        public string NumberMaxPartition { get; set; }

        public List<string> Params { get; set; }
    }
}
