using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CatKennel;

public struct Weight
{
    public double Kilograms { get; private set; }

    public Weight(double kilograms)
    {
        if (double.IsNaN(kilograms) || double.IsInfinity(kilograms) || kilograms < 0)
            throw new ArgumentOutOfRangeException(nameof(kilograms), "Вес не может быть отрицательным или некорректным.");

        Kilograms = kilograms;
    }

    public override string ToString() => $"{Kilograms:F1} кг";

    public static Weight operator +(Weight a, Weight b) => new Weight(a.Kilograms + b.Kilograms);
}

public class Animal
{
    private static int _nextId = 1;
    private Breed _breed;

    public int Id { get; }
    public Weight Weight { get; set; }

    public Breed Breed
    {
        get => _breed;
        set => _breed = value ?? BreedCatalog.Default;
    }

    public Animal()
    {
        Id = _nextId++;
        Weight = new Weight(0);
        Breed = BreedCatalog.Default;
    }

    public Animal(Weight weight, Breed breed)
    {
        Id = _nextId++;
        Weight = weight;
        Breed = breed;
    }

    public virtual void CopyFrom(Animal other)
    {
        if (other == null)
            return;

        Weight = other.Weight;
        Breed = other.Breed;
    }

    public override string ToString()
    {
        return $"Животное #{Id}: порода — {Breed}, вес — {Weight}";
    }
}

public class Cat : Animal
{
    public const int MaxNameLength = 30;
    private int _age;

    public string Name { get; set; }

    public int Age
    {
        get => _age;
        set => _age = value < 0 ? 0 : value;
    }

    public virtual int MaxAge => 30;

    public virtual string AgeText => $"{Age} {GetAgeSuffix(Age)}";

    public Cat() : base()
    {
        Name = "Безымянный";
        Age = 0;
    }

    public Cat(string name, int age, Weight weight, Breed breed)
        : base(weight, breed)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Безымянный" : name.Trim();
        Age = age;
    }

    public virtual void Play()
    {
        Console.WriteLine($"{Name} с удовольствием играет с игрушкой.");
    }

    public void Feed()
    {
        Weight += new Weight(0.1);
        Console.WriteLine($"{Name} сытно поел(а). Текущий вес: {Weight}.");
    }

    public static void JudgingLook()
    {
        Console.WriteLine("Кот медленно прищуривается и одаривает вас холодным осуждающим взглядом.");
    }

    public override void CopyFrom(Animal other)
    {
        base.CopyFrom(other);
        if (other is Cat otherCat)
        {
            Name = otherCat.Name;
            Age = otherCat.Age;
        }
    }

    public override string ToString()
    {
        return $"Кот #{Id}: «{Name}», возраст: {AgeText}, порода: {Breed}, вес: {Weight}";
    }

    public static string GetAgeSuffix(int age)
    {
        int lastDigit = age % 10;
        int lastTwoDigits = age % 100;

        if (lastTwoDigits >= 11 && lastTwoDigits <= 19)
            return "лет";

        return lastDigit switch
        {
            1 => "год",
            2 or 3 or 4 => "года",
            _ => "лет"
        };
    }
}

public class Kitten : Cat
{
    public Kitten() : base() { }

    public Kitten(string name, int age, Weight weight, Breed breed)
        : base(name, age, weight, breed) { }

    public override int MaxAge => 12;
    public override string AgeText => $"{Age} мес.";

    public override void Play()
    {
        Console.WriteLine($"Котёнок {Name} кувыркается и носится по комнате!");
    }

    public override string ToString()
    {
        return $"Котёнок #{Id}: «{Name}», возраст: {AgeText}, порода: {Breed}, вес: {Weight}";
    }
}

