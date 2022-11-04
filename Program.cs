using System;
using System.IO;
using System.Linq;

namespace Mazey
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            var maze = new Maze(20, 20);
            var maker = new MazeMaker(maze);
            maker.MakeMaze();
            var solver = new MazeSolver(maze);
            solver.Solve();
            var printer = new MazePrinter(Console.Out, new UnicodeCharSet(), maze, MazeSolver.IsSolutionCell);
            printer.Print();
        }
    }
}
