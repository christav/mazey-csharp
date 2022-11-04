using System;
using System.IO;

namespace Mazey
{
    public interface ICharSet
    {
        char[] CornerChars { get; }
        string[] SolutionChars { get; }
    }

    public class UnicodeCharSet : ICharSet
    {
        private static char[] unicodeCorners = new char[] {
            ' ',
            '╹',
            '╺',
            '┗',
            '╻',
            '┃',
            '┏',
            '┣',
            '╸',
            '┛',
            '━',
            '┻',
            '┓',
            '┫',
            '┳',
            '╋'
        };

        private static string[] unicodeSolutions = new[] {
            "   ",
            "   ",
            "   ",
            " ╰┄",
            "   ",
            " ┆ ",
            " ╭┄",
            "   ",
            "   ",
            "┄╯ ",
            "┄┄┄",
            "   ",
            "┄╮ ",
            "   ",
            "   ",
            "   "
        };

        public char[] CornerChars => unicodeCorners;
        public string[] SolutionChars => unicodeSolutions;
    }

    public class AsciiCharSet : ICharSet
    {
        private static char[] asciiCornerChars => new[]
        {
            ' ',
            '+',
            '+',
            '+',
            '+',
            '|',
            '+',
            '+',
            '+',
            '+',
            '-',
            '+',
            '+',
            '+',
            '+',
            '+'
        };

        private static string[] asciiSolutionChars => new[] {
            "   ",
            "   ",
            "   ",
            " XX",
            "   ",
            " X ",
            " XX",
            "   ",
            "   ",
            "XX ",
            "XXX",
            "   ",
            "XX ",
            "   ",
            "   ",
            "   "
        };

        public char[] CornerChars => asciiCornerChars;
        public string[] SolutionChars => asciiSolutionChars;
    }

    public class MazePrinter
    {
        TextWriter output;
        Maze maze;
        Func<Cell, bool> isSolutionCell;
        ICharSet charSet;

        public MazePrinter(TextWriter output, ICharSet charSet, Maze maze, Func<Cell, bool> isSolutionCell)
        {
            this.output = output;
            this.charSet = charSet;
            this.maze = maze;
            this.isSolutionCell = isSolutionCell;
        }

        public void Print()
        {
            for (int row = 0; row < maze.Rows; ++row)
            {
                PrintRowSeparator(row);
                PrintRow(row);
            }
            PrintMazeBottom();
        }

        private char[] CornerChars => charSet.CornerChars;
        private string[] SolutionChars => charSet.SolutionChars;

        private string HorizontalBar
        {
            get { return $"{CornerChars[10]}{CornerChars[10]}{CornerChars[10]}"; }
        }

        private char CornerChar(Cell cell)
        {
            int index = 0;
            index |= cell.Go(Direction.Up).CanGo(Direction.Left) ? 0 : 1;
            index |= cell.CanGo(Direction.Up) ? 0 : 2;
            index |= cell.CanGo(Direction.Left) ? 0 : 4;
            index |= cell.Go(Direction.Left).CanGo(Direction.Up) ? 0 : 8;

            if (cell.Row == 0)
            {
                index &= 0xe;
            }
            if (cell.Col == 0)
            {
                index &= 0x7;
            }

            return CornerChars[index];
        }

        private void PrintRowSeparator(int row)
        {
            foreach (var cell in maze.CellsInRow(row))
            {
                output.Write(CornerChar(cell));
                if (cell.CanGo(Direction.Up))
                {
                    if (isSolutionCell(cell) && isSolutionCell(cell.Go(Direction.Up)))
                    {
                        output.Write(SolutionChars[5]);
                    }
                    else
                    {
                        output.Write("   ");
                    }
                }
                else
                {
                    output.Write(HorizontalBar);
                }
            }
            PrintRowSeparatorEnd(row);
            output.Write("\n");
        }

        private void PrintRowSeparatorEnd(int row)
        {
            var cell = maze.GetCell(row, maze.Cols - 1);
            int index = 0;
            index |= cell.Go(Direction.Up).CanGo(Direction.Right) ? 0 : 1;
            index |= cell.CanGo(Direction.Right) ? 0 : 4;
            index |= cell.CanGo(Direction.Up) ? 0 : 8;

            if (row == 0)
            {
                index &= 0xe;
            }
            output.Write(CornerChars[index]);
        }

        private void PrintRow(int row)
        {
            foreach (var cell in maze.CellsInRow(row))
            {
                if (cell.CanGo(Direction.Left))
                {
                    if (isSolutionCell(cell) && (cell.IsEntrance || isSolutionCell(cell.Go(Direction.Left))))
                    {
                        output.Write(SolutionChars[10][1]);
                    }
                    else
                    {
                        output.Write(" ");
                    }
                }
                else
                {
                    output.Write(CornerChars[5]);
                }
                output.Write(CellContents(cell));
            }

            if (maze.Exit.Row == row)
            {
                if (isSolutionCell(maze.Exit))
                {
                    output.Write(SolutionChars[10][1]);
                }
                else
                {
                    output.Write(" ");
                }
            }
            else
            {
                output.Write(CornerChars[5]);
            }
            output.Write("\n");
        }

        private string CellContents(Cell cell)
        {
            if (!isSolutionCell(cell))
            {
                return "   ";
            }
            int index = 0;
            index |= cell.CanGo(Direction.Up) && isSolutionCell(cell.Go(Direction.Up)) ? 1 : 0;
            index |= cell.CanGo(Direction.Right) && (cell.IsExit || isSolutionCell(cell.Go(Direction.Right))) ? 2 : 0;
            index |= cell.CanGo(Direction.Down) && isSolutionCell(cell.Go(Direction.Down)) ? 4 : 0;
            index |= cell.CanGo(Direction.Left) && (cell.IsEntrance || isSolutionCell(cell.Go(Direction.Left))) ? 8 : 0;
            return SolutionChars[index];
        }

        private void PrintMazeBottom()
        {
            foreach (var cell in maze.CellsInRow(maze.Rows - 1))
            {
                PrintBottomSeparator(cell);
            }
            PrintBottomRightChar();
            output.Write("\n");
        }

        private void PrintBottomSeparator(Cell cell)
        {
            int index = 0xa | (cell.CanGo(Direction.Left) ? 0 : 1);
            if (cell.Col == 0)
            {
                index &= 0x7;
            }
            output.Write(CornerChars[index]);
            output.Write(HorizontalBar);
        }

        private void PrintBottomRightChar()
        {
            var cell = maze.GetCell(maze.Rows - 1, maze.Cols - 1);
            int index = 0x8 | (cell.CanGo(Direction.Right) ? 0 : 1);
            output.Write(CornerChars[index]);
        }
    }
}

