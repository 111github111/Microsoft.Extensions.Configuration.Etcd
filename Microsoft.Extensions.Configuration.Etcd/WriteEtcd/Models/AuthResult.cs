using Newtonsoft.Json;

namespace WriteEtcd.Models
{
    // {"header":{"cluster_id":"14841639068965178418","member_id":"10276657743932975437","revision":"22","raft_term":"7"},"token":"dMQxzqYTUyrbPQpv.80"}
    public class AuthResult
    {
        [JsonProperty("header")]
        public AuthResultHander Header { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public class AuthResultHander
    {
        [JsonProperty("cluster_id")]
        public ulong ClusterId { get; set; }

        [JsonProperty("member_id")]
        public ulong MemberId { get; set; }

        [JsonProperty("revision")]
        public long Revision { get; set; }

        [JsonProperty("raft_term")]
        public ulong RaftTerm { get; set; }
    }
}
