using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

enum CatBreed
{
    Siamese,
    MaineCoon,
    Sphynx,
    ScottishFold
}

struct Weight
{
    public double Kilograms { get; private set; }

    public Weight(double kilograms)
    {
        Kilograms = kilograms < 0 ? 0 : kilograms;
    }

    public override string ToString()
    {
        return $"{Kilograms:F1} кг";
    }

    public static Weight operator +(Weight a, Weight b)
    {
        return new Weight(a.Kilograms + b.Kilograms);
    }
}

class Animal
{
    private static int _nextId = 1;
    public int Id { get; }

    public Weight Weight { get; set; }
    public CatBreed Breed { get; set; }

    public Animal()
    {
        Id = _nextId++;
        Weight = new Weight(0);
        Breed = CatBreed.Siamese;
    }

    public Animal(Weight weight, CatBreed breed)
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

    public static string GetBreedName(CatBreed breed)
    {
        return breed switch
        {
            CatBreed.Siamese => "Сиамская",
            CatBreed.MaineCoon => "Мейн-кун",
            CatBreed.Sphynx => "Сфинкс",
            CatBreed.ScottishFold => "Шотландская вислоухая",
            _ => breed.ToString()
        };
    }

    public override string ToString()
    {
        return $"Животное #{Id}: порода — {GetBreedName(Breed)}, вес — {Weight}";
    }
}

class Cat : Animal
{
    private int _age;

    public string Name { get; set; }

    public int Age
    {
        get => _age;
        set => _age = value < 0 ? 0 : value;
    }

    public Cat() : base()
    {
        Name = "Безымянный";
        Age = 0;
    }

    public Cat(string name, int age, Weight weight, CatBreed breed)
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
        return $"Кот #{Id}: «{Name}», возраст: {Age} {GetAgeSuffix(Age)}, порода: {GetBreedName(Breed)}, вес: {Weight}";
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

class Kitten : Cat
{
    public Kitten() : base() { }

    public Kitten(string name, int age, Weight weight, CatBreed breed)
        : base(name, age, weight, breed) { }

    public override void Play()
    {
        Console.WriteLine($"Котёнок {Name} кувыркается и носится по комнате!");
    }

    public override string ToString()
    {
        return $"Котёнок #{Id}: «{Name}», возраст: {Age} мес., порода: {GetBreedName(Breed)}, вес: {Weight}";
    }
}

interface IPetHouse
{
    void AddAnimal(Animal animal);
    bool RemoveAnimal(int index);
    void ShowAnimals();
}

class PetHouse : IPetHouse
{
    private readonly List<Animal> _animals = new List <Animal>();

    public IReadOnlyList <Animal> Animals => _animals.AsReadOnly();
    public int Count => _animals.Count;

    public void AddAnimal(Animal animal)
    {
        if (animal != null)
            _animals.Add(animal);
    }

    public bool RemoveAnimal(int index)
    {
        if (index >= 0 && index < _animals.Count)
        {
            _animals.RemoveAt(index);
            return true;
        }
        return false;
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
        {
            Console.WriteLine($"  [{i + 1}] {_animals[i]}");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Cat currentCat = null;
        PetHouse petHouse = new PetHouse();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=========== МЕНЮ ПИТОМНИКА ===========");
            Console.WriteLine("1. Создать / изменить активного кота");
            Console.WriteLine("2. Посмотреть профиль кота");
            Console.WriteLine("3. Действия с котом");
            Console.WriteLine("4. Заселить кота в домик");
            Console.WriteLine("5. Показать всех обитателей домика");
            Console.WriteLine("6. Забрать питомца из домика");
            Console.WriteLine("0. Завершить программу");
            Console.WriteLine("======================================");
            Console.Write("Выберите пункт: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    currentCat = CreateCat();
                    Console.WriteLine($"\nКот «{currentCat.Name}» готов к взаимодействию.");
                    WaitForKey();
                    break;

                case "2":
                    ShowCatProperties(currentCat);
                    WaitForKey();
                    break;

                case "3":
                    CatMethods(currentCat);
                    break;

                case "4":
                    if (currentCat == null)
                    {
                        Console.WriteLine("Сначала создайте кота (пункт 1).");
                    }
                    else
                    {
                        petHouse.AddAnimal(currentCat);
                        Console.WriteLine($"Кот «{currentCat.Name}» успешно заселён в домик!");
                    }
                    WaitForKey();
                    break;

                case "5":
                    petHouse.ShowAnimals();
                    WaitForKey();
                    break;

                case "6":
                    petHouse.ShowAnimals();

                    if (petHouse.Count > 0)
                    {
                        Console.Write("\nВведите порядковый номер питомца для выселения: ");

                        if (int.TryParse(Console.ReadLine(), out int number))
                        {
                            if (petHouse.RemoveAnimal(number - 1))
                                Console.WriteLine("Питомец успешно покинул домик.");
                            else
                                Console.WriteLine("Ошибка: питомца с таким номером нет в списке.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: введено не число.");
                        }
                    }
                    WaitForKey();
                    break;

                case "0":
                    Console.WriteLine("Программа завершена. До встречи!");
                    return;

                default:
                    Console.WriteLine("Неизвестный пункт меню. Попробуйте снова.");
                    WaitForKey();
                    break;
            }
        }
    }

