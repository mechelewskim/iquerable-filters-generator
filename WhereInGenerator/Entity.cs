using System.ComponentModel.DataAnnotations;
using WhereInGenerator.Generator;

namespace WhereInGenerator
{
    public class Entity
    {
        [GenerateWhereFilter]
        public long Id { get; set; }

        [Required]
        [GenerateWhereFilter]
        public string Name { get; set; }

        [GenerateWhereFilter]
        public long AccountId { get; set; }
    }
}
