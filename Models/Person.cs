using StudiaZadanko.Models.Core;

namespace StudiaZadanko.Models
{
    public class Person : BaseModel
    {
        public override string db_table { get; } = "persons";
        public string? name { get; set; }
        public string? surname { get; set; }

        public string? phone_number { get; set; }
        public string? address { get; set; }

        public string? email { get; set; }
    }
}
