namespace StudiaZadanko.Models.Core
{
    abstract public class BaseModel
    {
        public int ID { get; set; }

        public virtual string db_table { get; } = "base_table";
    }
}
