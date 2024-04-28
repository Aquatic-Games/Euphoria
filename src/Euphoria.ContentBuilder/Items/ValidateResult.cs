namespace Euphoria.ContentBuilder.Items;

public struct ValidateResult
{
    public bool Succeeded;

    public string FailureReason;

    public ValidateResult(bool succeeded, string failureReason)
    {
        Succeeded = succeeded;
        FailureReason = failureReason;
    }

    public static ValidateResult Success => new ValidateResult(true, null);

    public static ValidateResult Failure(string failureReason) 
        => new ValidateResult(false, failureReason);
}