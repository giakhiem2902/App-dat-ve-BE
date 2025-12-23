using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppdatveCore.Entities;

public class BusCompany
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    // Quan hệ 1 - Nhiều: Một hãng xe có nhiều xe
    // Sử dụng virtual để hỗ trợ Lazy Loading nếu cần
    public virtual ICollection<Bus> Buses { get; set; } = new List<Bus>();
}