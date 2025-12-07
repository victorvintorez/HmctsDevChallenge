using System.ComponentModel.DataAnnotations;
using HmctsDevChallenge.Backend.Helpers.Validation;

namespace HmctsDevChallenge.Backend.Models.Dto;

public readonly record struct TaskDto
{
    public readonly record struct Create
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public required string Title { get; init; }

        [MaxLength(2046, ErrorMessage = "Description cannot exceed 2046 characters.")]
        public string? Description { get; init; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(63, ErrorMessage = "Status cannot exceed 63 characters.")]
        public required string Status { get; init; }

        [Required(ErrorMessage = "Due Date is required.")]
        [FutureDateTime(ErrorMessage = "Due Date must be in the future.")]
        public required DateTime Due { get; init; }
    }

    public readonly record struct Read
    {
        public required int Id { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required string Status { get; init; }
        public required DateTime Due { get; init; }
    }
}