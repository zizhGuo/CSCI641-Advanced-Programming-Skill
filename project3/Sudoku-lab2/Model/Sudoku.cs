using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



/// <summary>
/// CSCI-641 Advanced Programming Skills Lab3
/// Author: Zizhun Guo.
/// zg2808@cs.rit.edu
/// 
/// The Sudoku puzzle that has unique solution is generated based on two steps:
///     1. Generate the a complete Sudoku (no empty entry):
///         i. Use backtracking algorithm implemented in lab2.
///     2. Dibble on the Sudoku, generate the puzzle:
///         i. Dibble one hole, justify the current puzzle if it has unique solution.
///         ii. If it has multiple solutions, cancel current hole, dibble in another entry.
///         iii. If it has unique solution, continues. If there's no unique solution, programs terminates.
///         
/// Features:
///     1. If the entry contains one one candidate number, this number is the value of the entry.
///     2. If one of candidate entry is unique out of the candidates from row/column/block, this candidates number is the value of the entry.
/// 
/// Solution:
///     1. Based on Sudoku rule, check the entry value if it is satisfied.
///     2. When updated, check the entry if it is satisfied Feature 1.
///     3. When Updated, check the entry if it is satisfied Feature 2.
///     4. Repeat 1~3 till get the answers.
///
/// </summary>
namespace Sudoku_lab2.Model
{
    /// <summary>
    /// Core business logic for Model. 
    /// Can be take out for further MVVM development.
    /// </summary>
    public class Sudoku
    {
        // The size of the board. (4 or 9)
        int size;

        // The mode of the game. (1. "easy"; 2. "medium"; 3. "hard")
        // failed for inputing the mode would casue exception.
        string mode;

        // Sudoku board. (ready for player to play).
        int[,] sudoku;

        // Sudoku solution. (used for generating the board)
        int[,] solution;

        // Util: list for generating the random number, used for backtracking.
        List<int>[,] cells;


        // Util: random seed for shuffle the cell list. 
        Random rand;

        int flag = 0;

        int[,] testSudoku;

        // The sudoku pullze that has unique solution
        int[,] sudoku_unique;

        // Jagged array for Sudoku solution
        public int[][] sudoku_unique_solution;

        // Jagged array for Sudoku puzzle board
        public int[][] sudoku_unique_board;

        // Jagged array for Save into the file
        public int[][] puzzle_output;

        /// <summary>
        /// Property for board.
        /// </summary>
        internal int[,] Sudokuboard
        {
            get { return sudoku; }
            set { value = sudoku; }
        }
        /// <summary>
        /// Property for solution.
        /// </summary>
        internal int[,] SudokuSolution
        {
            get { return solution; }
            set { value = solution; }
        }
        /// <summary>
        /// Property for size.
        /// </summary>
        internal int Size
        {
            get { return size; }
            set { value = size; }
        }
        /// <summary>
        /// Property for mode.
        /// </summary>
        internal string Mode
        {
            get { return mode; }
            set { value = mode; }
        }

        /// <summary>
        /// Constructor for initializing the Sudoku solution and board.
        /// </summary>
        /// <param name="size"> The size of sudoku.</param>
        /// <param name="mode"> The mode of sudoku.</param>
        public Sudoku(int size, string mode)
        {
            this.size = size;
            this.mode = mode;

            sudoku = new int[size, size];
            solution = new int[size, size];
            cells = new List<int>[size, size];
            sudoku_unique = new int[size, size];
            sudoku_unique_solution = new int[size][];
            sudoku_unique_board = new int[size][];
            puzzle_output = new int[size][];
            for (int i = 0; i < size; i++)
            {
                sudoku_unique_solution[i] = new int[size];
                sudoku_unique_board[i] = new int[size];
                puzzle_output[i] = new int[size];
            }

            rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            // ----------------------------- Sudoku Board only for testing -------------------------------

            testSudoku = new int[,] { {-1, -1, -1, -1, 2, -1, 8, 4, -1 },
                                      {-1, -1, -1, -1, -1, -1, -1, -1, -1},
                                      {5, -1, 4, -1, 3, 7, -1, -1,-1},
                                      {-1, 4, -1, -1, -1, -1, -1, -1, -1},
                                      {-1, -1, 1, -1, -1, 3, 7, -1, 5},
                                      {-1, -1, -1, 2, -1, -1, -1, -1, 9},
                                      {1, -1, -1, 4, 8, -1, -1, 6, -1},
                                      {-1, -1, -1, 6, -1, -1, -1, 5, -1},
                                      {9, -1, 7, -1, -1, 2, -1, -1, -1}};
            // -----------------------------------------------------------------------------------
            try
            {
                // used -1 to represent the null position.
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        sudoku[i, j] = -1;
                    }
                }

