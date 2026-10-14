using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CatKennel;

public static class InputReader
{
    public static string ReadString(string prompt, int maxLength = 50, bool allowEmpty = false)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim() ?? "";

            if (input.Length == 0 && !allowEmpty)
            {
                Console.WriteLine("Ошибка: значение не может быть пустым.");
                continue;
            }

            if (input.Length > maxLength)
            {
                Console.WriteLine($"Ошибка: максимальная длина — {maxLength} символов.");
                continue;
            }

            return input;
        }
    }

    public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim();

            if (!int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                Console.WriteLine("Ошибка: нужно ввести целое число.");
                continue;
            }

            if (value < min || value > max)
            {
                Console.WriteLine($"Ошибка: число должно быть от {min} до {max}.");
                continue;
            }

            return value;
        }
    }

    public static double ReadDouble(string prompt, double min, double max)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim().Replace(',', '.');

            if (!double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                || double.IsNaN(value) || double.IsInfinity(value))
            {
                Console.WriteLine("Ошибка: нужно ввести число (например, 4.5).");
                continue;
            }

            if (value < min || value > max)
            {
                Console.WriteLine($"Ошибка: значение должно быть от {min} до {max}.");
                continue;
            }

            return value;
        }
    }

    public static void WaitForKey()
    {
        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey(true);
    }
}

