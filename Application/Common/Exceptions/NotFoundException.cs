namespace TodoAppAPI.Application.Common.Exceptions;

public class NotFoundException(string resourceName, object key)
    : Exception($"{resourceName} with key '{key}' was not found.");
