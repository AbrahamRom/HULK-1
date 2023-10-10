
public class CompilingError
{
    public CompilingError(int location, ErrorCode code ,string argument)
    {
        this.Code = code;
        this.Argument = argument;
        Location = location;
    }

    public ErrorCode Code { get; }

    public string Argument { get; }

    public int Location { get; }
}

public enum ErrorCode
{
    None,
    Expected,
    Invalid,
    Unknown,
}
