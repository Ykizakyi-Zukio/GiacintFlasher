// from https://github.com/Ykizakyi-Zukio/GiacintTrustEncrypt

namespace GiacintFlasher.Lib.Data;

internal static class Color
{
    internal static string AMain = "\u001b[38;5;218m";  // Розово-лиловый из 256-цветной палитры
    internal static string BMain = "\u001b[38;5;225m"; // Постельно розовый, более яркий
    internal static string Reset = "\u001b[0m";        // Сброс форматирования
    internal static string Error = "\u001b[38;5;217m"; // светло-розово-красный (почти пастельный)
    internal static string Success = "\u001b[38;5;151m"; // светлый салатово-зелёный
    internal static string Warning = "\x1b[38;5;230m"; // Постельно желтый
    internal static string Info = "\u001b[38;5;153m"; // Постельно голубой
    internal static string Blue = "\u001b[38;5;75m"; // Ярко-синий
    internal static string Cyan = "\u001b[38;5;87m"; // Ярко-голубой
    internal static string Green = "\u001b[38;5;120m"; // Ярко-зеленый
    internal static string Yellow = "\u001b[38;5;226m"; // Ярко-желтый
}
