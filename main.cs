using System;
using static System.Console;

class Maze
{
    private int[,] pole; // Поле как приватное поле класса

    public Maze(int rows, int cols)
    {
        pole = new int[rows, cols]; // Создаем массив заданного размера
        InitializePole(); // Инициализируем поле с закрашенными стенками
    }

    // Метод для инициализации поля: стенки закрашены (1), внутри пусто (0)
    private void InitializePole()
    {
        for (int i = 0; i < pole.GetLength(0); i++)
        {
            for (int j = 0; j < pole.GetLength(1); j++)
            {
                // Закрашиваем верхнюю и нижнюю стенки
                if (i == 0 || i == pole.GetLength(0) - 1)
                {
                    pole[i, j] = 1;
                }
                // Закрашиваем левую и правую стенки
                else if (j == 0 || j == pole.GetLength(1) - 1)
                {
                    pole[i, j] = 1;
                }
                else
                {
                    pole[i, j] = 0; // Внутренние клетки пустые
                }
            }
        }
    }

    // Тут метод для вывода поля
    public void PrintPole()
    {
        for (int i = 0; i < pole.GetLength(0); i++)
        {
            for (int j = 0; j < pole.GetLength(1); j++)
            {
                if (pole[i, j] == 0)
                    Write("  ");
                else
                    Write("██");
            }
            WriteLine();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Maze maze = new Maze(25, 25); // Поле 25 х 25

        while (true)
        {
            Clear();
            WriteLine("- - Логическая Змейка - -");
            WriteLine("[1] - Начать");
            WriteLine("[0] - Выход");
            Write("Введите опцию: ");
            string option = ReadLine();

            if (option == "0")
            {
                Clear();
                WriteLine("Выход из программы!");
                WriteLine("Хорошего дня!");
                break;
            }
            else if (option == "1")
            {
                Clear();
                WriteLine("Тут будет игра");
                maze.PrintPole();
                WriteLine("Нажмите любую клавишу для продолжения...");
                ReadKey();
            }
            else
            {
                Clear();
                WriteLine(" Неверная команда!");
                WriteLine("--------------------");
            }
        }
    }
}
