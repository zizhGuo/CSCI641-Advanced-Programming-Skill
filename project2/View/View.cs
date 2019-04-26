using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sudoku.Model;

namespace sudoku.View
{
    /// <summary>
    /// This interface is used for observer class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObserver<T>
    {
        void Notify(T subject);
    }
    /// <summary>
    /// View model only for printing out the sudoku solution and board in given style.
    /// Every view is also a observer.
    /// </summary>
    class View : IObserver<Sudoku>
    {
        // Contructor to register the subject.
        public View(Sudoku sudoku)
        {
            sudoku.Register(this);
        }
        /// <summary>
        /// Notification if any changed reported from the subject in observation.
        /// </summary>
        /// <param name="sudoku"></param>
        public void Notify(Sudoku sudoku)
        {
            ViewSudoku(sudoku);
        }
        /// <summary>
        /// Printing method.
        /// </summary>
        /// <param name="sudoku"></param>
        public void ViewSudoku(Sudoku sudoku)
        {
            int size = sudoku.Size;
            int[,] solution = sudoku.SudokuSolution;
            int[,] board = sudoku.Sudokuboard;
            Console.WriteLine("Sudoku generation done! ");
            Console.WriteLine("Size: " + sudoku.Size + " Mode: " + sudoku.Mode);
            Console.WriteLine("Solution:");
            if (size == 4)
            {
                for (int i = 0; i < size; i++)
                {
                    if (i % 2 == 0)
                    {
                        Console.WriteLine("------+------");
                    }
                    for (int j = 0; j < size; j++)
                    {
                        if (j % 2 == 0)
                        {
                            Console.Write("| " + solution[i, j] + " ");
                        }
                        else
                        {
                            Console.Write(solution[i, j] + " ");
                        }

                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("------+------");
                Console.WriteLine("");
                Console.WriteLine("Game:");

                for (int i = 0; i < size; i++)
                {
                    if (i % 2 == 0)
                    {
                        Console.WriteLine("------+------");
                    }
                    for (int j = 0; j < size; j++)
                    {
                        if (j % 2 == 0)
                        {
                            if (board[i, j] == -1)
                            {
                                Console.Write("| X ");
                            }
                            else Console.Write("| " + board[i, j] + " ");
                        }
                        else
                        {
                            if (board[i, j] == -1)
                            {
                                Console.Write("X ");
                            }
                            else
                            {
                                Console.Write(board[i, j] + " ");
                            }
                        }

                    }
                    Console.WriteLine("|");

                }
                Console.WriteLine("------+------");
            }

            if (size == 9)
            {
                for (int i = 0; i < size; i++)
                {
                    if (i % 3 == 0)
                    {
                        Console.WriteLine("--------+-------+--------");
                    }
                    for (int j = 0; j < size; j++)
                    {
                        if (j % 3 == 0)
                        {
                            Console.Write("| " + solution[i, j] + " ");
                        }
                        else
                        {
                            Console.Write(solution[i, j] + " ");
                        }

                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("--------+-------+--------");
                Console.WriteLine("");
                Console.WriteLine("Game:");

                for (int i = 0; i < size; i++)
                {
                    if (i % 3 == 0)
                    {
                        Console.WriteLine("--------+-------+--------");
                    }
                    for (int j = 0; j < size; j++)
                    {
                        if (j % 3 == 0)
                        {
                            if (board[i, j] == -1)
                            {
                                Console.Write("| X ");
                            }
                            else Console.Write("| " + board[i, j] + " ");
                        }
                        else
                        {
                            if (board[i, j] == -1)
                            {
                                Console.Write("X ");
                            }
                            else
                            {
                                Console.Write(board[i, j] + " ");
                            }
                        }

                    }
                    Console.WriteLine("|");

                }
                Console.WriteLine("--------+-------+--------");
            }
            Console.WriteLine("Enjoy the sudoku! ^_^    \n\nZizhun Guo\nzg2808@cs.rit.edu");
        }


    }
}