    static void WaitForKey()
    {
        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    static Cat CreateCat()
    {
        Console.WriteLine("\n--- Создание кота ---");
        Console.WriteLine("1 — Взрослый кот");
        Console.WriteLine("2 — Котёнок");
        Console.Write("Кого создаём? ");

        string type = Console.ReadLine();

        Console.Write("Кличка: ");
        string name = Console.ReadLine();

        int age = ReadInt("Возраст (лет/месяцев): ", min: 0);
        double weight = ReadDouble("Вес (в кг): ", min: 0.1);

        Console.WriteLine("\nВыберите породу:");
        Console.WriteLine("0 — Сиамская");
        Console.WriteLine("1 — Мейн-кун");
        Console.WriteLine("2 — Сфинкс");
        Console.WriteLine("3 — Шотландская вислоухая");

        int breedNumber = ReadInt("Номер породы: ", min: 0, max: 3);
        CatBreed breed = (CatBreed)breedNumber;

        Weight catWeight = new Weight(weight);

        return type == "2"
            ? new Kitten(name, age, catWeight, breed)
            : new Cat(name, age, catWeight, breed);
    }

    static void ShowCatProperties(Cat cat)
    {
        if (cat == null)
        {
            Console.WriteLine("Кот ещё не создан.");
            return;
        }

        string ageSuffix = cat is Kitten ? "мес." : Cat.GetAgeSuffix(cat.Age);

        Console.WriteLine();
        Console.WriteLine("Свойства объекта:");
        Console.WriteLine($"Имя: {cat.Name}");
        Console.WriteLine($"Возраст: {cat.Age} {ageSuffix}");
        Console.WriteLine($"Вес: {cat.Weight}");
        Console.WriteLine($"Порода: {Animal.GetBreedName(cat.Breed)}");
    }

    static void CatMethods(Cat cat)
    {
        if (cat == null)
        {
            Console.WriteLine("Сначала создайте кота.");
            WaitForKey();
            return;
        }

        Console.WriteLine($"\n--- Действия с котом «{cat.Name}» ---");
        Console.WriteLine("1. Поиграть");
        Console.WriteLine("2. Покормить");
        Console.WriteLine("3. Поймать осуждающий взгляд");
        Console.WriteLine("4. Сделать клон кота");
        Console.Write("Ваш выбор: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                cat.Play();
                break;

            case "2":
                cat.Feed();
                break;

            case "3":
                Cat.JudgingLook();
                break;

            case "4":
                Cat clone = cat is Kitten
                    ? new Kitten()
                    : new Cat();
                clone.CopyFrom(cat);

                Console.WriteLine("Успешно создана копия кота:");
                Console.WriteLine(clone);
                break;

            default:
                Console.WriteLine("Неизвестное действие.");
                break;
        }

        WaitForKey();
    }

    static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int val) && val >= min && val <= max)
                return val;

            Console.WriteLine($"Пожалуйста, введите целое число от {min} до {max}.");
        }
    }

    static double ReadDouble(string prompt, double min = 0)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Replace(',', '.');
            if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double val) && val >= min)
                return val;

            Console.WriteLine("Пожалуйста, введите корректное число веса (например, 4.5).");
        }
    }
}