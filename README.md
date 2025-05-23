# Документация по проекту "Змейка"

## Введение
Проект "Змейка" — это консольная игра, реализованная на языке C# (.NET 6.0 или выше). Игра представляет собой классическую версию "Змейки", где игрок управляет змейкой, собирает еду для увеличения счёта и избегает столкновений со стенами и собственным телом. Проект использует объектно-ориентированный подход с применением классов, наследования, интерфейсов, делегатов, исключений и других возможностей C#. Дополнительно реализованы функции сохранения рекордов в JSON-файл, их отображения и очистки.


## Общая структура проекта
Проект организован с использованием пространств имён для разделения логики:
- **SnakeGame.Entities**: Содержит классы и перечисления для игровых объектов (змейка, стены, еда, координаты).
- **SnakeGame.Core**: Содержит игровую логику, управление рекордами и основной класс игры.
- **SnakeGame**: Содержит точку входа (`Program`), где запускается меню и обрабатываются пользовательские команды.

Код использует следующие ключевые элементы C#:
- **Классы и наследование**: Для представления игровых объектов (`GameObject` как базовый класс).
- **Интерфейсы**: Для обработки столкновений (`ICollidable`).
- **Перечисления**: Для направлений движения (`Direction`).
- **Делегаты и события**: Для обработки окончания игры (`GameOverHandler`).
- **Перегрузка операторов**: Для работы с координатами (`Point`).
- **Обобщения**: Для управления рекордами (`ScoreRecord<T>`).
- **Коллекции**: `LinkedList<Point>` для тела змейки, `List<Wall>` для стен.
- **Исключения**: Для обработки столкновений со стенами (`WallCollisionException`).
- **Сериализация JSON**: Для сохранения рекордов в файл `scores.json`.

## Требования для запуска
- **Среда**: Visual Studio с поддержкой .NET 6.0 или выше.
- **Консоль**: Должна поддерживать Unicode (настраивается через `OutputEncoding = System.Text.Encoding.UTF8`).
- **Файловая система**: Доступ на запись для создания/изменения `scores.json`.

## Установка и запуск
1. Создайте новый проект C# (консольное приложение, .NET 6.0 или выше) в Visual Studio.
2. Скопируйте код в файл `Program.cs`.
3. Скомпилируйте и запустите проект.
4. При запуске создастся файл `scores.json` в папке с исполняемым файлом для хранения рекордов.

## Игровой процесс
- **Меню**:
  - `[1]` — Начать игру.
  - `[2]` — Показать таблицу рекордов.
  - `[3]` — Очистить все рекорды.
  - `[0]` — Выйти из программы.
- **Игра**:
  - Игрок управляет змейкой (`░░`) с помощью стрелок (вверх, вниз, влево, вправо).
  - Змейка движется только при нажатии клавиши (пошаговая механика).
  - Цель: собирать еду (`▒▒`), увеличивая счёт на 10 очков за каждую еду.
  - Змейка растёт при поедании еды.
  - Игра заканчивается при столкновении со стенами (`██`), собственным телом или при достижении максимального размера.
  - После окончания игры игрок вводит имя для сохранения счёта в таблицу рекордов.
- **Рекорды**:
  - Сохраняются в `scores.json`.
  - Для каждого никнейма сохраняется только лучший результат в режиме "Classic".
  - Таблица сортируется по убыванию счёта.

## Символы отображения
Поле игры представляет собой сетку 25x25 клеток, где каждая клетка занимает два символа в консоли для визуальной ширины:
- **Стены**: `██` — границы поля.
- **Пустое пространство**: `  ` (два тонких пробела) — свободные клетки.
- **Змейка**: `░░` — тело и голова змейки.
- **Еда**: `▒▒` — объект, который нужно собрать.

