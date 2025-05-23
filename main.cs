using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Console;

// Пространство имен для игровых объектов
namespace SnakeGame.Entities
{
    // Перечисление для направлений движения
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    // Базовый класс для игровых объектов
    public abstract class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }

        protected GameObject(int x, int y)
        {
            X = x;
            Y = y;
        }

        public abstract string GetSymbol();
    }

    // Интерфейс для обработки столкновений
    public interface ICollidable
    {
        bool CheckCollision(Snake snake);
    }

    // Класс стены
    public class Wall : GameObject, ICollidable
    {
        public Wall(int x, int y) : base(x, y) { }

        public override string GetSymbol() => "██";

        public bool CheckCollision(Snake snake)
        {
            return snake.Head.X == X && snake.Head.Y == Y;
        }
    }

    // Класс еды
    public class Food : GameObject, ICollidable
    {
        public Food(int x, int y) : base(x, y) { }

        public override string GetSymbol() => "▒▒";

        public bool CheckCollision(Snake snake)
        {
            return snake.Head.X == X && snake.Head.Y == Y;
        }
    }

    // Класс для координат (используется в змейке)
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Перегрузка оператора + для добавления сегмента змейки
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
    }

    // Класс змейки
    public class Snake : GameObject
    {
        public LinkedList<Point> Body { get; } = new LinkedList<Point>();
        public Direction CurrentDirection { get; set; }
        public Point Head => Body.First.Value;

        public Snake(int x, int y) : base(x, y)
        {
            Body.AddFirst(new Point(x, y));
            CurrentDirection = Direction.Right;
        }

        public override string GetSymbol() => "░░";

        // Перемещение змейки
        public bool Move()
        {
            Point newHead = new Point(Head.X, Head.Y);
            switch (CurrentDirection)
            {
                case Direction.Up: newHead.Y--; break;
                case Direction.Down: newHead.Y++; break;
                case Direction.Left: newHead.X--; break;
                case Direction.Right: newHead.X++; break;
            }
            Body.AddFirst(newHead);
            return true;
        }

        // Увеличение длины при поедании еды
        public void Grow()
        {
            Body.AddLast(new Point(Body.Last.Value.X, Body.Last.Value.Y));
        }

        // Удаление хвоста при движении
        public void RemoveTail()
        {
            Body.RemoveLast();
        }

        // Проверка столкновения с собой
        public bool CheckSelfCollision()
        {
            var head = Body.First;
            var current = head.Next;
            while (current != null)
            {
                if (head.Value.X == current.Value.X && head.Value.Y == current.Value.Y)
                    return true;
                current = current.Next;
            }
            return false;
        }
    }
}

// Пространство имен для игровой логики
namespace SnakeGame.Core
{
    using SnakeGame.Entities; // Для доступа к Point, Snake, Food и т.д.

    // Исключение для столкновения со стеной
    public class WallCollisionException : Exception
    {
        public WallCollisionException(string message) : base(message) { }
    }

    // Обобщенный класс для рекордов
    public class ScoreRecord<T> where T : IComparable<T>
    {
        public string PlayerName { get; set; }
        public T Score { get; set; }
        public string GameMode { get; set; }

        public ScoreRecord(string playerName, T score, string gameMode)
        {
            PlayerName = playerName;
            Score = score;
            GameMode = gameMode;
        }
    }

    // Класс для управления рекордами
    public class ScoreManager
    {
        private readonly string _filePath = "scores.json";
        private List<ScoreRecord<int>> _scores = new List<ScoreRecord<int>>();

