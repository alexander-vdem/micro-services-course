using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models;

public class Platform 
{
    public int Id { get; set; } = default;

    public string Name { get; set; } = string.Empty;

    public string Publisher { get; set; } = string.Empty;

    public string Cost { get; set; } = string.Empty;
}