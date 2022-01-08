using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Squidzi.Domain.SquidexRepo.Models
{
    public class AuthorData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string Name { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Link { get; set; }
    }
}
