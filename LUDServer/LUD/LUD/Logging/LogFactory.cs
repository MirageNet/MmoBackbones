#region Statements

#endregion

namespace LUD.Logging
{
    public sealed class LogFactory
    {
        public static LogType LogType = LogType.Log;

        public static void Log(string message, LogType logType)
        {
            if(LogType != logType) return;

            switch (logType)
            {
                case LogType.Log:

                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine($"{DateTime.Now} [DEBUG] - {message}");

                    break;
                case LogType.Warning:

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    
                    Console.WriteLine($"{DateTime.Now} [WARNING] - {message}");

                    break;
                case LogType.Error:

                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($"{DateTime.Now} [ERROR] - {message}");

                    break;
                case LogType.Exception:

                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    Console.WriteLine($"{DateTime.Now} [EXCEPTION] - {message}");

                    break;
                default:
                    Console.WriteLine($"{DateTime.Now} [UNKNOWN] - log type: {logType} message: {message}");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
