using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{
    public partial class MainForm : Form
    {
        GameForm gameForm;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            string input = inputTextBox.Text;
            input = input.Trim();
            int n;
            // check if input is valid
            if (int.TryParse(input, out n))
            {
                //if not valid show message
                if (!(n >= 2 && n <= 8)) 
                {
                    MessageBox.Show("Input must be an integer in range from 2 to 8.");
                    return;
                }
                // if game is open then dispose it and open a new one
                if (gameForm != null)
                {
                    gameForm.Dispose();
                    gameForm = null;
                }
                // open the window for gaming
                this.gameForm = new GameForm();
                // pass size of the map
                gameForm.Init(n);
                gameForm.Show();
                // minimize current window
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                MessageBox.Show("Input must be an integer in range from 2 to 8.");
            }
        }
    }
}
