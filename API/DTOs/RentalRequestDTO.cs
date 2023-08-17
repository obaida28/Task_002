using System.ComponentModel.DataAnnotations;
using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class RentalRequestDTO : RequestDTO<Rental>
{
    [DataType (DataType.Date)]
    public DateTime SearchDate { get; set; }
}