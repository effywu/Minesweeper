using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        void removeBlank(int x, int y)
        {

            if (!squares[x, y].getBtn.Visible)
            {
                return;
            }
            else
            {
                //abc++;
                squares[x, y].getBtn.Visible = false;
                remainsquare--;
                for (int xx = -1; xx < 2; xx++)
                {
                    for (int yy = -1; yy < 2; yy++)
                    {
                        if (x + xx >= 0 && y + yy >= 0 && x + xx < width && y + yy < height)
                        {
                            if (squares[x, y].getLbl.Text == " ")
                            {
                                removeBlank(x + xx, y + yy);
                            }

                        }
                    }
                }
            }


        }

        void bttnOnclick(object sender, System.EventArgs e)
        {
            if (!tmr_ElapsedTime.Enabled)
            {
                return;
            }

            Button bttnClick = sender as Button;

            if (bttnClick == null)
            {
                return; //not a button.
            }
            string[] split = bttnClick.Parent.Name.Split(new Char[] { ' ' });
            //string[] split = bttnClick.Name.Split(new Char[] { ' ' });

            int x = System.Convert.ToInt32(split[0]);
            int y = System.Convert.ToInt32(split[1]);



            if (squares[x, y].getLbl.Text == "*")
            {
                //Game Over!
                tmr_ElapsedTime.Enabled = false;

                for (int xx = 0; xx < width; xx++)
                {
                    for (int yy = 0; yy < height; yy++)
                    {
                        if (squares[xx, yy].getLbl.Text == "*")
                        {
                            squares[xx, yy].getBtn.Visible = false;
                        }

                    }
                }


            }


            removeBlank(x, y);
            if (remainmine == remainsquare)
            {
                Won won = new Won();
                won.Show();
            };

        }

        void bttnOnRightClick(object sender, EventArgs e)
        {


        }

        private Square createButton(int x, int y, int gridX, int gridY)
        {
            Square s = new Square();
            s.Name = gridX.ToString() + " " + gridY.ToString();
            s.Location = new System.Drawing.Point(x, y);
            s.BringToFront();
            Controls.AddRange(new System.Windows.Forms.Control[] { s, });
            s.getBtn.Click += new System.EventHandler(bttnOnclick);
            return s;
        }

        private int[,] grid;
        private Button[,] btn_grid;
        private Label[,] lbl_grid;
        private Square[,] squares;
        private int timer = 0;
        private int remainmine;
        private int remainsquare;


        int mintCount, width, height, startX = 15, startY = 68;
        //suspend / resume drawing
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETSalmonRAW = 11;

        public static void SuspendDrawing(System.Windows.Forms.Control control)
        {
            SendMessage(control.Handle, WM_SETSalmonRAW, false, 0);
        }
        public static void ResumeDrawing(System.Windows.Forms.Control control)
        {
            SendMessage(control.Handle, WM_SETSalmonRAW, true, 0);
            control.Refresh();
        }
        private bool createGrid()
        {
            this.Width = startX * 2 + (width + 1) * 24 - 5;
            this.Height = startY * 2 + (height) * 24;
            squares = new Square[width, height];
            Random rnd1 = new Random();
            //Add buttons/Labels.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    squares[x, y] = createButton(startX + 24 * (x + 0), startY + 24 * (y + 0), x, y);
                }
            }

            int currMineCount = mintCount;
            //Add Mines

            while (currMineCount > 0)
            {
                int mineX = rnd1.Next(width);
                int mineY = rnd1.Next(height);

                if (squares[mineX, mineY].getLbl.Text != "*")
                {
                    squares[mineX, mineY].setLbl = "*";
                    squares[mineX, mineY].getLbl.Font = new Font("Microsoft Sans Serif", 30.75f, squares[mineX, mineY].getLbl.Font.Style, squares[mineX, mineY].getLbl.Font.Unit);
                    currMineCount--;
                }
            }

            //Calculate Numbers.

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (squares[x, y].getLbl.Text != "*")
                    {
                        int numMines = 0;
                        for (int xx = -1; xx < 2; xx++)
                        {
                            for (int yy = -1; yy < 2; yy++)
                            {
                                if (x + xx >= 0 && y + yy >= 0 && x + xx < width && y + yy < height)
                                {
                                    if (squares[x + xx, y + yy].getLbl.Text == "*")
                                    {
                                        numMines++;
                                    }
                                }


                            }
                        }

                        if (numMines == 0)
                        {
                            squares[x, y].getLbl.Text = " ";
                        }
                        else
                        {
                            squares[x, y].getLbl.Text = numMines.ToString();
                        }


                    }

                }
            }


            return true;
        }


        private void clearPreviousGame()
        {
            if (squares != null)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        //grid[x, y] = 0;



                        if (Controls.Contains(squares[x, y]))
                        {
                            Controls.Remove(squares[x, y]);
                        }
                    }
                }
            }

        }

        public void startLevel(int difficulty)
        {
            clearPreviousGame();
            lbl_ElapsedTime.Text = "0";
            timer = 0;
            tmr_ElapsedTime.Start();
            bool error = false;

            switch (difficulty)
            {
                case 1:
                    mintCount = 10;
                    width = 9;
                    height = 9;

                    break;
                case 2:
                    mintCount = 40;
                    width = 16;
                    height = 16;
                    break;
                case 3:
                    mintCount = 99;
                    width = 30;
                    height = 16;
                    break;
                default:
                    error = true;
                    break;
            }
            remainmine = mintCount;
            remainsquare = width * height;

            lbl_mines.Text = mintCount.ToString();

            if (!error)
            {
                createGrid();
            }
        }

        private void tmr_ElapsedTime_Tick(object sender, EventArgs e)
        {
            timer++;
            lbl_ElapsedTime.Text = timer.ToString();
        }

        private void byeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startLevel(1);
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startLevel(2);
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startLevel(3);
        }
    }
}
