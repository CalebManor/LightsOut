using System;
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

namespace LightsOut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LightsOutGame game;
        public MainWindow()
        {
            InitializeComponent();

            game = new LightsOutGame();

            CreateGrid();
            DrawGrid();
        }

        private void CreateGrid()
        {
            int rectSize = (int)boardCanvas.Width / game.GridSize;
            // Create rectangles for grid
            for(int r = 0; r < game.GridSize; r++)
            {
                for(int c = 0; c < game.GridSize; c++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = Brushes.White;
                    rect.Width = rectSize + 1;
                    rect.Height = rect.Width + 1;
                    rect.Stroke = Brushes.Black;
                    
                    // Store each row and col as a Point
                    rect.Tag = new Point(r, c);

                    // Register event handler
                    rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
                    
                    // Put the rectangle at the proper location within the canvas
                    Canvas.SetTop(rect, r * rectSize);
                    Canvas.SetLeft(rect, c * rectSize);
                    
                    // Add the new rectangle to thecanvas' children
                    boardCanvas.Children.Add(rect);
                }
            }
        }

        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get row and column from Rectangle's Tag
            Rectangle rect = sender as Rectangle;
            var rowCol = (Point)rect.Tag;
            int row = (int)rowCol.X;
            int col = (int)rowCol.Y;
            
            game.Move(row, col);

            // TODO: Redraw the grid and see if the game is over
            DrawGrid();
            if (game.IsGameOver())
            {
                MessageBox.Show("You've won!");
            }

            // Event was handled
            e.Handled = true;
        }

        private void DrawGrid()
        {
            int index = 0;

            // Set the colors of the rectangles
            for(int r = 0; r < game.GridSize; r++)
            {
                for(int c =0; c < game.GridSize; c++)
                {
                    Rectangle rect = boardCanvas.Children[index] as Rectangle;
                    index++;
                    
                    if(game.GetGridValue(r, c))
                    {
                        // On
                        rect.Fill = Brushes.White;
                        rect.Stroke = Brushes.Black;
                    }
                    else
                    {
                        // Off
                        rect.Fill = Brushes.Black;
                        rect.Stroke = Brushes.White;
                    } 
                }
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            game.NewGame();
            DrawGrid();
        }

        private void MenuHelp_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.ShowDialog();
        }
    }

    public class LightsOutGame
    {
        private int gridSize= 3;
        private bool[,] grid;           // Store the on/off state of the grid
        private Random rand;

        public const int MaxGridSize = 7;
        public const int MinGridSize = 3;

        // Returns the grid size
        public int GridSize
        {
            get
            {
                return gridSize;
            }
            set
            {
                if(value>= MinGridSize && value<= MaxGridSize)
                {
                    gridSize = value;
                    grid = new bool[gridSize, gridSize];
                    NewGame();
                }
            }
        }
        public LightsOutGame() 
        { 
            rand = new Random(); 
            GridSize = MinGridSize; 
        }
        
        // Returns the grid value at the given row and column
        public bool GetGridValue(int row, int col)
        {
            return grid[row, col];
        }

        // Randomizes the grid
        public void NewGame()
        {
            for(int r = 0; r < gridSize; r++)
            {
                for(int c = 0; c < gridSize; c++)
                {
                    grid[r, c] = rand.Next(2) == 1;
                }
            }
        }
        
        // Inverts the selected box and all surrounding boxes
        public void Move(int row, int col)
        {
            if(row < 0 || row >= gridSize || col < 0 || col >= gridSize)
            {
                throw new ArgumentException("Row or column is outside the legal range of 0 to "
                    + (gridSize -1));
            }
            
            // Invert selected box and all surrounding boxes
            for(int i = row -1; i <= row + 1; i++)
            {
                for(int j = col -1; j <= col + 1; j++)
                {
                    if(i >= 0 && i < gridSize && j >= 0 && j < gridSize)
                    {
                        grid[i, j] = !grid[i, j];
                    }
                }
            }
        }
        
        // Returns true if all cells are off
        public bool IsGameOver()
        {
            for(int r = 0; r < gridSize; r++)
            {
                for(int c = 0; c < gridSize; c++)
                {
                    if(grid[r, c])
                    {
                        return false;
                    }
                }
            }

            // All values must be false (off)
            return true;
        }
    }
}
