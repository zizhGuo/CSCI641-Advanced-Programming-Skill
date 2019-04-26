using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using sudoku.Model;
using sudoku.View;

namespace sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            Sudoku sudoku = new Sudoku(Int32.Parse(args[0]), args[1]);
            new View.View(sudoku);
            sudoku.Update();
            Console.ReadLine();

        }

    }
    public static class MyExtensions
    {
        /// <summary>
        /// List<T> extension method for shuffle function.
        /// This extensive function allows to shuffle the order of every elements from the list.
        /// Referenced and modified from https://stackoverflow.com/questions/273313/randomize-a-listt.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="rand"></param>
        public static void Shuffle<T>(this List<T> list, Random rand)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// List<T> extension method for popping the last element.
        /// This extensive function allows to use List as a stack 
        /// that can pop out the last element of the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Pop<T>(this List<T> list)
        {
            int index = list.Count - 1;
            T r = list[index];
            list.RemoveAt(index);
            return r;
        }
    }

    
}
