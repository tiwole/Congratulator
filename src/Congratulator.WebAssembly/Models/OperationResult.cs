using System.Buffers;

namespace Congratulator.WebAssembly.Models;

public partial class OperationResult<TData>
{
    /// <summary>
    /// Operation result
    /// </summary>
    /// <param name="resultData"></param>
    /// <param name="isSuccessful"></param>
    /// <param name="message"></param>
    /// <param name="status">optional response status</param>
    public OperationResult(TData? resultData, bool isSuccessful, string? message = null,
        OperationStatus status = OperationStatus.Done)
    {
        Data = resultData;
        IsSuccessful = isSuccessful;
        Message = message ?? "Default message";
        OperationStatus = status;
    }

    public TData? Data { get; }

    public string Message { get; }

    /// <summary>
    /// Status response code
    /// </summary>
    public OperationStatus OperationStatus { get; protected set; }

    /// <summary>
    /// Indicates result status, if value is false then Data contains errors
    /// </summary>
    public bool IsSuccessful { get; }
}

public class OperationResult : OperationResult<object>
{
    public OperationResult(object? resultData,
        bool isSuccessful,
        string? message = null,
        OperationStatus status = OperationStatus.Done)
        : base(resultData, isSuccessful, message, status)
    {
    }

    public static OperationResult FromResult<T>(OperationResult<T> result)
    {
        return new OperationResult(result.Data, result.IsSuccessful, result.Message, result.OperationStatus);
    }
}

public class FailureResult : OperationResult
{
    public FailureResult(string? message = null, OperationStatus status = OperationStatus.InvalidData)
        : base(null, false, message, status)
    {
    }
}

public class FailureResult<TData> : OperationResult<TData>
{
    public FailureResult(string? message = null, OperationStatus status = OperationStatus.InvalidData)
        : base(default, false, message, status)
    {
    }
}

public class SuccessfulResult<TData> : OperationResult<TData>
{
    public SuccessfulResult(TData? resultData = default, string? message = null, OperationStatus status = OperationStatus.Done)
        : base(resultData, true, message, status)
    {
    }
}

public class SuccessfulResult : OperationResult
{
    public SuccessfulResult(object? resultData = null, string? message = null, OperationStatus status = OperationStatus.Done)
        : base(resultData, true, message, status)
    {
    }
}