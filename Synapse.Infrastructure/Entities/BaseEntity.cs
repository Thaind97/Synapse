using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Synapse.Infrastructure.Entities
{
    [Serializable]
    public abstract class BaseEmptyEntity : IBaseEntity
    {
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }

    [Serializable]
    public class BaseEntity : BaseEmptyEntity
    {
        public Guid CreatedBy { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime? UpdatedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime? DeletedDate { get; set; }
        public bool IsSync { get; set; }
    }
}
