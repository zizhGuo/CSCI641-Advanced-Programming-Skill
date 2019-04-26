using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sudoku.View;

namespace sudoku.Model
{
    /// <summary>
    /// Subject clas.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public abstract class ISubject<S>
    {
        private S self;
        private HashSet<sudoku.View.IObserver<S>> observers = new HashSet<sudoku.View.IObserver<S>>();

        public void SetSelf(S Sobject)
        {
            self = Sobject;
        }
        /// <summary>
        /// Registration for observer object.
        /// </summary>
        /// <param name="o"></param>
        public void Register(sudoku.View.IObserver<S> o)
        {
            observers.Add(o);
        }
        /// <summary>
        /// Deregistration for obeserver.
        /// </summary>
        /// <param name="o"></param>
        public void Deregister(sudoku.View.IObserver<S> o)
        {
            observers.Remove(o);
        }
        /// <summary>
        /// Updation for the change.
        /// </summary>
        protected void ReportChange()
        {
            foreach (var o in observers)
            {
                o.Notify(self);
            }
        }
    }
    /// <summary>
    /// Core business logic for Model. 
    /// Can be take out for further MVVM development.
    /// </summary>
    class Sudoku: ISubject<Sudoku>
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

            rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

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

                // Convert the solution into board based on the mode.
                switch (mode)
                {
                    case "easy":
                        GenerateBoard(r: 0.4);
                        break;
                    case "medium":
                        GenerateBoard(r: 0.55);
                        break;
                    case "hard":
                        GenerateBoard(r: 0.75);
                        break;

                }

                // Construction for subject itself.
                SetSelf(this);
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
        /// Update to the observer. 
        /// </summary>
        public void Update() {
            ReportChange();
        }
    }
}
