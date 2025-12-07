using System.ComponentModel.DataAnnotations;

namespace HmctsDevChallenge.Backend.Models.Database;

public class Task
{
    [Key] public int Id { get; set; }

    [MaxLength(255)] public required string Title { get; set; }

    [MaxLength(2046)] public string? Description { get; set; }

    [MaxLength(63)] public required string Status { get; set; }

    public required DateTime Due { get; set; }
}