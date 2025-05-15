using System;
using static System.Console;

class Maze
{
    private int[,] pole; // Поле как приватное поле класса

    public Maze(int rows, int cols)
    {
        pole = new int[rows, cols]; // делаем массив заданного размера
    }

    // Метод для вывода поля
    public void PrintPole()
    {
        for (int i = 0; i < pole.GetLength(0); i++)
        {
            for (int j = 0; j < pole.GetLength(1); j++)
            {
                Write(pole[i, j] + " ");
            }
            WriteLine(); 
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        
        Maze maze = new Maze(3, 6);

        while (true)
        {
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
