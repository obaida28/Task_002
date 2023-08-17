using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entites;
public class Rental : BaseEntity
{
    public Guid CarId { get; set; }
    public virtual Car Car { get; set; }
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
    public Guid? DriverId { get; set; }
    public virtual Driver Driver { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DailyRate { get; set; }
    [MaxLength(30)]
    public string State { get; set; }
    [NotMapped]
    public bool IsActive => EndDate >= DateTime.Now.Date && StartDate < DateTime.Now.Date;
    public Rental() : base() {}
}