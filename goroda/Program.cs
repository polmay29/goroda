using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static HashSet<string> usedCities = new HashSet<string>(); // Уже использованные города
    static List<string> cities = new List<string>(); // Все города
    static Random rand = new Random(); // Генератор случайных чисел

    static void Main()
    {
        LoadCitiesFromFile("city.txt");

        Console.WriteLine("Выберите режим игры: \n1 - Два игрока \n2 - Игрок против компьютера");
        int mode = int.Parse(Console.ReadLine());

        if (mode == 1)
            TwoPlayerGame();
        else if (mode == 2)
            PlayerVsComputer();
        else
            Console.WriteLine("Неверный режим.");
    }

    static void LoadCitiesFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            cities = File.ReadAllLines(filePath).ToList();
        }
        else
        {
            Console.WriteLine("Файл с городами не найден.");
            Environment.Exit(0);
        }
    }

    static void TwoPlayerGame()
    {
        string lastCity = "";
        int currentPlayer = 1;

        while (true)
        {
            Console.WriteLine($"Игрок {currentPlayer}, назовите город (или введите 'сдаться' для завершения):");
            string playerCity = Console.ReadLine().Trim();

            if (playerCity.ToLower() == "сдаться")
            {
                Console.WriteLine($"Игрок {3 - currentPlayer} победил!");
                break;
            }

            while (!IsValidCity(playerCity, lastCity, currentPlayer))
            {
                Console.WriteLine($"Игрок {currentPlayer}, попробуйте еще раз:");
                playerCity = Console.ReadLine().Trim();

                if (playerCity.ToLower() == "сдаться")
                {
                    Console.WriteLine($"Игрок {3 - currentPlayer} победил!");
                    return;
                }
            }

            usedCities.Add(playerCity.ToLower());
            lastCity = playerCity;

            if (!CitiesAvailable(lastCity))
            {
                Console.WriteLine($"Победила дружба! Все города на букву '{char.ToUpper(lastCity.Last())}' закончились.");
                break;
            }

            currentPlayer = 3 - currentPlayer; // Переключение между игроками
        }
    }

    static void PlayerVsComputer()
    {
        string lastCity = "";
        int currentPlayer = 1;

        while (true)
        {
            if (currentPlayer == 1) // Игрок
            {
                Console.WriteLine("Игрок, назовите город (или введите 'сдаться' для завершения):");
                string playerCity = Console.ReadLine().Trim();

                if (playerCity.ToLower() == "сдаться")
                {
                    Console.WriteLine("Компьютер победил!");
                    break;
                }

                while (!IsValidCity(playerCity, lastCity, currentPlayer))
                {
                    Console.WriteLine("Попробуйте еще раз:");
                    playerCity = Console.ReadLine().Trim();

                    if (playerCity.ToLower() == "сдаться")
                    {
                        Console.WriteLine("Компьютер победил!");
                        return;
                    }
                }

                usedCities.Add(playerCity.ToLower());
                lastCity = playerCity;

                if (!CitiesAvailable(lastCity))
                {
                    Console.WriteLine("Игрок победил! Все города на букву '" + char.ToUpper(lastCity.Last()) + "' закончились.");
                    break;
                }
            }
            else // Компьютер
            {
                char nextLetter = GetNextLetter(lastCity);
                var availableCities = cities
                    .Where(city => !usedCities.Contains(city.ToLower()) && city.StartsWith(nextLetter.ToString(), StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (availableCities.Count > 0)
                {
                    string computerCity = availableCities[rand.Next(availableCities.Count)];
                    Console.WriteLine($"Компьютер называет: {computerCity}");
                    usedCities.Add(computerCity.ToLower());
                    lastCity = computerCity;

                    if (!CitiesAvailable(lastCity))
                    {
                        Console.WriteLine("Компьютер победил! Все города на букву '" + char.ToUpper(lastCity.Last()) + "' закончились.");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Игрок победил! Компьютер не может назвать город.");
                    break;
                }
            }
            currentPlayer = 3 - currentPlayer; // Переключение между игроком и компьютером
        }
    }

    static char GetNextLetter(string city)
    {
        if (string.IsNullOrEmpty(city)) return 'а';

        char lastChar = city[city.Length - 1];

        // Проверяем, является ли последний символ 'ь', 'ъ' или 'ы'
        if (lastChar == 'ь' || lastChar == 'ъ' || lastChar == 'ы')
        {
            // Возвращаем предпоследний символ
            if (city.Length >= 2)
            {
                lastChar = city[city.Length - 2];
            }
            else
            {
                return 'а'; // В случае короткого города, возвращаем 'а'
            }
        }

        return char.ToLower(lastChar);
    }

    static bool IsValidCity(string city, string lastCity, int currentPlayer)
    {
        if (usedCities.Contains(city.ToLower()))
        {
            Console.WriteLine($"Игрок {currentPlayer} проиграл! Город \"{city}\" уже был назван.");
            return false;
        }

        if (!cities.Contains(city, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Игрок {currentPlayer} проиграл! Город \"{city}\" не существует.");
            return false;
        }

        if (!string.IsNullOrEmpty(lastCity))
        {
            char expectedFirstChar = GetNextLetter(lastCity);
            if (char.ToLower(city[0]) != expectedFirstChar)
            {
                Console.WriteLine($"Игрок {currentPlayer} проиграл! Город \"{city}\" не начинается на букву \"{expectedFirstChar}\".");
                return false;
            }
        }

        return true;
    }

    static bool CitiesAvailable(string lastCity)
    {
        if (string.IsNullOrEmpty(lastCity)) return false; // Проверка на пустую строку

        char lastChar = GetNextLetter(lastCity); // Используем GetNextLetter для получения правильного символа

        return cities.Any(city =>
            !usedCities.Contains(city.ToLower()) &&
            city.Length > 0 && // Убедимся, что город не пустой
            char.ToLower(city[0]) == lastChar);
    }
}
