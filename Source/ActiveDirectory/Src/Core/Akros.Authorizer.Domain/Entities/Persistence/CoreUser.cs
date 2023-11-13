using Newtonsoft.Json;

namespace Akros.Authorizer.Domain.Entities.Persistence
{
    public sealed record CoreUser
    {
        public string? P_UPDATE { get; set; }
        public string? P_USERNAME { get; set; }
        public string? P_PASSWORD { get; set; }
        public string? P_ROLE { get; set; }
        public string? P_STATUS { get; set; }
        public string? P_NAME { get; set; }
        public string? P_LAST_NAME { get; set; }
        public int P_DEPARTMENT { get; set; }
        public int P_APP { get; set; }
        public int P_PAIS { get; set; }
        public string? P_EXTENDED_PROPERTY { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
