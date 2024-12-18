using System;
using System.Linq;

class Program
{
    const int Width = 20;
    const int Height = 10;

    static char[,] maze = new char[Height * 2 + 1, Width * 2 + 1];
    static int playerX = 1, playerY = 1;
    static int direction = 0;

    static void Main()
    {
        GenerateMaze();
        DisplayMaze();
        PlayGame();
    }

    static void GenerateMaze()
    {
        var rand = new Random();

        // Заполняем лабиринт стенами
        for (int y = 0; y < maze.GetLength(0); y++)
            for (int x = 0; x < maze.GetLength(1); x++)
                maze[y, x] = (y % 2 == 0 || x % 2 == 0) ? '|' : '─';

        CarvePath(rand, 1, 1);

        // Добавляем вход и выход
        maze[1, 0] = ' '; // Вход в лабиринт
        maze[maze.GetLength(0) - 2, 1] = ' '; // Выход из лабиринта (левой части)
    }

    static void CarvePath(Random rand, int x, int y)
    {
        int[] directions = { 0, 1, 2, 3 };
        directions = directions.OrderBy(d => rand.Next()).ToArray(); // Случайный порядок

        foreach (var dir in directions)
        {
            int dx = 0, dy = 0;
            switch (dir)
            {
                case 0: dx = 1; break; // Вправо
                case 1: dx = -1; break; // Влево
                case 2: dy = 1; break; // Вниз
                case 3: dy = -1; break; // Вверх
            }

            int newX = x + dx * 2;
            int newY = y + dy * 2;

            if (IsCellInBounds(newX, newY) && maze[newY, newX] == '─')
            {
                maze[y + dy, x + dx] = ' ';
                maze[newY, newX] = ' ';
                CarvePath(rand, newX, newY);
            }
        }
    }

    static bool IsCellInBounds(int x, int y)
    {
        return x > 0 && x < Width * 2 && y > 0 && y < Height * 2; // Проверка на границы
    }

    static void PlayGame()
    {
        while (true)
        {
            DisplayPlayer(); // Отображаем игрока

            // Проверка достижения выхода
            if (playerX == 1 && playerY == maze.GetLength(0) - 2)
            {
                Console.WriteLine("Вы нашли выход! Поздравляем!");
                Console.ReadKey(); // Ожидаем нажатия клавиши перед закрытием
                break; // Заканчиваем игру
            }

            MoveRightHandMethod(); // Логика движения по методу правой руки
            System.Threading.Thread.Sleep(100); // Задержка для наблюдения за движением
        }
    }

    static void MoveRightHandMethod()
    {
        int[,] directions = {
            { 0, -1 },
            { 1, 0 },
            { 0, 1 },
            { -1, 0 }
        };

        for (int i = 0; i < 4; i++)
        {
            int newDir = (direction + 3) % 4; // Поворачиваем вправо
            int newX = playerX + directions[newDir, 0];
            int newY = playerY + directions[newDir, 1];

            if (IsCellInBounds(newX, newY) && (maze[newY, newX] == ' ' || maze[newY, newX] == '.'))
            {
                direction = newDir; // Поворачиваем вправо и двигаемся
                MarkPath(); // Отмечаем пройденный путь
                playerX = newX;
                playerY = newY;
                return;
            }

            // Если не можем повернуть вправо, проверяем вперед
            int forwardX = playerX + directions[direction, 0];
            int forwardY = playerY + directions[direction, 1];

            if (IsCellInBounds(forwardX, forwardY) && (maze[forwardY, forwardX] == ' ' || maze[forwardY, forwardX] == '.'))
            {
                MarkPath(); // Отмечаем пройденный путь
                playerX = forwardX;
                playerY = forwardY;
                return;
            }

            // Если не можем двигаться вперед, поворачиваем влево
            direction = (direction + 1) % 4;
        }
    }

    static void MarkPath()
    {
        // Отметим путь с помощью '.'
        if (maze[playerY, playerX] == ' ')
        {
            maze[playerY, playerX] = '.'; // Отметить пройденный путь
        }
    }

    static void DisplayMaze()
    {

        Console.WriteLine('┌' + new string('─', maze.GetLength(1)) + '┐');

        for (int y = 0; y < maze.GetLength(0); y++)
        {
            Console.Write("│");
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                Console.Write(maze[y, x]);
            }
            Console.WriteLine("│");
        }


        Console.WriteLine('└' + new string('─', maze.GetLength(1)) + '┘');
    }

    static void DisplayPlayer()
    {
        Console.Clear();
        DisplayMaze();

        Console.SetCursorPosition(playerX, playerY + 1);
        Console.Write('O'); // Кружок для игрока
    }
}