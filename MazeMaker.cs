using System;
using System.Collections.Generic;
using System.Linq;

namespace Mazey
{
    public class MazeMaker
    {
        private readonly Maze maze;
        private readonly Random rng;

        public MazeMaker(Maze maze)
        {
            this.maze = maze;
            rng = new Random();
        }

        public Maze MakeMaze()
        {
            int startRow = rng.Next(maze.Rows);
            int startCol = rng.Next(maze.Cols);
            MakeMaze(maze.GetCell(startRow, startCol));

            maze.AllCells().ForEach(c => c.Mark = 0);

            maze.OpenWall(maze.GetCell(rng.Next(maze.Rows), 0), Direction.Left);
            maze.OpenWall(maze.GetCell(rng.Next(maze.Rows), maze.Cols - 1), Direction.Right);
                
            return maze;
        }

        private void MakeMaze(Cell cell)
        {
            cell.Mark = 1;

            var directions = AvailableDirections(cell).ToList();
            while (directions.Count > 0)
            {
                int index = rng.Next(directions.Count);
                Direction d = directions[index];
                directions.RemoveAt(index);

                if (cell.Go(d).Mark == 0)
                {
                    maze.OpenWall(cell, d);
                    MakeMaze(cell.Go(d));
                }
            }
        }

        private IEnumerable<Direction> AvailableDirections(Cell cell)
        {
            return Maze.Directions().Where(d => cell.Go(d).IsInMaze && cell.Go(d).Mark == 0);
        }
    }
}
