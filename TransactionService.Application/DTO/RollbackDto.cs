namespace TransactionService.Application.DTO;

public class RollbackDto
{
    public DateOnly RollbackDate { get; set; }
    public string? Username { get; set; } 
}