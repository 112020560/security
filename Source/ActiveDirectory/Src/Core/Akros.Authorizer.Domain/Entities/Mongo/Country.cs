using Akros.Authorizer.Domain.Common;

namespace Akros.Authorizer.Domain.Entities.Mongo
{
    [BsonCollection("Country")]
    public sealed class Country: Document
    {
        public int code { get; set; }
        public string? cod_area { get; set; }
        public string? name { get; set; }
        public string? abbreviation { get; set; }
        public bool enable { get; set; }
        public string? source_core { get; set; }
        public string? ldap { get; set; }
        public string? domain { get; set; }
        public int rule_code { get; set; }
    }
}
