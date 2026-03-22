namespace TodoAppAPI.Domain.Common.Exceptions;

public sealed class DomainValidationException(string message) : Exception(message);
