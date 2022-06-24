using Aws.GameLift;

namespace InvokeChat.Backend.Host.Aws;

public static class AwsUtils
{
    internal static void ProcessResponse(GenericOutcome outcome)
    {
        if (!outcome.Success)
        {
            throw new InvalidOperationException(outcome.Error.ToString());
        }
    }
}
