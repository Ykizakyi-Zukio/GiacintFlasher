// from https://github.com/Ykizakyi-Zukio/GiacintTrustEncrypt

using System.Text;
using GiacintFlasher.Lib.Data;


namespace GiacintFlasher.Lib.Services;

internal static class Debug
{
    internal static void Error(Exception ex) => Error(ex.ToString());
    internal static void Error(string message)
    {
        Console.Error.WriteLine($"{Color.Reset}[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] {Color.Error}× {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static void Warning(string message)
    {
        Console.WriteLine($"{Color.Reset}[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] {Color.Warning}⚠  {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static void Success(string message)
    {
        Console.WriteLine($"{Color.Reset}[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] {Color.Success}✓  {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static void Info(string message)
    {
        if (message == "") return;
        Console.WriteLine($"{Color.Reset}[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] {Color.Info}ⓘ  {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static string? Input()
    {
        Console.Write($"{Color.Reset}[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] ~ {Environment.UserName} ->{Color.Info} ");
        return Console.ReadLine();
    }
}
