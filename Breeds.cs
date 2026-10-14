using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CatKennel;

/// <summary>Порода кота. Раньше была enum, теперь класс — чтобы породы можно было добавлять во время работы.</summary>
public class Breed
{
    public string Name { get; }

    public Breed(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название породы не может быть пустым.", nameof(name));

        Name = name.Trim();
    }

    public override string ToString() => Name;
}

/// <summary>Справочник пород. Содержит стандартные породы и позволяет добавлять новые.</summary>
public static class BreedCatalog
{
    public const int MaxNameLength = 30;

    private static readonly List<Breed> _breeds = new List<Breed>
    {
        new Breed("Сиамская"),
        new Breed("Мейн-кун"),
        new Breed("Сфинкс"),
        new Breed("Шотландская вислоухая")
    };

    public static IReadOnlyList<Breed> All => _breeds.AsReadOnly();
    public static Breed Default => _breeds[0];

    /// <summary>Пытается добавить породу. При ошибке возвращает false и текст причины.</summary>
    public static bool TryAdd(string name, out string error)
    {
        name = name?.Trim();

        if (string.IsNullOrEmpty(name))
        {
            error = "Название породы не может быть пустым.";
            return false;
        }

        if (name.Length > MaxNameLength)
        {
            error = $"Название слишком длинное (максимум {MaxNameLength} символов).";
            return false;
        }

        if (!name.Any(char.IsLetter))
        {
            error = "Название породы должно содержать хотя бы одну букву.";
            return false;
        }

        if (_breeds.Any(b => string.Equals(b.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            error = "Такая порода уже есть в списке.";
            return false;
        }

        _breeds.Add(new Breed(name));
        error = null;
        return true;
    }
}