        public void LoadScores()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _scores = JsonSerializer.Deserialize<List<ScoreRecord<int>>>(json) ?? new List<ScoreRecord<int>>();
            }
        }

        public void SaveScore(string playerName, int score, string gameMode)
        {
            // Проверяем, есть ли уже запись для игрока в этом режиме
            var existingScore = _scores.FirstOrDefault(s => s.PlayerName == playerName && s.GameMode == gameMode);
            if (existingScore != null)
            {
                // Обновляем, если новый результат лучше
                if (score > existingScore.Score)
                    existingScore.Score = score;
            }
            else
            {
                // Добавляем новую запись
                _scores.Add(new ScoreRecord<int>(playerName, score, gameMode));
            }

            // Сортируем по убыванию счета
            _scores = _scores.OrderByDescending(s => s.Score).ToList();
            string json = JsonSerializer.Serialize(_scores, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void ClearScores()
        {
            _scores.Clear();
            if (File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, JsonSerializer.Serialize(_scores, new JsonSerializerOptions { WriteIndented = true }));
            }
            Clear();
            WriteLine("Все рекорды очищены!");
            WriteLine("\nНажмите любую клавишу...");
            ReadKey();
        }

        public void DisplayScores()
        {
            Clear();
            WriteLine("--- Таблица рекордов ---");
            if (_scores.Count == 0)
            {
                WriteLine("Рекордов пока нет!");
            }
            else
            {
                foreach (var score in _scores)
                {
                    WriteLine($"Игрок: {score.PlayerName}, Счет: {score.Score}, Режим: {score.GameMode}");
                }
            }
            WriteLine("\nНажмите любую клавишу...");
            ReadKey();
        }
    }

    // Делегат и событие для завершения игры
    public delegate void GameOverHandler(int score);

    // Основной класс игры
    public class Game
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Snake _snake;
        private readonly List<Wall> _walls;
        private readonly Random _random;
        private Food _food;
        private int _score;
        private bool _isGameOver;
        private string[,] _field;
        public event GameOverHandler OnGameOver;

        public Game(int width, int height)
        {
            _width = width;
            _height = height;
            _snake = new Snake(width / 2, height / 2);
            _walls = new List<Wall>();
            _random = new Random();
            _food = GenerateFood();
            _score = 0;
            _isGameOver = false;
            _field = new string[_width, _height];
            InitializeWalls();
            InitializeField();
        }

        private void InitializeWalls()
        {
            for (int i = 0; i < _width; i++)
            {
                _walls.Add(new Wall(i, 0)); // Верхняя стена
                _walls.Add(new Wall(i, _height - 1)); // Нижняя стена
            }
            for (int i = 0; i < _height; i++)
            {
                _walls.Add(new Wall(0, i)); // Левая стена
                _walls.Add(new Wall(_width - 1, i)); // Правая стена
            }
        }

        private void InitializeField()
        {
            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                    _field[x, y] = "  "; // Пустое пространство — два тонких пробела

            foreach (var wall in _walls)
                _field[wall.X, wall.Y] = wall.GetSymbol();

            foreach (var point in _snake.Body)
                _field[point.X, point.Y] = _snake.GetSymbol();

            _field[_food.X, _food.Y] = _food.GetSymbol();
        }

        private Food GenerateFood()
        {
            int x, y;
            do
            {
                x = _random.Next(1, _width - 1);
                y = _random.Next(1, _height - 1);
            } while (_snake.Body.Any(p => p.X == x && p.Y == y) || _walls.Any(w => w.X == x && w.Y == y));
            return new Food(x, y);
        }

        public int Score => _score;

        public void Run()
        {
            Render(); // Отрисовываем начальное состояние
            while (!_isGameOver)
            {
                if (KeyAvailable)
                {
                    var key = ReadKey(true).Key;
                    bool validMove = true;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            if (_snake.CurrentDirection != Direction.Down)
                                _snake.CurrentDirection = Direction.Up;
                            else
                                validMove = false;
                            break;
                        case ConsoleKey.DownArrow:
                            if (_snake.CurrentDirection != Direction.Up)
                                _snake.CurrentDirection = Direction.Down;
                            else
                                validMove = false;
                            break;
                        case ConsoleKey.LeftArrow:
                            if (_snake.CurrentDirection != Direction.Right)
                                _snake.CurrentDirection = Direction.Left;
                            else
                                validMove = false;
                            break;
                        case ConsoleKey.RightArrow:
                            if (_snake.CurrentDirection != Direction.Left)
                                _snake.CurrentDirection = Direction.Right;
                            else
                                validMove = false;
                            break;
                        default:
                            validMove = false; // Игнорируем другие клавиши
                            break;
                    }

                    if (validMove)
                    {
                        Point oldHead = new Point(_snake.Head.X, _snake.Head.Y);
                        _snake.Move();

                        // Проверка столкновений
                        if (_walls.Any(w => w.CheckCollision(_snake)))
                        {
                            throw new WallCollisionException("Змейка врезалась в стену!");
                        }

                        if (_snake.CheckSelfCollision())
                        {
                            _isGameOver = true;
                        }

                        bool ateFood = _food.CheckCollision(_snake);
                        if (ateFood)
                        {
                            _snake.Grow();
                            _score += 10;
                            _food = GenerateFood();
                        }
                        else
                        {
                            Point tail = _snake.Body.Last.Value;
                            _snake.RemoveTail();
                            _field[tail.X, tail.Y] = "  "; // Очищаем хвост
                        }

                        // Обновляем поле
                        _field[_snake.Head.X, _snake.Head.Y] = _snake.GetSymbol(); // Новая голова
                        if (ateFood)
                        {
                            _field[oldHead.X, oldHead.Y] = _snake.GetSymbol(); // Старый сегмент
                            _field[_food.X, _food.Y] = _food.GetSymbol(); // Новая еда
                        }

                        Render();
                    }
                }
            }

            OnGameOver?.Invoke(_score);
        }

        private void Render()
        {
            SetCursorPosition(0, 0);
            Write($"Счет: {_score}".PadRight(WindowWidth));
            for (int y = 0; y < _height; y++)
            {
                SetCursorPosition(0, y + 1);
                for (int x = 0; x < _width; x++)
                    Write(_field[x, y]);
                WriteLine();
            }
        }
    }
}

