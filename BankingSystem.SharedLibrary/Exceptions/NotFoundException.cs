namespace BankingSystem.SharedLibrary.Exceptions;

public class NotFoundException(string message) : Exception(message);