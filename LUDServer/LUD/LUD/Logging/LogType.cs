namespace LUD.Logging
{
    [Flags]
    public enum LogType : byte
    {
        Log,
        Warning,
        Error,
        Exception
    }
}
