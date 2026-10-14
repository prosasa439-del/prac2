using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CatKennel;

public interface IPetHouse
{
    bool AddAnimal(Animal animal);
    bool RemoveAnimal(int index);
    void ShowAnimals();
}

public class PetHouse : IPetHouse
{
    private readonly List<Animal> _animals = new List<Animal>();

    public IReadOnlyList<Animal> Animals => _animals.AsReadOnly();
    public int Count => _animals.Count;

    public bool AddAnimal(Animal animal)
    {
        if (animal == null || _animals.Contains(animal))
            return false;

        _animals.Add(animal);
        return true;
    }

    public bool RemoveAnimal(int index)
    {
        if (index < 0 || index >= _animals.Count)
            return false;

        _animals.RemoveAt(index);
        return true;
    }

    public void ShowAnimals()
    {
        if (_animals.Count == 0)
        {
            Console.WriteLine("В домике пока никого нет.");
            return;
        }

        Console.WriteLine("Обитатели домика:");
        for (int i = 0; i < _animals.Count; i++)
            Console.WriteLine($"  [{i + 1}] {_animals[i]}");
    }
}

