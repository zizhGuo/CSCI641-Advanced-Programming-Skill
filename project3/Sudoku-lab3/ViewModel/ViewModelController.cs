using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku_lab2.Model;
using System.ComponentModel;


namespace Sudoku_lab3.ViewModel
{
    /// <summary>
    /// This is the ViewModel part of this APP. 
    /// </summary>
    public class ViewModelController : INotifyPropertyChanged
    {
        private static ViewModelController instance = new ViewModelController();

        // The actual model that is being using in the whole runing time.
        private Sudoku model;
        int testNumber;

        // default difficulty.
        private string difficulty = "easy";
        public string Difficulty { get { return difficulty; }set { difficulty = value; } }
        private string[] difficultys = {"easy", "medium"};
        public string[] Difficultys { get { return difficultys; } private set { difficultys = value; } }

        public ViewModelController()
        {
            model = new Sudoku(Int32.Parse("9"), "easy");
            testNumber = 0;


        }

        public void CreateNewSudoku(string diff)
        {
            model = new Sudoku(Int32.Parse("9"), diff);
        }

        public static ViewModelController GetInstance()
        {
            return instance;
        }
        public int[][] Sudoku
        {
            get { return model.sudoku_unique_board; }
            set { model.sudoku_unique_board = value; OnPropertyChanged(nameof(Sudoku)); }
        }
        public int[][] Solution
        {
            get { return model.sudoku_unique_solution; }
            set { model.sudoku_unique_solution = value; OnPropertyChanged(nameof(Solution)); }
        }
        public int[][] PuzzleOutput
        {
            get { return model.puzzle_output; }
            set { model.puzzle_output = value; OnPropertyChanged(nameof(PuzzleOutput)); }
        }

        public int TestNumber
        {
            get { return testNumber; }
            set { testNumber = value; OnPropertyChanged(nameof(TestNumber)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
