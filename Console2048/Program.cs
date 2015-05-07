using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Console2048
{
    public class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();

            bool result = true;
            game.Run();
            while (result)
            {
                var key = Console.ReadKey();
                ProcessDirection direction;
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        direction = ProcessDirection.Up;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        direction = ProcessDirection.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        direction = ProcessDirection.Left;
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        direction = ProcessDirection.Right;
                        break;

                    case ConsoleKey.Enter:
                        direction = ProcessDirection.None;
                        game.Run();
                        break;
                    case ConsoleKey.Escape:
                        direction = ProcessDirection.None;
                        result = false;
                        break;
                    case ConsoleKey.D1:
                        direction = ProcessDirection.None;
                        game.FieldSize = 4;
                        game.Run();
                        break;
                    case ConsoleKey.D2:
                        direction = ProcessDirection.None;
                        game.FieldSize = 6;
                        game.Run();
                        break;
                    case ConsoleKey.D3:
                        direction = ProcessDirection.None;
                        game.FieldSize = 8;
                        game.Run();
                        break;
                    case ConsoleKey.D4:
                        direction = ProcessDirection.None;
                        game.FieldSize = 10;
                        game.Run();
                        break;
                    case ConsoleKey.D5:
                        direction = ProcessDirection.None;
                        game.FieldSize = 15;
                        game.Run();
                        break;
                    default:
                        direction = ProcessDirection.None;
                        break;
                }

                game.Step(direction);
            }
        }
    }

    public enum ProcessDirection
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    public class Game
    {
        public int FieldSize = 4;
        private int[,] _field;
        private bool _hasMoves;
        private bool _won;
        private bool _over;

        private void PrintGame()
        {
            if (_won)
            {
                PrintGameWon();
                return;
            }

            if (_over)
            {
                PrintGameOver();
                return;
            }

            Console.Clear();
            Console.WriteLine("2048 CONSOLE VERSION");
            Console.WriteLine("--------------------------");

            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    Console.ForegroundColor = GetNumberColor(_field[i, j]);
                    Console.Write(String.Format("{0, -5}", _field[i, j]));
                    Console.ResetColor();
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("← ↑ → ↓ or WASD for control, Enter - restart, Esc - exit");
            Console.WriteLine("1, 2, 3, 4, 5 for more madness");
        }

        public void Run()
        {
            _over = false;
            InitializeField();
            PrintGame();
        }

        public void Step(ProcessDirection direction)
        {
            if (_over)
                PrintGameOver();

            _hasMoves = false;
            if (direction == ProcessDirection.None)
                return;

            Move(direction);
            Merge(direction);

            CheckWin();
            CheckOver();


            if (!_won && !_over)
                AddNewElement();

            PrintGame();
        }

        private void Move(ProcessDirection direction)
        {
            switch (direction)
            {
                case ProcessDirection.Left:
                    for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
                    {
                        for (int colIndex = 1; colIndex < FieldSize; colIndex++)
                        {
                            if (_field[rowIndex, colIndex] == 0)
                                continue;

                            int colIndexMoving = colIndex;
                            while (colIndexMoving != 0 && _field[rowIndex, colIndexMoving - 1] == 0)
                            {
                                _field[rowIndex, colIndexMoving - 1] = _field[rowIndex, colIndexMoving];
                                _field[rowIndex, colIndexMoving] = 0;

                                colIndexMoving--;
                                _hasMoves = true;
                            }
                        }
                    }
                    break;
                case ProcessDirection.Right:
                    for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
                    {
                        for (int colIndex = FieldSize - 1; colIndex >= 0; colIndex--)
                        {
                            if (_field[rowIndex, colIndex] == 0)
                                continue;

                            int colIndexMoving = colIndex;
                            while (colIndexMoving != FieldSize - 1 && _field[rowIndex, colIndexMoving + 1] == 0)
                            {
                                _field[rowIndex, colIndexMoving + 1] = _field[rowIndex, colIndexMoving];
                                _field[rowIndex, colIndexMoving] = 0;

                                colIndexMoving++;
                                _hasMoves = true;
                            }
                        }
                    }
                    break;
                case ProcessDirection.Up:
                    for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                    {
                        for (int rowIndex = 1; rowIndex < FieldSize; rowIndex++)
                        {
                            if (_field[rowIndex, colIndex] == 0)
                                continue;

                            int rowIndexMoving = rowIndex;
                            while (rowIndexMoving != 0 && _field[rowIndexMoving - 1, colIndex] == 0)
                            {
                                _field[rowIndexMoving - 1, colIndex] = _field[rowIndexMoving, colIndex];
                                _field[rowIndexMoving, colIndex] = 0;

                                rowIndexMoving--;
                                _hasMoves = true;
                            }
                        }
                    }
                    break;
                case ProcessDirection.Down:
                    for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                    {
                        for (int rowIndex = FieldSize - 1; rowIndex >= 0; rowIndex--)
                        {
                            if (_field[rowIndex, colIndex] == 0)
                                continue;

                            int rowIndexMoving = rowIndex;
                            while (rowIndexMoving != FieldSize - 1 && _field[rowIndexMoving + 1, colIndex] == 0)
                            {
                                _field[rowIndexMoving + 1, colIndex] = _field[rowIndexMoving, colIndex];
                                _field[rowIndexMoving, colIndex] = 0;

                                rowIndexMoving++;
                                _hasMoves = true;
                            }
                        }
                    }
                    break;
            }
        }

        private void Merge(ProcessDirection direction)
        {
            switch (direction)
            {
                case ProcessDirection.Left:
                    for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
                    {
                        for (int colIndex = FieldSize - 1; colIndex > 0; colIndex--)
                        {
                            if (_field[rowIndex, colIndex] == 0 || colIndex == 0)
                                continue;

                            if (_field[rowIndex, colIndex - 1] == _field[rowIndex, colIndex])
                            {
                                _field[rowIndex, colIndex - 1] *= 2;
                                _field[rowIndex, colIndex] = 0;

                                _hasMoves = true;
                            }
                        }
                    }
                    break;
                case ProcessDirection.Right:
                    for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
                    {
                        for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                        {
                            if (_field[rowIndex, colIndex] == 0 || colIndex == FieldSize - 1)
                                continue;

                            if (_field[rowIndex, colIndex + 1] == _field[rowIndex, colIndex])
                            {
                                _field[rowIndex, colIndex + 1] *= 2;
                                _field[rowIndex, colIndex] = 0;

                                _hasMoves = true;
                            }
                        }
                    }
                    break;
                case ProcessDirection.Up:
                    for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                    {
                        for (int rowIndex = FieldSize - 1; rowIndex > 0; rowIndex--)
                        {
                            if (_field[rowIndex, colIndex] == 0 || rowIndex == 0)
                                continue;

                            if (_field[rowIndex - 1, colIndex] == _field[rowIndex, colIndex])
                            {
                                _field[rowIndex - 1, colIndex] *= 2;
                                _field[rowIndex, colIndex] = 0;

                                _hasMoves = true;
                            }
                        }
                    }
                    break;
                case ProcessDirection.Down:
                    for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                    {
                        for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
                        {
                            if (_field[rowIndex, colIndex] == 0 || rowIndex == FieldSize - 1)
                                continue;

                            if (_field[rowIndex + 1, colIndex] == _field[rowIndex, colIndex])
                            {
                                _field[rowIndex + 1, colIndex] *= 2;
                                _field[rowIndex, colIndex] = 0;

                                _hasMoves = true;
                            }
                        }
                    }
                    break;
            }
        }

        private void AddNewElement()
        {
            var random = new Random();

            int retryCounts = 0;

            int newI = random.Next(0, FieldSize - 1);
            int newJ = random.Next(0, FieldSize - 1);
            while (retryCounts < 5 && _field[newI, newJ] != 0)
            {
                newI = random.Next(0, FieldSize - 1);
                newJ = random.Next(0, FieldSize - 1);

                retryCounts++;
            }

            if (retryCounts == 5)
                for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                    for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
                        if (_field[colIndex, rowIndex] == 0)
                        {
                            newI = colIndex;
                            newJ = rowIndex;
                        }

            if (random.Next(0, 9) == 5)
                _field[newI, newJ] = 4;
            else
                _field[newI, newJ] = 2;
        }

        private void CheckWin()
        {
            for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
                for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                    if (_field[rowIndex, colIndex] == 2048)
                    {
                        _won = true;
                    }
        }

        private void CheckOver()
        {
            if (!HasEmptyTiles() || !MergesAvailable())
                _over = true;
        }

        private bool HasEmptyTiles()
        {
            bool result = false;
            for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
                for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                    if (_field[rowIndex, colIndex] == 0)
                    {
                        result = true;
                    }

            return result;
        }

        //check for empty tiles and available merges
        private bool MergesAvailable()
        {
            //backup current field
            int[,] backupField = new int[FieldSize, FieldSize];
            for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
                for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                {
                    backupField[rowIndex, colIndex] = _field[rowIndex, colIndex];
                }

            Move(ProcessDirection.Up);

            bool result = false;
            for (int rowIndex = 0; rowIndex < FieldSize - 1; rowIndex++)
            {
                for (int colIndex = 0; colIndex < FieldSize - 1; colIndex++)
                {
                    if (_field[rowIndex, colIndex] == _field[rowIndex, colIndex + 1] || _field[rowIndex, colIndex] == _field[rowIndex + 1, colIndex])
                    {
                        result = true;
                        break;
                    }
                }
            }

            _field = backupField;

            return result;
        }

        private void InitializeField()
        {
            _field = new int[FieldSize, FieldSize];
            for (int rowIndex = 0; rowIndex < FieldSize; rowIndex++)
            {
                for (int colIndex = 0; colIndex < FieldSize; colIndex++)
                {
                    _field[rowIndex, colIndex] = 0;
                }
            }

            var random = new Random();
            var initialTiles = random.Next(2, 3);
            for (int i = 0; i < initialTiles; i++)
            {
                AddNewElement();
            }

        }

        private ConsoleColor GetNumberColor(int value)
        {
            ConsoleColor result;
            switch (value)
            {
                case 2:
                    result = ConsoleColor.Yellow;
                    break;
                case 4:
                    result = ConsoleColor.Magenta;
                    break;
                case 8:
                    result = ConsoleColor.Green;
                    break;
                case 16:
                    result = ConsoleColor.Blue;
                    break;
                case 32:
                    result = ConsoleColor.Cyan;
                    break;
                case 64:
                    result = ConsoleColor.Red;
                    break;
                case 128:
                    result = ConsoleColor.DarkYellow;
                    break; ;
                case 256:
                    result = ConsoleColor.DarkMagenta;
                    break;
                case 512:
                    result = ConsoleColor.DarkGreen;
                    break;
                case 1024:
                    result = ConsoleColor.DarkBlue;
                    break;
                case 2048:
                    result = ConsoleColor.DarkCyan;
                    break;
                default:
                    result = ConsoleColor.Gray;
                    break;
            }
            return result;
        }

        private void PrintGameWon()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(@"                                              (`\ .-') /`            .-') _  ");
            Console.WriteLine(@"                                               `.( OO ),'           ( OO ) ) ");
            Console.WriteLine(@"  ,--.   ,--..-'),-----.  ,--. ,--.         ,--./  .--.  ,-.-') ,--./ ,--,'  ");
            Console.WriteLine(@"   \  `.'  /( OO'  .-.  ' |  | |  |         |      |  |  |  |OO)|   \ |  |\  ");
            Console.WriteLine(@" .-')     / /   |  | |  | |  | | .-')       |  |   |  |, |  |  \|    \|  | ) ");
            Console.WriteLine(@"(OO  \   /  \_) |  |\|  | |  |_|( OO )      |  |.'.|  |_)|  |(_/|  .     |/  ");
            Console.WriteLine(@" |   /  /\_   \ |  | |  | |  | | `-' /      |         | ,|  |_.'|  |\    |   ");
            Console.WriteLine(@" `-./  /.__)   `'  '-'  '('  '-'(_.-'       |   ,'.   |(_|  |   |  | \   |   ");
            Console.WriteLine(@"   `--'          `-----'   `-----'          '--'   '--'  `--'   `--'  `--'   ");

            Console.ResetColor();
        }

        private void PrintGameOver()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(@"                   *              )               (     ");
            Console.WriteLine(@" (        (      (  `          ( /(               )\ )  ");
            Console.WriteLine(@" )\ )     )\     )\))(   (     )\()) (   (   (   (()/(  ");
            Console.WriteLine(@"(()/(  ((((_)(  ((_)()\  )\   ((_)\  )\  )\  )\   /(_)) ");
            Console.WriteLine(@" /(_))_ )\ _ )\ (_()((_)((_)    ((_)((_)((_)((_) (_))   ");
            Console.WriteLine(@"(_)) __|(_)_\(_)|  \/  || __|  / _ \\ \ / / | __|| _ \  ");
            Console.WriteLine(@"  | (_ | / _ \  | |\/| || _|  | (_) |\ V /  | _| |   /  ");
            Console.WriteLine(@"   \___|/_/ \_\ |_|  |_||___|  \___/  \_/   |___||_|_\  ");
            Console.WriteLine();
            Console.WriteLine("Enter - restart. Escape - exit");

            Console.ResetColor();
        }
    }
}
