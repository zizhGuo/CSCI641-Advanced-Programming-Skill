using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sudoku_lab3.ViewModel;

namespace Sudoku_lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewModel.ViewModelController viewModel { get; set; }

        //private string difficulty = "medium";
        //public string Difficulty { get { return difficulty; }set { difficulty = value; } }
        private int size = 9;
        public int Size { get { return size; }set { size = value; } }


        public MainWindow()
        {
            viewModel = ViewModelController.GetInstance();
            InitializeComponent();
            InitPuzzle();
        }

        /// <summary>
        /// Initializing the puzzle.
        /// </summary>
        private void InitPuzzle()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    string name = "b" + i.ToString() + j.ToString();
                    TextBox tb = this.FindName(name) as TextBox;
                    tb.IsEnabled = true;
                }
            }
            viewModel.CreateNewSudoku(viewModel.Difficulty);

            int[][] temp = viewModel.Solution;
            viewModel.Solution = temp;
            int[][] temp2 = viewModel.Sudoku;
            viewModel.Sudoku = temp2;
            int[][] temp3 = viewModel.PuzzleOutput;
            viewModel.PuzzleOutput = temp3;

            //TextBox tb = e.Source as TextBox;
            //TextBox ob = this.FindName("b00") as TextBox;
            // ob.IsEnabled = false;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    string name = "b" + i.ToString() + j.ToString();
                    TextBox tb = this.FindName(name) as TextBox;
                    if (viewModel.Sudoku[i][j] != -1)
                    {
                        tb.IsEnabled = false;
                    }
                    else
                    {
                        tb.Text = "";
                    }
                }
            }
        }
        
        /// <summary>
        /// 
        /// This function works for create a New Sudoku puzzle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    string name = "b" + i.ToString() + j.ToString();
                    TextBox tb = this.FindName(name) as TextBox;
                    tb.IsEnabled = true;
                }
            }

            viewModel.CreateNewSudoku(viewModel.Difficulty);

            // Activate the data binding event. 
            int[][] temp = viewModel.Solution;
            viewModel.Solution = temp;
            int[][] temp2 = viewModel.Sudoku;
            viewModel.Sudoku = temp2;
            int[][] temp3 = viewModel.PuzzleOutput;
            viewModel.PuzzleOutput = temp3;


            //TextBox tb = e.Source as TextBox;
            //TextBox ob = this.FindName("b00") as TextBox;
            // ob.IsEnabled = false;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    string name = "b" + i.ToString() + j.ToString();
                    TextBox tb = this.FindName(name) as TextBox;
                    if (viewModel.Sudoku[i][j] != -1)
                    {                   
                        tb.IsEnabled = false;
                    }
                    else
                    {
                        tb.Text = "";
                    }
                }
            }
        }

        /// <summary>
        /// This function works for revealing the answer for the accesable entry. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            int row = Convert.ToInt32(tb.Name[1].ToString());
            int col = Convert.ToInt32(tb.Name[2].ToString());
            int answer = viewModel.Solution[row][col];
            tb.Text = answer.ToString();

            //int[][] temp2 = viewModel.Sudoku;
            //viewModel.Sudoku = temp2;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            //var temp = viewModel.Sudoku;
            string[] lines = new string[Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (viewModel.PuzzleOutput[i][j] == -1)
                    {
                        lines[i] += "X ";
                    }
                    else
                    {
                        lines[i] += viewModel.PuzzleOutput[i][j] + " ";
                    }
                }
            }
            string docPath = Environment.CurrentDirectory;

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "puzzle.txt")))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
                //outputFile.WriteLine(viewModel.Difficulty);
            }
        }

        /// <summary>
        /// This function works for limitation of user's input number. (range 1 to 9)
        /// Regular expression are used here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^1-9]").IsMatch(e.Text);
            //TextBox textBox = sender as TextBox;
            TextBox textBox = e.Source as TextBox;
            if (e.Handled == false) textBox.Text = ""
;        }

        /// <summary>
        /// This function works for validate the current puzzle by comparing the current one with the unique solution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Vaidate_Click(object sender, RoutedEventArgs e)
        {
            bool isSolved = true;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (viewModel.Sudoku[i][j].ToString() != viewModel.Solution[i][j].ToString()) isSolved = false;
                }
            }
            if (isSolved)
            {
                MessageBoxResult result = MessageBox.Show(this, "Congratualation! Puzzle Solved!");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(this, "Good try! Gotta keep working on it.");
            }

        }
    }
}
