using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Domain.Entities;

public class TimeConfiguration
{
    public int Id { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; } = "";
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    [MaxLength(100)]
    public string CreatedBy { get; set; } = "";
}