// Главная программа
namespace SnakeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Настройка консоли для корректного отображения Unicode
            OutputEncoding = System.Text.Encoding.UTF8;

            SnakeGame.Core.ScoreManager scoreManager = new SnakeGame.Core.ScoreManager();
            scoreManager.LoadScores();

            while (true)
            {
                Clear();
                WriteLine("- - Змейка - -");
                WriteLine("[1] - Начать игру");
                WriteLine("[2] - Показать рекорды");
                WriteLine("[3] - Очистить рекорды");
                WriteLine("[0] - Выход");
                Write("Выберите опцию: ");
                string option = ReadLine();

                if (option == "0")
                {
                    Clear();
                    WriteLine("Выход из программы!");
                    break;
                }
                else if (option == "2")
                {
                    scoreManager.DisplayScores();
                }
                else if (option == "3")
                {
                    scoreManager.ClearScores();
                }
                else if (option == "1")
                {
                    Clear();
                    SnakeGame.Core.Game game = new SnakeGame.Core.Game(25, 25);
                    game.OnGameOver += (score) =>
                    {
                        Clear();
                        WriteLine($"Игра окончена! Ваш счет: {score}");
                        Write("Введите ваше имя: ");
                        string playerName = ReadLine();
                        scoreManager.SaveScore(playerName, score, "Classic");
                        WriteLine("Нажмите любую клавишу...");
                        ReadKey();
                    };

                    try
                    {
                        game.Run();
                    }
                    catch (SnakeGame.Core.WallCollisionException ex)
                    {
                        Clear();
                        WriteLine(ex.Message);
                        WriteLine($"Игра окончена! Ваш счет: {game.Score}");
                        Write("Введите ваше имя: ");
                        string playerName = ReadLine();
                        scoreManager.SaveScore(playerName, game.Score, "Classic");
                        WriteLine("Нажмите любую клавишу...");
                        ReadKey();
                    }
                }
                else
                {
                    Clear();
                    WriteLine("Неверная команда!");
                    WriteLine("Нажмите любую клавишу...");
                    ReadKey();
                }
            }
        }
    }
}
