using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CatKennel;

/// <summary>Главное меню приложения.</summary>
public class MenuApp
{
    private Cat _currentCat;
    private readonly PetHouse _petHouse = new PetHouse();

    public void Run()
    {
        while (true)
        {
            PrintMenu();
            int choice = InputReader.ReadInt("Выберите пункт: ", 0, 7);

            switch (choice)
            {
                case 1: CreateCat(); break;
                case 2: ShowProfile(); break;
                case 3: CatActions.ShowMenu(_currentCat); break;
                case 4: MoveInCat(); break;
                case 5: ShowResidents(); break;
                case 6: MoveOutPet(); break;
                case 7: AddBreed(); break;
                case 0:
                    Console.WriteLine("Программа завершена. До встречи!");
                    return;
            }
        }
    }

    private static void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("=========== МЕНЮ ПИТОМНИКА ===========");
        Console.WriteLine("1. Создать нового активного кота");
        Console.WriteLine("2. Посмотреть профиль кота");
        Console.WriteLine("3. Действия с котом");
        Console.WriteLine("4. Заселить кота в домик");
        Console.WriteLine("5. Показать всех обитателей домика");
        Console.WriteLine("6. Забрать питомца из домика");
        Console.WriteLine("7. Добавить новую породу");
        Console.WriteLine("0. Завершить программу");
        Console.WriteLine("======================================");
    }

    private void CreateCat()
    {
        _currentCat = CatCreator.Create();
        Console.WriteLine($"\nКот «{_currentCat.Name}» готов к взаимодействию.");
        InputReader.WaitForKey();
    }

    private void ShowProfile()
    {
        CatActions.ShowProperties(_currentCat);
        InputReader.WaitForKey();
    }

    private void MoveInCat()
    {
        if (_currentCat == null)
            Console.WriteLine("Сначала создайте кота (пункт 1).");
        else if (_petHouse.AddAnimal(_currentCat))
            Console.WriteLine($"Кот «{_currentCat.Name}» успешно заселён в домик!");
        else
            Console.WriteLine($"Кот «{_currentCat.Name}» уже живёт в домике.");

        InputReader.WaitForKey();
    }

    private void ShowResidents()
    {
        _petHouse.ShowAnimals();
        InputReader.WaitForKey();
    }

    private void MoveOutPet()
    {
        _petHouse.ShowAnimals();

        if (_petHouse.Count > 0)
        {
            Console.WriteLine();
            int number = InputReader.ReadInt("Номер питомца для выселения: ", 1, _petHouse.Count);
            Console.WriteLine(_petHouse.RemoveAnimal(number - 1)
                ? "Питомец успешно покинул домик."
                : "Ошибка: питомца с таким номером нет в списке.");
        }

        InputReader.WaitForKey();
    }

    private void AddBreed()
    {
        BreedSelector.AddNew();
        InputReader.WaitForKey();
    }
}

/// <summary>Диалог создания кота / котёнка.</summary>
public static class CatCreator
{
    public static Cat Create()
    {
        Console.WriteLine("\n--- Создание кота ---");
        Console.WriteLine("1 — Взрослый кот");
        Console.WriteLine("2 — Котёнок");
        int type = InputReader.ReadInt("Кого создаём? ", 1, 2);

        bool isKitten = type == 2;

        string name = InputReader.ReadString("Кличка: ", Cat.MaxNameLength);

        int maxAge = isKitten ? new Kitten().MaxAge : new Cat().MaxAge;
        string unit = isKitten ? "месяцев" : "лет";
        int age = InputReader.ReadInt($"Возраст ({unit}, 0–{maxAge}): ", 0, maxAge);

        double kg = InputReader.ReadDouble("Вес (в кг, 0.1–20): ", 0.1, 20);
        Breed breed = BreedSelector.Select();

        var weight = new Weight(kg);
        return isKitten
            ? new Kitten(name, age, weight, breed)
            : new Cat(name, age, weight, breed);
    }
}

/// <summary>Просмотр профиля и действия с котом.</summary>
public static class CatActions
{
    public static void ShowProperties(Cat cat)
    {
        if (cat == null)
        {
            Console.WriteLine("Кот ещё не создан (пункт 1).");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Свойства объекта:");
        Console.WriteLine($"Имя: {cat.Name}");
        Console.WriteLine($"Возраст: {cat.AgeText}");
        Console.WriteLine($"Вес: {cat.Weight}");
        Console.WriteLine($"Порода: {cat.Breed}");
    }

    public static void ShowMenu(Cat cat)
    {
        if (cat == null)
        {
            Console.WriteLine("Сначала создайте кота (пункт 1).");
            InputReader.WaitForKey();
            return;
        }

        Console.WriteLine($"\n--- Действия с котом «{cat.Name}» ---");
        Console.WriteLine("1. Поиграть");
        Console.WriteLine("2. Покормить");
        Console.WriteLine("3. Поймать осуждающий взгляд");
        Console.WriteLine("4. Сделать клон кота");
        Console.WriteLine("0. Назад");

        int choice = InputReader.ReadInt("Ваш выбор: ", 0, 4);

        switch (choice)
        {
            case 1:
                cat.Play();
                break;
            case 2:
                cat.Feed();
                break;
            case 3:
                Cat.JudgingLook();
                break;
            case 4:
                Cat clone = cat is Kitten ? new Kitten() : new Cat();
                clone.CopyFrom(cat);
                Console.WriteLine("Успешно создана копия кота:");
                Console.WriteLine(clone);
                break;
            case 0:
                return;
        }

        InputReader.WaitForKey();
    }
}

/// <summary>Выбор существующей породы и добавление новой.</summary>
public static class BreedSelector
{
    public static Breed Select()
    {
        var breeds = BreedCatalog.All;

        Console.WriteLine("\nВыберите породу:");
        for (int i = 0; i < breeds.Count; i++)
            Console.WriteLine($"{i + 1} — {breeds[i].Name}");

        int number = InputReader.ReadInt("Номер породы: ", 1, breeds.Count);
        return breeds[number - 1];
    }

    public static void AddNew()
    {
        Console.WriteLine("\n--- Добавление новой породы ---");
        string name = InputReader.ReadString("Название породы: ", BreedCatalog.MaxNameLength);

        if (BreedCatalog.TryAdd(name, out string error))
            Console.WriteLine($"Порода «{name.Trim()}» добавлена в список.");
        else
            Console.WriteLine($"Ошибка: {error}");
    }
}

