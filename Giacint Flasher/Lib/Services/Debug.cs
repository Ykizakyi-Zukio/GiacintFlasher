// from https://github.com/Ykizakyi-Zukio/GiacintTrustEncrypt

using System.Text;
using GiacintFlasher.Lib.Data;


namespace GiacintFlasher.Lib.Services;

internal static class Debug
{
    internal static void Error(Exception ex) => Error(ex.ToString());
    internal static void Error(string message)
    {
        Console.Error.WriteLine($"{CalcTime()} {Color.Error}× {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static void Warning(string message)
    {
        Console.WriteLine($"{CalcTime()} {Color.Warning}⚠  {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static void Success(string message)
    {
        Console.WriteLine($"{CalcTime()} {Color.Success}✓  {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static void Info(string message)
    {
        if (message == "") return;
        Console.WriteLine($"{CalcTime()} {Color.Info}ⓘ  {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static string? Input()
    {
        Console.Write($"{CalcTime()} ~ {Environment.UserName} ->{Color.Info} ");
        return Console.ReadLine();
    }

    private static string CalcTime() => $"{Color.Reset}[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}]";
}
