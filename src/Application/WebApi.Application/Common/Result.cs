namespace WebApi.Application.Common;

/// <summary>
/// Класс для представления результата выполнения операции
/// </summary>
/// <typeparam name="T">Тип возвращаемого значения</typeparam>
public class Result<T>
{
    /// <summary>
    /// Признак успешного выполнения операции
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Значение результата (доступно только при успешном выполнении)
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Сообщение об ошибке (доступно только при неуспешном выполнении)
    /// </summary>
    public string? Error { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Создать успешный результат
    /// </summary>
    /// <param name="value">Значение результата</param>
    /// <returns>Успешный результат с заданным значением</returns>
    public static Result<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// Создать результат с ошибкой
    /// </summary>
    /// <param name="error">Сообщение об ошибке</param>
    /// <returns>Результат с ошибкой</returns>
    public static Result<T> Failure(string error) => new(false, default, error);
}