Пример поля:
```
Счет: 10
██████████████████████████████████████████████████
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                                              ██
██                   ░░                         ██
██                   ▒▒                         ██
██████████████████████████████████████████████████
```
![image](https://github.com/user-attachments/assets/7c7cc52c-b8ef-4ff2-ae73-c9ad7d4a8c19)

## Подробное описание кода

### Пространство имён: `SnakeGame.Entities`
Содержит классы и перечисления для игровых объектов.

#### Перечисление: `Direction`
```csharp
public enum Direction { Up, Down, Left, Right }
```
- **Назначение**: Определяет возможные направления движения змейки.
- **Значения**:
  - `Up`: Вверх (уменьшение координаты Y).
  - `Down`: Вниз (увеличение Y).
  - `Left`: Влево (уменьшение X).
  - `Right`: Вправо (увеличение X).
- **Использование**: Хранит текущее направление змейки и используется для вычисления новой позиции головы.

#### Абстрактный класс: `GameObject`
```csharp
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
```
- **Назначение**: Базовый класс для всех игровых объектов (змейка, стены, еда).
- **Свойства**:
  - `X`: Координата по горизонтали (столбец).
  - `Y`: Координата по вертикали (строка).
- **Конструктор**:
  - `GameObject(int x, int y)`: Инициализирует объект с координатами `x` и `y`.
- **Абстрактный метод**:
  - `GetSymbol()`: Возвращает строковое представление объекта (два символа) для отображения в консоли.
- **Логика**: Обеспечивает общую структуру для всех объектов, позволяя хранить их координаты и определять их визуальное представление.

#### Интерфейс: `ICollidable`
```csharp
public interface ICollidable
{
    bool CheckCollision(Snake snake);
}
```
- **Назначение**: Определяет метод для проверки столкновений с змейкой.
- **Метод**:
  - `CheckCollision(Snake snake)`: Возвращает `true`, если змейка столкнулась с объектом, иначе `false`.
- **Использование**: Реализуется классами `Wall` и `Food` для обработки столкновений.

#### Класс: `Wall`
```csharp
public class Wall : GameObject, ICollidable
{
    public Wall(int x, int y) : base(x, y) { }

    public override string GetSymbol() => "██";

    public bool CheckCollision(Snake snake)
    {
        return snake.Head.X == X && snake.Head.Y == Y;
    }
}
```
- **Назначение**: Представляет стену на игровом поле.
- **Наследование**: От `GameObject`, реализует `ICollidable`.
- **Конструктор**:
  - `Wall(int x, int y)`: Инициализирует стену с координатами `x` и `y`.
- **Методы**:
  - `GetSymbol()`: Возвращает `██` — визуальное представление стены.
  - `CheckCollision(Snake snake)`: Проверяет, совпадают ли координаты головы змейки с координатами стены.
- **Логика**: Стены формируют границы поля. Столкновение с ними вызывает исключение `WallCollisionException`.

#### Класс: `Food`
```csharp
public class Food : GameObject, ICollidable
{
    public Food(int x, int y) : base(x, y) { }

    public override string GetSymbol() => "▒▒";

    public bool CheckCollision(Snake snake)
    {
        return snake.Head.X == X && snake.Head.Y == Y;
    }
}
```
- **Назначение**: Представляет еду, которую собирает змейка.
- **Наследование**: От `GameObject`, реализует `ICollidable`.
- **Конструктор**:
  - `Food(int x, int y)`: Инициализирует еду с координатами `x` и `y`.
- **Методы**:
  - `GetSymbol()`: Возвращает `▒▒` — визуальное представление еды.
  - `CheckCollision(Snake snake)`: Проверяет, совпадают ли координаты головы змейки с координатами еды.
- **Логика**: При столкновении с едой змейка растёт, счёт увеличивается на 10, и генерируется новая еда.

#### Класс: `Point`
```csharp
public class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }
}
```
- **Назначение**: Хранит координаты точки на поле.
- **Свойства**:
  - `X`: Координата по горизонтали.
  - `Y`: Координата по вертикали.
- **Конструктор**:
  - `Point(int x, int y)`: Инициализирует точку с координатами `x` и `y`.
- **Перегрузка оператора**:
  - `operator +`: Складывает две точки, создавая новую с суммой координат (`a.X + b.X`, `a.Y + b.Y`).
- **Логика**: Используется для представления сегментов тела змейки и расчёта новых позиций.

#### Класс: `Snake`
```csharp
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

    public void Grow()
    {
        Body.AddLast(new Point(Body.Last.Value.X, Body.Last.Value.Y));
    }

    public void RemoveTail()
    {
        Body.RemoveLast();
    }

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
```
- **Назначение**: Представляет змейку, её движение, рост и проверку столкновений.
- **Наследование**: От `GameObject`.
- **Свойства**:
  - `Body`: `LinkedList<Point>` — список сегментов тела змейки (голова — первый элемент).
  - `CurrentDirection`: Текущее направление движения (`Direction`).
  - `Head`: Свойство, возвращающее координаты головы (первый элемент `Body`).
- **Конструктор**:
  - `Snake(int x, int y)`: Создаёт змейку с одним сегментом в позиции `(x, y)` и направлением вправо.
- **Методы**:
  - `GetSymbol()`: Возвращает `░░` — визуальное представление змейки.
  - `Move()`: Добавляет новую голову в направлении `CurrentDirection`, изменяя координаты на основе направления.
  - `Grow()`: Добавляет новый сегмент в конец тела (копия последнего сегмента).
  - `RemoveTail()`: Удаляет последний сегмент тела.
  - `CheckSelfCollision()`: Проверяет, столкнулась ли голова с любым другим сегментом тела.
- **Логика**:
  - Змейка движется, добавляя новую голову и удаляя хвост (если не растёт).
  - При поедании еды вызывается `Grow()`, и хвост не удаляется.
  - Столкновение с собой завершает игру.

### Пространство имён: `SnakeGame.Core`
Содержит игровую логику и управление рекордами.

#### Директива: `using SnakeGame.Entities`
```csharp
using SnakeGame.Entities;
```
- **Назначение**: Импортирует классы и перечисления (`Point`, `Snake`, `Food`, `Wall`, `Direction`) из `SnakeGame.Entities` для использования в `SnakeGame.Core`.

#### Класс: `WallCollisionException`
```csharp
public class WallCollisionException : Exception
{
    public WallCollisionException(string message) : base(message) { }
}
```
- **Назначение**: Исключение, выбрасываемое при столкновении змейки со стеной.
- **Конструктор**:
  - `WallCollisionException(string message)`: Передаёт сообщение об ошибке в базовый класс `Exception`.
- **Логика**: Используется в методе `Run` для обработки столкновений со стенами.

#### Класс: `ScoreRecord<T>`
```csharp
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
```
- **Назначение**: Обобщённый класс для хранения записи о рекорде игрока.
- **Обобщение**: `T : IComparable<T>` — тип счёта, поддерживающий сравнение (в игре используется `int`).
- **Свойства**:
  - `PlayerName`: Имя игрока.
  - `Score`: Счёт игрока (тип `T`).
  - `GameMode`: Режим игры (в игре — "Classic").
- **Конструктор**:
  - `ScoreRecord(string playerName, T score, string gameMode)`: Инициализирует запись с указанными значениями.
- **Логика**: Хранит данные для таблицы рекордов, используется в `ScoreManager`.

#### Класс: `ScoreManager`
```csharp
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
        var existingScore = _scores.FirstOrDefault(s => s.PlayerName == playerName && s.GameMode == gameMode);
        if (existingScore != null)
        {
            if (score > existingScore.Score)
                existingScore.Score = score;
        }
        else
        {
            _scores.Add(new ScoreRecord<int>(playerName, score, gameMode));
        }

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
```
- **Назначение**: Управляет таблицей рекордов: загрузка, сохранение, отображение и очистка.
- **Поля**:
  - `_filePath`: Путь к файлу `scores.json` для хранения рекордов.
  - `_scores`: Список записей (`List<ScoreRecord<int>>`).
- **Методы**:
  - `LoadScores()`: Читает `scores.json` и десериализует его в `_scores`. Если файла нет, список остаётся пустым.
  - `SaveScore(string playerName, int score, string gameMode)`:
    - Проверяет, есть ли запись для `playerName` в `gameMode`.
    - Если запись есть, обновляет счёт, если новый выше.
    - Если записи нет, добавляет новую.
    - Сортирует `_scores` по убыванию счёта.
    - Сериализует `_scores` в JSON и записывает в `scores.json`.
  - `ClearScores()`: Очищает `_scores` и перезаписывает `scores.json` пустым списком. Выводит сообщение об очистке.
  - `DisplayScores()`: Отображает таблицу рекордов. Если записей нет, выводит "Рекордов пока нет!".
- **Логика**:
  - Обеспечивает персистентность рекордов через JSON.
  - Предотвращает дублирование никнеймов, сохраняя только лучший результат.
  - Сортировка делает таблицу удобной для чтения.

#### Делегат: `GameOverHandler`
```csharp
public delegate void GameOverHandler(int score);
```
- **Назначение**: Определяет сигнатуру метода для обработки события окончания игры.
- **Аргумент**:
  - `score`: Счёт игрока на момент окончания игры.
- **Логика**: Используется для события `OnGameOver` в классе `Game`.

#### Класс: `Game`
```csharp
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
            _walls.Add(new Wall(i, 0));
            _walls.Add(new Wall(i, _height - 1));
        }
        for (int i = 0; i < _height; i++)
        {
            _walls.Add(new Wall(0, i));
            _walls.Add(new Wall(_width - 1, i));
        }
    }

    private void InitializeField()
    {
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                _field[x, y] = "  ";

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
        Render();
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
                        validMove = false;
                        break;
                }

                if (validMove)
                {
                    Point oldHead = new Point(_snake.Head.X, _snake.Head.Y);
                    _snake.Move();

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
                        _field[tail.X, tail.Y] = "  ";
                    }

                    _field[_snake.Head.X, _snake.Head.Y] = _snake.GetSymbol();
                    if (ateFood)
                    {
                        _field[oldHead.X, oldHead.Y] = _snake.GetSymbol();
                        _field[_food.X, _food.Y] = _food.GetSymbol();
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
```
- **Назначение**: Управляет игровым процессом, включая движение змейки, столкновения, рендеринг и счёт.
- **Поля**:
  - `_width`, `_height`: Размеры поля (25x25).
  - `_snake`: Экземпляр змейки.
  - `_walls`: Список стен.
  - `_random`: Генератор случайных чисел для еды.
  - `_food`: Текущая еда.
  - `_score`: Текущий счёт.
  - `_isGameOver`: Флаг окончания игры.
  - `_field`: Массив `string[,]` для хранения символов поля.
  - `OnGameOver`: Событие, вызываемое при завершении игры.
- **Конструктор**:
  - `Game(int width, int height)`: Инициализирует поле, змейку, стены, еду и вызывает `InitializeWalls` и `InitializeField`.
- **Методы**:
  - `InitializeWalls()`: Создаёт стены по периметру поля (верх, низ, лево, право).
  - `InitializeField()`: Заполняет `_field` пустыми клетками (`  `), затем добавляет стены, змейку и еду.
  - `GenerateFood()`: Создаёт еду в случайной позиции, избегая змейки и стен.
  - `Run()`: Основной игровой цикл:
    - Отрисовывает поле (`Render`).
    - Обрабатывает нажатия стрелок, игнорируя противоположные направления.
    - Двигает змейку (`_snake.Move()`).
    - Проверяет столкновения со стенами (выбрасывает исключение) и с собой (завершает игру).
    - При поедании еды вызывает `_snake.Grow()`, увеличивает `_score` на 10 и генерирует новую еду.
    - Если еда не съедена, удаляет хвост и очищает его клетку (`  `).
    - Обновляет поле: новая голова, старый сегмент (при росте), новая еда.
    - Вызывает `OnGameOver` при завершении игры.
  - `Render()`: Отрисовывает поле, используя `SetCursorPosition` для предотвращения мигания.
- **Свойства**:
  - `Score`: Возвращает текущий счёт.
- **Логика**:
  - Управляет всеми аспектами игры: движение, столкновения, рендеринг.
  - Обеспечивает пошаговую механику (движение только по нажатию клавиши).
  - Предотвращает мигание экрана, обновляя только изменённые части.

### Пространство имён: `SnakeGame`
Содержит точку входа программы.

#### Класс: `Program`
```csharp
class Program
{
    static void Main(string[] args)
    {
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
```
- **Назначение**: Точка входа, реализует меню и обработку пользовательского ввода.
- **Метод `Main`**:
  - **Инициализация**:
    - `OutputEncoding = System.Text.Encoding.UTF8`: Настраивает консоль для корректного отображения Unicode-символов (`██`, `░░`, `▒▒`).
    - Создаёт `ScoreManager` и загружает рекорды (`LoadScores`).
  - **Цикл меню**:
    - Очищает экран (`Clear`).
    - Выводит меню с опциями: начать игру, показать рекорды, очистить рекорды, выйти.
    - Читает ввод пользователя (`ReadLine`).
  - **Обработка опций**:
    - `0`: Выводит сообщение о выходе и прерывает цикл.
    - `2`: Вызывает `DisplayScores` для отображения рекордов.
    - `3`: Вызывает `ClearScores` для очистки рекордов.
    - `1`: Запускает игру:
      - Создаёт экземпляр `Game` (поле 25x25).
      - Подписывается на событие `OnGameOver`: выводит счёт, запрашивает имя, сохраняет рекорд.
      - Запускает `game.Run()` в блоке `try`.
      - Обрабатывает `WallCollisionException`: выводит сообщение, счёт, запрашивает имя, сохраняет рекорд.
    - Другое: Выводит сообщение о неверной команде.
- **Логика**:
  - Обеспечивает интерфейс взаимодействия с пользователем.
  - Управляет жизненным циклом игры и рекордов.
  - Обрабатывает исключения для корректного завершения игры.

## Логика взаимодействия компонентов
1. **Запуск программы**:
   - `Program.Main` настраивает консоль и создаёт `ScoreManager`.
   - Загружаются существующие рекорды из `scores.json`.

2. **Меню**:
   - Пользователь выбирает опцию.
   - Опции обрабатываются через условные операторы, вызывая соответствующие методы.

3. **Игра**:
   - Создаётся `Game`, инициализируются змейка, стены, еда и поле.
   - `Run` обрабатывает ввод, двигает змейку, проверяет столкновения, обновляет поле.
   - Поле рендерится без мигания, используя `_field` и `SetCursorPosition`.
   - При поедании еды змейка растёт, счёт увеличивается, генерируется новая еда.
   - При столкновении или самопересечении игра завершается, вызывается `OnGameOver`.

4. **Рекорды**:
   - После игры запрашивается имя, счёт сохраняется через `SaveScore`.
   - Если никнейм существует, обновляется лучший результат.
   - Рекорды сортируются и записываются в `scores.json`.
   - Опция `[2]` отображает таблицу, `[3]` очищает её.

## Особенности реализации
- **Пошаговая механика**: Змейка движется только при нажатии клавиши, что делает игру удобной для новичков.
- **Устранение мигания**: Использование `SetCursorPosition` позволяет обновлять только изменённые клетки.
- **Unicode-символы**: Двухсимвольные клетки (`██`, `  `, `░░`, `▒▒`) создают красивый и читаемый вид поля.
- **Персистентность**: Рекорды сохраняются в `scores.json`, что позволяет сохранять прогресс между запусками.
- **Модульность**: Разделение на пространства имён и классы упрощает поддержку и расширение кода.
- **Обработка ошибок**: Исключения (`WallCollisionException`) обеспечивают корректное завершение игры.

## Возможные улучшения
- Добавить разные режимы игры (например, с увеличивающейся скоростью).
- Ввести ограничение на количество рекордов (например, топ-10).
- Добавить анимации или звуковые эффекты (требует внешних библиотек).
- Реализовать поддержку разных размеров поля через настройки.

## Заключение
Проект "Змейка" демонстрирует использование объектно-ориентированного программирования и возможностей C# для создания интерактивной консольной игры. Код структурирован, модулен и соответствует принципам хорошей практики программирования. Документация охватывает все аспекты реализации, от структуры классов до логики игрового процесса, и может служить руководством для разработчиков, желающих изучить или расширить проект.
