using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{
    public partial class GameForm : Form
    {
        // given by user 
        // n on range from 2 to 8 is ok
        int n = 2;
        int mapSize;
        const int cellSize = 50;

        // = 1 or 2
        int currentPlayer;

        // button of the checker that's moving on next click
        Button prevButton;

        bool isMoving;

        // 0: no checker on the cell
        // 1: there's a checker belonging to 1st player
        // 2: there's a checker belonging to 2nd player
        int[,] map;
        // if queenMap[j, i] == true then there's a queen
        bool[,] queenMap;

        Button[,] buttons;

        Image whiteFigure;
        Image blackFigure;
        public GameForm()
        {
            InitializeComponent();

            // icons for checkers
            whiteFigure = new Bitmap(new Bitmap(@"C:\Users\TUF Gaming\OneDrive - НИТУ МИСиС\Документы\Учеба\3 семестр\Технологии программирования\Курсовая (2)\Варвара Ко\6\Checkers\w.png"), new Size(cellSize - 10, cellSize - 10));
            blackFigure = new Bitmap(new Bitmap(@"C:\Users\TUF Gaming\OneDrive - НИТУ МИСиС\Документы\Учеба\3 семестр\Технологии программирования\Курсовая (2)\Варвара Ко\6\Checkers\b.png"), new Size(cellSize - 10, cellSize - 10));
        }

        private void Checkers_Load(object sender, EventArgs e)
        {

        }

        // sets primary data
        // creates map of checkers and empty cells
        // creates an empty map of queen indicators
        public void Init(int n)
        {
            // set primary data
            this.n = n;
            if (n < 2)
            {
                n = 4;
            }
            currentPlayer = 1;
            isMoving = false;
            prevButton = null;

            mapSize = n * 2;
            buttons = new Button[mapSize, mapSize];

            // create a map and fulfill the playable cells
            map = new int[mapSize, mapSize];
            for (int i = 0; i < mapSize; i++)
            {
                if (i < n - 1)
                    currentPlayer = 1;
                else if (i > mapSize - n)
                    currentPlayer = 2;
                // if not playable set to 0
                else
                    currentPlayer = 0;
                for (int j = 0; j < mapSize; j++)
                {
                    map[i, j] = (i + j) % 2 * currentPlayer;
                }
            }
            currentPlayer = 1;

            // there are no queens so fill with false value
            queenMap = new bool[mapSize, mapSize];
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    queenMap[i, j] = false;
                }
            }

            // visualize the map
            CreateMap();
        }
        public void CreateMap()
        {
            this.Width = (mapSize + 1) * cellSize;
            this.Height = (mapSize + 1) * cellSize;

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    // make the button operatable in OnFigurePress when clicked
                    button.Click += new EventHandler(OnFigurePress);
                    button.Size = new Size(cellSize, cellSize);
                    // set icons of checkers
                    if (map[i, j] == 1)
                        button.Image = whiteFigure;
                    else if (map[i, j] == 2) button.Image = blackFigure;

                    // set background color for every cell
                    button.BackColor = GetPrevButtonColor(button);

                    buttons[i, j] = button;

                    // make the button clickable by user
                    this.Controls.Add(button);
                }
            }
        }
        public void SwitchPlayer()
        {
            currentPlayer = (currentPlayer == 1) ? 2 : 1;
        }
        // fills playable cells with gray color and non-playable with white
        public Color GetPrevButtonColor(Button prevButton)
        {
            // if it was playable return gray
            if ((prevButton.Location.Y / cellSize % 2) != 0)
            {
                if ((prevButton.Location.X / cellSize % 2) == 0)
                {
                    return Color.Gray;
                }
            }
            else if ((prevButton.Location.Y / cellSize) % 2 == 0)
            {
                if ((prevButton.Location.X / cellSize) % 2 != 0)
                {
                    return Color.Gray;
                }
            }
            // if the previous button was not playable return white
            return Color.White;
        }
        private void OnFigurePress(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int x = button.Location.X / cellSize;
            int y = button.Location.Y / cellSize;

            // trying to move blank cell or checker
            // that doesn't belong to current player
            if (!isMoving && (map[y, x] == 0 || map[y, x] != currentPlayer))
            {
                return;
            }

            if (isMoving)
            {
                if (MoveAble(prevButton, button))
                {
                    Move(prevButton, button);
                    SwitchPlayer();
                }
                isMoving = false;
                prevButton = null;
            }
            else
            {
                if (map[y, x] == 0)
                {
                    return;
                }
                isMoving = true;
                prevButton = button;
            }
            RefreshMap();
            if (currentPlayer == 1)
            {
                button.BackColor = Color.DarkGoldenrod;
            }
            else
            {
                button.BackColor = Color.DarkCyan;
            }
            if (!AnyMove())
            {
                Congratulate();
            }
        }
        // check whether or not it possible to move a checker
        // from prevButton's position to button's one
        private bool MoveAble(Button prevButton, Button button)
        {
            // somewhy for buttons x and y are switched
            int fromX = prevButton.Location.X / cellSize;
            int fromY = prevButton.Location.Y / cellSize;
            int toX = button.Location.X / cellSize;
            int toY = button.Location.Y / cellSize;

            // trying to move to non-playable cell
            if((toX + toY) % 2 == 0)
            {
                return false;
            }

            // if trying to move checker that doesn't belongs to the current  player
            if (map[fromY, fromX] != 0 && map[fromY, fromX] != currentPlayer)
            {
                return false;
            }

            // check if the move is diagonal
            if (Math.Abs(fromX - toX) != Math.Abs(fromY - toY))
            {
                return false;
            }

            // move for a regular checker
            if (!queenMap[fromY, fromX])
            {
                // move without eating
                if(Math.Abs(fromX - toX) == 1 && Math.Abs(fromY - toY) == 1)
                {
                    // player 1 moves down else don't let it
                    if ((currentPlayer == 1 && toY - fromY < 0))
                    {
                        return false;
                    }
                    // player 2 moves up 
                    if ((currentPlayer == 2 && toY - fromY > 0))
                    {
                        return false;
                    }
                    // check if to cell is blank
                    if (map[toY, toX] == 0) 
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                // move with eating
                else if (Math.Abs(fromX - toX) == 2 && Math.Abs(fromY - toY) == 2)
                {
                    int dirEatX = (toX - fromX) / 2;
                    int dirEatY = (toY - fromY) / 2;

                    // check if checker that is about to be eaten is of opposite color
                    if (map[fromY + dirEatY, fromX + dirEatX] != 0 && map[fromY + dirEatY, fromX + dirEatX] != map[fromY, fromX]) 
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            // move of a queen
            else
            {
                // find buttons in route
                int dirX = (toX - fromX);
                int dirY = (toY - fromY);

                int countCheckersBetween = 0;
                // number of opposite checkers in route
                int countOpCheckersBetween = 0;

                // location of the last button found on the way
                int LastBetweenButtonX = -1, LastBetweenButtonY = -1;

                // find checkers that are trying to be 'jumped' over
                // dirX > 0 - move to the right
                // dirX < 0 - move to the left
                // dirY > 0 - move to the down
                // dirY < 0 - move to the up
                if (dirX > 0 && dirY > 0)
                {
                    int i = fromX + 1;
                    int j = fromY + 1;
                    while (i < toX && j < toY)
                    {
                        if (map[j, i] != 0)
                        {
                            countCheckersBetween += 1;
                            if (map[j, i] != map[fromY, fromX])
                            {
                                countOpCheckersBetween += 1;
                                LastBetweenButtonX = i;
                                LastBetweenButtonY = j;
                            }
                        }
                        i++;
                        j++;
                    }
                }
                else if(dirX > 0 && dirY < 0)
                {
                    int i = fromX + 1;
                    int j = fromY - 1;
                    while (i < toX && j > toY)
                    {
                        if (map[j, i] != 0)
                        {
                            countCheckersBetween += 1;
                            if (map[j, i] != map[fromY, fromX])
                            {
                                countOpCheckersBetween += 1;
                                LastBetweenButtonX = i;
                                LastBetweenButtonY = j;
                            }
                        }
                        i++;
                        j--;
                    }
                }
                else if (dirX < 0 && dirY < 0)
                {
                    int i = fromX - 1;
                    int j = fromY - 1;
                    while (i > toX && j > toY)
                    {
                        if (map[j, i] != 0)
                        {
                            countCheckersBetween += 1;
                            if (map[j, i] != map[fromY, fromX])
                            {
                                countOpCheckersBetween += 1;
                                LastBetweenButtonX = i;
                                LastBetweenButtonY = j;
                            }
                        }
                        i--;
                        j--;
                    }
                }
                else if (dirX < 0 && dirY > 0)
                {
                    int i = fromX - 1;
                    int j = fromY + 1;
                    while (i > toX && j < toY)
                    {
                        if (map[j, i] != 0)
                        {
                            countCheckersBetween += 1;
                            if (map[j, i] != map[fromY, fromX])
                            {
                                countOpCheckersBetween += 1;
                                LastBetweenButtonX = i;
                                LastBetweenButtonY = j;
                            }
                        }
                        i--;
                        j++;
                    }
                }

                // move without eating
                if (countCheckersBetween == 0)
                {
                    return true;
                }
                // move eating 
                // number of checkers to be eaten must be = 1
                else if(countCheckersBetween == 1 && countOpCheckersBetween == 1)
                {
                    // after eating, queen must stay in neighbor to eaten checker cell 
                    if (Math.Abs(toX - LastBetweenButtonX) == 1 && Math.Abs(toY - LastBetweenButtonY) == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else 
                { 
                    return false; 
                }
                
            }
        }
        // move a checker
        private void Move(Button prevButton, Button button)
        {
            {
                // somewhy x and y is switched for buttons
                int fromX = prevButton.Location.X / cellSize;
                int fromY = prevButton.Location.Y / cellSize;
                int toX = button.Location.X / cellSize;
                int toY = button.Location.Y / cellSize;
                // move for a regular checker
                if (!queenMap[fromY, fromX])
                {
                    // move without eating
                    if (Math.Abs(fromX - toX) == 1 && Math.Abs(fromY - toY) == 1)
                    {
                        // switching player indicator
                        map[toY, toX] = map[fromY, fromX];
                        map[fromY, fromX] = 0;
                    }
                    // move with eating
                    else if (Math.Abs(fromX - toX) == 2 && Math.Abs(fromY - toY) == 2)
                    {
                        int dirEatX = (toX - fromX) / 2;
                        int dirEatY = (toY - fromY) / 2;

                        // set eaten checker's cell as blank and move checker
                        map[fromY + dirEatY, fromX + dirEatX] = 0; 
                        map[toY, toX] = map[fromY, fromX];
                        map[fromY, fromX] = 0;

                        // if the eaten checker was a queen then update the array
                        if (queenMap[fromY + dirEatY, fromX + dirEatX])
                        {
                            queenMap[fromY + dirEatY, fromX + dirEatX] = false; 
                        }
                    }
                    // check if the checker became a queen
                    if (currentPlayer == 1 && toY == mapSize - 1)
                    {
                        queenMap[toY, toX] = true;
                    }
                    else if (currentPlayer == 2 && toY == 0)
                    {
                        queenMap[toY, toX] = true;
                    }
                }
                // move of a queen
                else
                {
                    // find buttons in route
                    int dirX = (toX - fromX);
                    int dirY = (toY - fromY);

                    int countCheckersBetween = 0;

                    // number of opposite checkers in route
                    int countOpCheckersBetween = 0;

                    // location of the last button found on the way
                    int LastBetweenButtonX = -1, LastBetweenButtonY = -1;

                    // find checkers that are trying to be 'jumped' over
                    // dirX > 0 - move to the right
                    // dirX < 0 - move to the left
                    // dirY > 0 - move to the down
                    // dirY < 0 - move to the up
                    if (dirX > 0 && dirY > 0)
                    {
                        // start from the first next cell
                        int i = fromX + 1;
                        int j = fromY + 1;
                        while (i < toX && j < toY)
                        {
                            if (map[j, i] != 0)
                            {
                                countCheckersBetween += 1;
                                if (map[j, i] != map[fromY, fromX])
                                {
                                    countOpCheckersBetween += 1;
                                    LastBetweenButtonX = i;
                                    LastBetweenButtonY = j;
                                }
                            }
                            i++;
                            j++;
                        }
                    }
                    else if (dirX > 0 && dirY < 0)
                    {
                        int i = fromX + 1;
                        int j = fromY - 1;
                        while (i < toX && j > toY)
                        {
                            if (map[j, i] != 0)
                            {
                                countCheckersBetween += 1;
                                if (map[j, i] != map[fromY, fromX])
                                {
                                    countOpCheckersBetween += 1;
                                    LastBetweenButtonX = i;
                                    LastBetweenButtonY = j;
                                }
                            }
                            i++;
                            j--;
                        }
                    }
                    else if (dirX < 0 && dirY < 0)
                    {
                        int i = fromX - 1;
                        int j = fromY - 1;
                        while (i > toX && j > toY)
                        {
                            if (map[j, i] != 0)
                            {
                                countCheckersBetween += 1;
                                if (map[j, i] != map[fromY, fromX])
                                {
                                    countOpCheckersBetween += 1;
                                    LastBetweenButtonX = i;
                                    LastBetweenButtonY = j;
                                }
                            }
                            i--;
                            j--;
                        }
                    }
                    else if (dirX < 0 && dirY > 0)
                    {
                        int i = fromX - 1;
                        int j = fromY + 1;
                        while (i > toX && j < toY)
                        {
                            if (map[j, i] != 0)
                            {
                                countCheckersBetween += 1;
                                if (map[j, i] != map[fromY, fromX])
                                {
                                    countOpCheckersBetween += 1;
                                    LastBetweenButtonX = i;
                                    LastBetweenButtonY = j;
                                }
                            }
                            i--;
                            j++;
                        }
                    }

                    // move without eating
                    if (countCheckersBetween == 0)
                    {
                        // switching player indicator
                        map[toY, toX] = map[fromY, fromX];
                        map[fromY, fromX] = 0;
                    }
                    // move eating 
                    // number of checkers to be eaten must be = 1   
                    else if (countCheckersBetween == 1 && countOpCheckersBetween == 1)
                    {
                        // after eating, queen must stay in neighbor to eaten checker cell 
                        if (Math.Abs(toX - LastBetweenButtonX) == 1 && Math.Abs(toY - LastBetweenButtonY) == 1)
                        {
                            // set eaten checker's cell as blank and move checker
                            map[LastBetweenButtonY, LastBetweenButtonX] = 0;
                            map[toY, toX] = map[fromY, fromX];
                            map[fromY, fromX] = 0;

                            // if the eaten checker was a queen then update the array
                            if (queenMap[LastBetweenButtonY, LastBetweenButtonX])
                            {
                                queenMap[LastBetweenButtonY, LastBetweenButtonX] = false;
                            }
                        }
                    }
                    queenMap[fromY, fromX] = false;
                    queenMap[toY, toX] = true;
                }
            }
        }
        // find if there any possible moves on the map
        private bool AnyMove()
        {
            // set a list of buttons with checkers on them
            List<Button> buttonsList = new List<Button>();
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[j, i] != 0)
                    {
                        buttonsList.Add(this.buttons[j, i]);
                    }
                }
            }

            // find if there any possible moves

            foreach (Button button in buttonsList)
            {
                for (int i = 0; i < mapSize; i++)
                {
                    for (int j = 0; j < mapSize; j++)
                    {
                        if (map[j, i] == 0)
                        {
                            // if a possible move is found return true
                            if (MoveAble(button, buttons[j, i]))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            // if none move is found return false
            return false;
        }
        // update visualization of the map
        private void RefreshMap()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var button = buttons[i, j];
                    button.BackColor = GetPrevButtonColor(button);
                    button.Location = new Point(j * cellSize, i * cellSize);
                    if (map[i, j] == 1)
                        button.Image = whiteFigure;
                    else if (map[i, j] == 2) button.Image = blackFigure;
                    // if not playable there's no checker's icon
                    else button.Image = null;
                }
            }
        }
        // when player wins the method opens
        // new form and passes id of the winner
        private void Congratulate()
        {
            SwitchPlayer();
            WinnerForm winnerForm = new WinnerForm();
            winnerForm.Congratulate(currentPlayer);
            winnerForm.Show();
            this.WindowState = FormWindowState.Minimized;
        }
    }
    
}
