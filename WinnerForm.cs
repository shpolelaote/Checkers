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
    public partial class WinnerForm : Form
    {
        // congratulate the winner
        // the id is given by GameForm class method
        public void Congratulate(int id)
        {
            if (id == 1)
            {
                winnerLabel.Text = "Player of white won!!";
            }
            else
            {
                winnerLabel.Text = "Player of black won!!";
            }
        }
        public WinnerForm()
        {
            InitializeComponent();
        }

        private void WinnerForm_Load(object sender, EventArgs e)
        {

        }
    }
}