                // Generate the solution.
                GenerateSolution();

                // Store the solution.
                SaveSolution();

                // Detect the validation of input mode.
                ModeDetection();
                DateTime start = DateTime.Now;
                // Convert the solution into board based on the mode.
                switch (mode)
                {
                    case "easy":
                        //GenerateBoard(r: 0.4);
                        sudoku_unique = DibbleGeneration(0.35);
                        break;
                    case "medium":
                        //GenerateBoard(r: 0.55);
                        sudoku_unique = DibbleGeneration(0.55);
                        break;
                    case "hard":
                        //GenerateBoard(r: 0.75);
                        sudoku_unique = DibbleGeneration(0.75);
                        break;

                }

                // Convert the 2d array into jagged array
                TwoDtoJagged();


            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Null Reference: " + e);
                Console.WriteLine("StackTrace " + e.StackTrace);
            }

        }
        /// <summary>
        /// Validation for the mode.
        /// </summary>
        void ModeDetection()
        {
            if (!(mode == "easy" || mode == "medium" || mode == "hard"))
            {
                throw new Exception("Mode setting does not satified. \n Should be 1. easy; 2. medium; 3. hard");
            }
        }

        /// <summary>
        /// Store the solution.
        /// </summary>
        void SaveSolution()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    solution[i, j] = sudoku[i, j];
                }
            }
        }
        /// <summary>
        /// Convert the solution into board.
        /// </summary>
        /// <param name="r"> game mode rate.</param>
        void GenerateBoard(double r = 0.4)
        {
            double rate = r;
            Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            int counter = 0;
            do
            {
                int x = rand.Next(0, size);
                int y = rand.Next(0, size);

                if (sudoku[x, y] != -1)
                {
                    sudoku[x, y] = -1;
                    counter++;
                }
            } while (counter < size * size * rate);
        }
        // Inialize the candidates.
        List<int>[,] InitCandidates(int[,] solution)
        {
            var candidates = new List<int>[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (solution[i, j] == -1)
                    {
                        candidates[i, j] = new List<int>();
                        for (int k = 1; k <= size; k++) candidates[i, j].Add(k);
                    }
                    else
                    {
                        candidates[i, j] = new List<int>();
                        candidates[i, j].Add(solution[i, j]);
                    }

                }
            }
            return candidates;
        }

        // Update the candidate board by row, column and 3x3 block.
        void UpdateCandidates(List<int>[,] candidates)
        {
            // For each line
            for (int i = 0; i < size; i++)
            {
                // for each entry in that specific line
                for (int j = 0; j < size; j++)
                {
                    //the entry that has already assigned a number
                    if (candidates[i, j].Count == 1)
                    {
                        var temp = candidates[i, j][0];
                        for (int k = 0; k < size; k++)
                        {
                            // for each entry that has this candidate and number of candidates greater than 1
                            if (candidates[i, k].Contains(temp) && candidates[i, k].Count > 1)
                            {
                                candidates[i, k].Remove(temp);
                            }
                        }
                    }
                }
            }

            //For each colomn
            for (int i = 0; i < size; i++)
            {
                // for each entry in that specific colomn
                for (int j = 0; j < size; j++)
                {
                    //the entry that has already assigned a number
                    if (candidates[i, j].Count == 1)
                    {
                        var temp = candidates[i, j][0];
                        for (int k = 0; k < size; k++)
                        {
                            // for each entry that has this candidate and number of candidates greater than 1
                            if (candidates[k, j].Contains(temp) && candidates[k, j].Count > 1)
                            {
                                candidates[k, j].Remove(temp);
                            }
                        }
                    }
                }
            }

            // For each block
            for (int i = 0; i < Math.Sqrt(size); i++)
            {
                for (int j = 0; j < Math.Sqrt(size); j++)
                {
                    // block changed
                    for (int k = 0; k < Math.Sqrt(size); k++)
                    {
                        for (int l = 0; l < Math.Sqrt(size); l++)
                        {
                            int x0 = i * (int)Math.Sqrt(size) + k;
                            int y0 = j * (int)Math.Sqrt(size) + l;

                            if (candidates[x0, y0].Count == 1)
                            {
                                var temp = candidates[x0, y0][0];

                                for (int a = 0; a < Math.Sqrt(size); a++)
                                {
                                    for (int b = 0; b < Math.Sqrt(size); b++)
                                    {
                                        int x1 = i * (int)Math.Sqrt(size) + a;
                                        int y1 = j * (int)Math.Sqrt(size) + b;
                                        if (candidates[x1, y1].Contains(temp) && candidates[x1, y1].Count > 1)
                                        {
                                            candidates[x1, y1].Remove(temp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //PrintCandidates();

        }

        /// <summary>
        ///  Check whether candidates subsets contains unique numbers.
        /// </summary>

        void CheckCandidateUnique(List<int>[,] candidates)
        {
            //Line checking
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (candidates[i, j].Count > 1)
                    {
                        var temp = candidates[i, j];

                        foreach (var cand in temp)
                        {
                            bool flag = true;

                            for (int k = 0; k < size; k++)
                            {
                                if (k != j && candidates[i, k].Contains(cand))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                candidates[i, j] = new List<int>();
                                candidates[i, j].Add(cand);
                                break;
                            }
                        }
                    }
                }
            }

            //Column checking
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (candidates[i, j].Count > 1)
                    {
                        var temp = candidates[i, j];

                        foreach (var cand in temp)
                        {
                            bool flag = true;

                            for (int k = 0; k < size; k++)
                            {
                                if (k != i && candidates[k, j].Contains(cand))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                candidates[i, j] = new List<int>();
                                candidates[i, j].Add(cand);
                                break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < Math.Sqrt(size); i++)
            {
                for (int j = 0; j < Math.Sqrt(size); j++)
                {
                    // block changed
                    for (int k = 0; k < Math.Sqrt(size); k++)
                    {
                        for (int l = 0; l < Math.Sqrt(size); l++)
                        {
                            int x0 = i * (int)Math.Sqrt(size) + k;
                            int y0 = j * (int)Math.Sqrt(size) + l;

                            if (candidates[x0, y0].Count > 1)
                            {
                                var temp = candidates[x0, y0];

                                foreach (var cand in temp)
                                {
                                    //Console.WriteLine("cand = " + cand);

                                    bool flag = true;

                                    for (int a = 0; a < Math.Sqrt(size); a++)
                                    {
                                        for (int b = 0; b < Math.Sqrt(size); b++)
                                        {
                                            int x1 = i * (int)Math.Sqrt(size) + a;
                                            int y1 = j * (int)Math.Sqrt(size) + b;

                                            foreach (var cand1 in candidates[x1, y1])
                                            {
                                                //Console.WriteLine("cand = " + cand1);
                                            }

                                            if ((x1 != x0 || y1 != y0) && candidates[x1, y1].Contains(cand))
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                        if (!flag) break;
                                    }

                                    if (flag)
                                    {
                                        candidates[x0, y0] = new List<int>();
                                        candidates[x0, y0].Add(cand);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            //PrintCandidates();
        }


        /// <summary>
        /// This function works for solving the Sudoku Puzzle, and check if it contains only one solution.
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns></returns>
        int SolveSudoku(List<int>[,] candidates)
        {
            int ret = 0;

            while (true)
            {
                // Get a candidates copy
                var candidates_old = new List<int>[size, size];

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        candidates_old[i, j] = new List<int>();
                        foreach (var ele in candidates[i, j])
                        {
                            candidates_old[i, j].Add(ele);
                        }
                    }
                }

                // Update the current puzzle.
                UpdateCandidates(candidates);

                // Checking the current puzzle if it has unique solution.
                CheckCandidateUnique(candidates);
                //PrintCandidates(candidates);

                // Sudoku sovled.
                bool solved = true;
                foreach (var l in candidates)
                {
                    if (l.Count > 1)
                    {
                        solved = false;
                    }
                }
                if (solved)
                {
                    flag++;
                    ret = flag;
                    //SudokuPrinter.PrintCandidate(candidates, size);
                    break;
                }

                // if not solved, but no change made comparing with previous candidates.
                bool unChanged = true;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        foreach (var ele in candidates[i, j])
                        {
                            if (!candidates[i, j].SequenceEqual(candidates_old[i, j]))
                            {
                                unChanged = false;
                                break;
                            }
                        }
                    }
                }
                if (unChanged)
                {
                    ret = -1;
                    break;
                }
            }

            // If still existing entry that has multiple candidates, assign all possible candidates to the entry,
            // and recursive call this function to compute the number of possible solutions. 
            if (ret == -1)
            {
                int x0 = 0;
                int y0 = 0;
                bool blank = false;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (candidates[i, j].Count > 1)
                        {
                            x0 = i;
                            y0 = j;
                            blank = true;
                        }
                    }
                }
                if (blank)
                {
                    foreach (var ele in candidates[x0, y0])
                    {
                        var temp = new List<int>[size, size];
                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                temp[i, j] = new List<int>();
                                foreach (var _ele in candidates[i, j])
                                {
                                    temp[i, j].Add(_ele);
                                }
                            }
                        }
                        //Console.WriteLine("flag = " + flag);
                        temp[x0, y0] = new List<int>();
                        temp[x0, y0].Add(ele);

                        //return value indicates the number of solutions.
                        ret = SolveSudoku(temp);

                        // multiple solution
                        if (flag > 2) break;

                    }
                }

            }
            // If there is only one solution, return 0.
            if (flag == 1)
            {
                //SudokuPrinter.PrintCandidate(candidates, size);
                ret = 0;
            }
            return ret;
        }

        /// <summary>
        /// This function works for creating a new Sudoku puzzle board.
        /// 1. Based
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        int[,] DibbleGeneration(double r = 0.4)
        {
            var sudoku_dibble = InitCandidates(solution);

            //initialize a sudoku board for storing the final sudoku game.
            int[,] sudoku_board = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    sudoku_board[i, j] = solution[i, j];
                }
            }

            // Generate a list of random number ranged from 0 to 15 or 0 to 80
            // whereas start_index defines the order of start point of dibbling, while dibble_index
            // defines the order for dibbling the rest list.
            var dibble_index = new List<int>();
            var rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            for (int i = 0; i < size * size; i++)
            {
                dibble_index.Add(i);
            }
            dibble_index.Shuffle(rand);
            int index = dibble_index.Count - 1;

            // Start dibbling!
            int testNum = 0;
            while (testNum < size * size * r)
            {
                int top = dibble_index[index];
                int x0 = top / size;
                int y0 = top % size;
                //Console.WriteLine("top = " + top);
                //Console.WriteLine("count = " + dibble_index.Count);
                //Console.WriteLine("x0 = " + x0 + "y0 = " + y0);

                if (sudoku_dibble[x0, y0].Count == 1)
                {

                    // Create a temp sudoku_dibble for unique solvement check.
                    List<int>[,] sudoku_dibble_temp = new List<int>[size, size];
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            sudoku_dibble_temp[i, j] = new List<int>();
                            foreach (var ele in sudoku_dibble[i, j])
                            {
                                sudoku_dibble_temp[i, j].Add(ele);
                            }
                        }
                    }
                    //SudokuPrinter.PrintCandidate(sudoku_dibble_temp, size);

                    //temp dibble value
                    int temp = sudoku_dibble[x0, y0][0];

                    //dibble one hole on the temp, assign it with a linked list ranged from 0 - 8
                    sudoku_dibble_temp[x0, y0] = new List<int>();
                    for (int k = 1; k <= size; k++) sudoku_dibble_temp[x0, y0].Add(k);

                    // Check if it has unique answer.
                    int ret = SolveSudoku(sudoku_dibble_temp);

                    // If it can be uniquely solved.
                    if (ret == 0)
                    {
                        testNum++;
                        sudoku_board[x0, y0] = -1;

                        sudoku_dibble[x0, y0] = new List<int>();
                        for (int k = 1; k <= size; k++) sudoku_dibble[x0, y0].Add(k);

                        dibble_index.Pop();
                        flag = 0;
                    }
                    else
                    {
                        dibble_index.Shuffle(rand);
                        flag = 0;
                        if (dibble_index.Count == 0) break;
                    }
                }
                index = dibble_index.Count - 1;
                //Console.WriteLine("index number = " + dibble_index.Count);
            }
            return sudoku_board;
        }

        /// <summary>
        /// 
        /// Printing the candidates puzzle for testing.
        /// </summary>
        /// <param name="candidates"></param>
        void PrintCandidates(List<int>[,] candidates)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (candidates[i, j].Count == 1)
                    {
                        Console.WriteLine("row = " + (i + 1) + " colomn = " + (j + 1) + " is " + candidates[i, j][0]);
                    }
                    else
                    {
                        Console.Write("row = " + (i + 1) + " colomn = " + (j + 1) + " is ");
                        for (int k = 0; k < candidates[i, j].Count; k++) Console.Write(candidates[i, j][k] + " ");
                        Console.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// The core business logic for generating the one solution of sudoku 
        /// based on the given size and mode.
        /// This function employs the recursion call.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public object GenerateSolution(int x = 0, int y = 0)
        {
            // Assignment for the coordinates.
            if (y > (size - 1))
            {
                x++;
                y = 0;
            }
            if (y < 0)
            {
                x--;
                y = (size - 1);
            }

            // If the x reaches the size + 1, the generation is done. Exit.
            if (x == size)
            {
                return null;
            }
            else
            {
                // Generating a new cell list with shuffled random number ranged from 1 to size;
                // If marches to a new null cell, initialize a new random list to it.
                if (cells[x, y] == null)
                {
                    cells[x, y] = new List<int>(size);
                    for (int i = 0; i < size; i++) cells[x, y].Add(i + 1);
                    cells[x, y].Shuffle(rand);

                }

                // If the current cell list becomes empty, backtracking.
                if (cells[x, y].Count == 0)
                {
                    sudoku[x, y] = -1;
                    cells[x, y] = null;
                    return GenerateSolution(x, y - 1);
                }

                // Assign random number from cells to board.
                sudoku[x, y] = cells[x, y].Pop();

                //Check if it is valid:
                //:valid move to next cell.
                //:invalid re-assignment
                if (Check(x, y))
                {
                    //Console.WriteLine("  passed");
                    return GenerateSolution(x, y + 1);
                }
                else
                {
                    //Console.WriteLine("  failed");
                    return GenerateSolution(x, y);
                }

            }

        }
        /// <summary>
        /// This function works for checking the validation for each generated random number.
        /// It check in three phase:
        /// phase 1: row
        /// phase 2: column
        /// phase 3: the local block(box).
        /// This checking method can also be used for player's answer check in the future.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool Check(int x, int y)
        {

            int temp = sudoku[x, y];

            // Check same row.
            for (int j = 0; j < size; j++)
            {
                if (j != y && sudoku[x, j] == temp)
                {
                    //Console.Write("--row--");
                    return false;
                }
            }

            // Check same column.
            for (int i = 0; i < size; i++)
            {
                if (i != x && sudoku[i, y] == temp)
                {
                    return false;
                }
            }


            //Check same block.
            int r = (int)Math.Sqrt(size);
            int x0 = (x / r) * r, y0 = (y / r) * r;
            for (int i = x0; i < x0 + r; i++)
            {
                for (int j = y0; j < y0 + r; j++)
                {
                    if (i != x && j != y && sudoku[i, j] == temp)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Conversion.
        /// </summary>
        void TwoDtoJagged()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    sudoku_unique_solution[i][j] = solution[i, j];
                    sudoku_unique_board[i][j] = sudoku_unique[i, j];
                    puzzle_output[i][j] = sudoku_unique[i, j];
                }
            }
        }
    }
}
