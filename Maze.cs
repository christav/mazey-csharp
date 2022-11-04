using System;
using System.Collections.Generic;
using System.Linq;

namespace Mazey
{
    public class Maze
    {
        private class MazeCell : Cell
        {
            private int row;
            private int col;
            private Maze maze;

            public MazeCell(Maze m, int row, int col)
            {
                this.maze = m;
                this.row = row;
                this.col = col;
            }
            public override int Row
            {
                get { return row; }
            }

            public override int Col
            {
                get { return col; }
            }

            public override bool CanGo(Direction d)
            {
                return maze.CanGo(row, col, d);
            }

            public override Cell Go(Direction d)
            {
                return new MazeCell(maze, Maze.RowOffset(row, d), Maze.ColOffset(col, d));
            }

            public override bool IsInMaze
            {
                get { return maze.IsInMaze(row, col); }
            }

            public override int Mark
            {
                get
                {
                    return maze.GetMark(row, col);
                }
                set
                {
                    maze.Mark(row, col, value);
                }
            }

            public override bool IsEntrance
            {
                get { 
                    var entrance = maze.Entrance;
                    return row == entrance.Row && col == entrance.Col;
                }
            }

            public override bool IsExit
            {
                get {
                    var exit = maze.Exit;
                    return row == exit.Row && col == exit.Col;
                }
            }
        }

        private int[,] cells;

        public int Cols { get; private set; }
        public int Rows { get; private set; }

        public Maze(int rows, int cols)
        {
            Cols = cols;
            Rows = rows;
            cells = new int[rows * 2 + 1, cols * 2 + 1];
            for (int r = 0; r < rows * 2 + 1; ++r)
            {
                for (int c = 0; c < cols * 2 + 1; ++c)
                {
                    cells[r, c] = 1;
                }
            }
            AllCells().ForEach(c => c.Mark = 0);
        }

        public Cell Entrance
        {
            get
            {
                return new MazeCell(this, Enumerable.Range(0, Rows).First(r => CanGo(r, 0, Direction.Left)), 0);
            }
        }

        public Cell Exit
        {
            get
            {
                return new MazeCell(this, Enumerable.Range(0, Rows).First(r => CanGo(r, Cols - 1, Direction.Right)), Cols - 1);
            }
        }
        public IEnumerable<IEnumerable<Cell>> AllRows()
        {
            for (int r = 0; r < Rows; ++r)
            {
                yield return CellsInRow(r);
            }
        }

        public IEnumerable<Cell> CellsInRow(int r)
        {
            for (int c = 0; c < Cols; ++c)
            {
                yield return GetCell(r, c);
            }
        }

        public IEnumerable<Cell> AllCells()
        {
            foreach (var row in AllRows())
            {
                foreach (var cell in row)
                {
                    yield return cell;
                }
            }
        }

        public Cell GetCell(int row, int col)
        {
            return new MazeCell(this, row, col);
        }

        private bool CanGo(int row, int col, Direction direction)
        {
            int y = RowToIndex(row, direction);
            int x = ColToIndex(col, direction);

            return IsInMaze(row, col) &&
                   cells[y, x] == 0;
        }

        public void OpenWall(Cell cell, Direction direction)
        {
            int y = RowToIndex(cell.Row, direction);
            int x = ColToIndex(cell.Col, direction);
            cells[y, x] = 0;
        }

        public void CloseWall(Cell cell, Direction direction)
        {
            cells[RowToIndex(cell.Row, direction), ColToIndex(cell.Col, direction)] = 1;
        }

        private void Mark(int row, int col, int mark, Direction direction = Direction.None)
        {
            cells[RowToIndex(RowOffset(row, direction), Direction.None), ColToIndex(ColOffset(col, direction), Direction.None)] = mark;
        }

        private int GetMark(int row, int col, Direction direction = Direction.None)
        {
            return cells[RowToIndex(RowOffset(row, direction), Direction.None), ColToIndex(ColOffset(col, direction), Direction.None)];
        }

        private bool IsInMaze(int row, int col, Direction direction = Direction.None)
        {
            row = RowOffset(row, direction);
            col = ColOffset(col, direction);

            return (row >= 0 && row < Rows) &&
                   (col >= 0 && col < Cols);
        }

        public static IEnumerable<Direction> Directions()
        {
            yield return Direction.Up;
            yield return Direction.Left;
            yield return Direction.Down;
            yield return Direction.Right;
        }

        private int RowToIndex(int row, Direction direction)
        {
            int y = row * 2 + 1;

            switch (direction)
            {
                case Direction.None:
                    return y;
                case Direction.Up:
                    return y - 1;
                case Direction.Left:
                    return y;
                case Direction.Down:
                    return y + 1;
                case Direction.Right:
                    return y;
            }
            throw new Exception("Unknown direction");
        }

        private int ColToIndex(int col, Direction direction)
        {
            int x = col * 2 + 1;

            switch (direction)
            {
                case Direction.None:
                    return x;
                case Direction.Up:
                    return x;
                case Direction.Left:
                    return x - 1;
                case Direction.Down:
                    return x;
                case Direction.Right:
                    return x + 1;
            }

            throw new Exception("Unknown direction");
        }

        private static int RowOffset(int row, Direction direction)
        {
            switch (direction)
            {
                case Direction.None:
                case Direction.Left:
                case Direction.Right:
                    return row;

                case Direction.Up:
                    return row - 1;

                case Direction.Down:
                    return row + 1;

                default:
                    throw new Exception("Unknown direction");
            }
        }
        private static int ColOffset(int col, Direction direction)
        {
            switch (direction)
            {
                case Direction.None:
                case Direction.Up:
                case Direction.Down:
                    return col;

                case Direction.Left:
                    return col - 1;

                case Direction.Right:
                    return col + 1;

                default:
                    throw new Exception("Unknown direction");
            }
        }
    }
}
