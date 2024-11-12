using System;

namespace TestEtcd.Models
{
    public class MyConfigtion
    {
        public string Redis { get; set; }
        public string SqlServer { get; set; }
        public string[] Array { get; set; }
        public KeyValue Object { get; set; }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
