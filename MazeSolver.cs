using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mazey
{
    public class MazeSolver
    {
        private readonly Maze maze;

        public MazeSolver(Maze maze)
        {
            this.maze = maze;
        }

        public Maze Solve()
        {

            return maze;
        }

        public static bool IsSolutionCell(Cell cell)
        {
            return false;
        }
    }
}
