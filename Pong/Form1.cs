﻿/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush drawBrush = new SolidBrush(Color.White);
        Font drawFont = new Font("Courier New", 50);
        Font drawFontSmall = new Font("Courier New", 20);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //create random number gen
        Random randGen = new Random();
        int ballRandomRight;
        int ballRandomDown;
        int intersectCount;

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int ballSpeed;
        Rectangle ball;

        //paddle speeds and rectangles
        int paddleSpeed;
        Rectangle p1, p2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 3;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                this.BackColor = Color.Black;
                ballRandomRight = randGen.Next(1, 3);
                ballRandomDown = randGen.Next(1, 3);
                intersectCount = 0; //used to count number of paddle intersects
                if (ballRandomRight == 1)
                {
                    ballMoveRight = false;
                }

                else
                {
                    ballMoveRight = true;
                }

                if (ballRandomDown == 1)
                {
                    ballMoveDown = false;
                }
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }
            paddleSpeed = 5;
            ballSpeed = 4;
            intersectCount = 0;
            this.BackColor = Color.ForestGreen;
            //set starting position for paddles on new game and point scored 
            const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            


            p1.Width = p2.Width = 5;    //width for both paddles set the same
            p1.Height = p2.Height = 60;  //height for both paddles set the same


            //p1 starting position
            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            //p2 starting position
            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            // set Width and Height of ball
            ball.Width = ball.Height = 10;
            // set starting X position for ball to middle of screen, (use this.Width and ball.Width)
            ball.X = this.Width / 2 - ball.Width / 2;

            // set starting Y position for ball to middle of screen, (use this.Height and ball.Height)
            ball.Y = this.Height / 2 - ball.Height / 2;


        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {

            #region update ball position

            // create code to move ball either left or right based on ballMoveRight and using ballSpeed
            if (ballMoveRight == true && ball.X < this.Width - ball.Width)
            {
                ball.X += ballSpeed;
            }

            if (ballMoveRight == false && ball.X > 0)
            {
                ball.X -= ballSpeed;
            }
            // create code move ball either down or up based on ballMoveDown and using ballSpeed

            if (ballMoveDown == true && ball.Y < this.Height - ball.Height)
            {
                ball.Y += ballSpeed;
            }

            if (ballMoveDown == false && ball.Y > 0)
            {
                ball.Y -= ballSpeed;
            }
            #endregion

            #region update paddle positions
            // create code to move player 1 paddle up using p1.Y and paddleSpeed
            if (aKeyDown == true && p1.Y > 0)
            {
                p1.Y -= paddleSpeed;
            }

            // create an if statement and code to move player 1 paddle down using p1.Y and paddleSpeed
            if (zKeyDown == true && p1.Y < this.Height - p1.Height)
            {
                p1.Y += paddleSpeed;
            }
            // TODO create an if statement and code to move player 2 paddle up using p2.Y and paddleSpeed
            if (jKeyDown == true && p2.Y > 0)
            {
                p2.Y -= paddleSpeed;
            }
            // TODO create an if statement and code to move player 2 paddle down using p2.Y and paddleSpeed
            if (mKeyDown == true && p2.Y < this.Height - p2.Height)
            {
                p2.Y += paddleSpeed;
            }
            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y <= 0) // if ball hits top line
            {
                // use ballMoveDown boolean to change direction
                ballMoveDown = true;
                // play a collision sound
                collisionSound.Play();
            }
            // In an else if statement use ball.Y, this.Height, and ball.Width to check for collision with bottom line
            else if (ball.Y >= this.Height - ball.Width)
            {
                // If true use ballMoveDown down boolean to change direction
                ballMoveDown = false;

                // play a collision sound
                collisionSound.Play();
            }


            #endregion

            #region ball collision with paddles

            // TODO create if statment that checks p1 collides with ball and if it does
            if (ball.IntersectsWith(p1))
            {
                intersectCount++; // add one to player intersect counter
                // --- play a "paddle hit" sound
                collisionSound.Play();
                //ballSpeed += 1;
                //paddleSpeed += 1;
                // --- use ballMoveRight boolean to change direction
                ballMoveRight = true;
                //this.BackColor = Color.Blue; //Change background colour of form

                //change colour and difficulty based on number of paddle hits
                if (intersectCount < 8) //easy mode, no changes
                {
                    ballSpeed = 4;
                    this.BackColor = Color.ForestGreen;
                }

                if (intersectCount >= 8) //medium mode, make paddles smaller
                {
                    p1.Height = p2.Height = 50;
                    ballSpeed = 4;
                    this.BackColor = Color.DarkOrange;
                }

                if (intersectCount >= 15) //hard mode, make them smaller again
                {
                    p1.Height = p2.Height = 40;
                    ballSpeed = 4;
                    this.BackColor = Color.Red;
                }

                if (intersectCount == 20) //insane mode starts
                {
                    p1.Height = p2.Height = 40;
                    ballSpeed = 6;
                    paddleSpeed = 6;
                    this.BackColor = Color.MediumPurple;
                }

                if (intersectCount > 20) //insane mode continues and speeds up ball speed and paddle speed with each paddle hit
                {
                    p1.Height = p2.Height = 40;
                    ballSpeed++;
                    paddleSpeed++;
                    this.BackColor = Color.MediumPurple;
                }

                if (intersectCount % 5 == 0 && intersectCount < 20) //power hit every five hits to make sure both players get to use it, but not in insane mode since it is already insane.
                {
                    ballSpeed = 2 * ballSpeed;
                }
            }



            // create if statment that checks p2 collides with ball and if it does
            if (ball.IntersectsWith(p2))
            {
                intersectCount++; // add one to player intersect counter
                // --- play a "paddle hit" sound
                collisionSound.Play();
                //ballSpeed += 1;
                //paddleSpeed += 1;
                // --- use ballMoveRight boolean to change direction
                ballMoveRight = false;
                this.BackColor = Color.Red; //change background colour of form

                if (intersectCount < 8) //easy mode, no changes
                {
                    ballSpeed = 4;
                    this.BackColor = Color.ForestGreen;
                }

                if (intersectCount >= 8) //medium mode, make paddles smaller
                {
                    p1.Height = p2.Height = 50;
                    ballSpeed = 4;
                    this.BackColor = Color.DarkOrange;
                }

                if (intersectCount >= 15) //hard mode, make them smaller again
                {
                    p1.Height = p2.Height = 40;
                    ballSpeed = 4;
                    this.BackColor = Color.Red;
                }

                if (intersectCount == 20) //insane mode starts
                {
                    p1.Height = p2.Height = 40;
                    ballSpeed = 6;
                    paddleSpeed = 6;
                    this.BackColor = Color.MediumPurple;
                }

                if (intersectCount > 20) //insane mode continues and speeds up ball speed and paddle speed with each paddle hit
                {
                    p1.Height = p2.Height = 40;
                    ballSpeed ++;
                    paddleSpeed ++;
                    this.BackColor = Color.MediumPurple;
                }

                if (intersectCount % 3 == 0 && intersectCount < 20) //power hit every five hits to make sure both players get to use it, but not in insane mode since it is already insane.
                {
                    ballSpeed = 2 * ballSpeed;
                }
            }


            /*  ENRICHMENT
             *  Instead of using two if statments as noted above see if you can create one
             *  if statement with multiple conditions to play a sound and change direction
             */

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X <= 0)  // ball hits left wall logic
            {

                
                // --- play score sound
                scoreSound.Play();
                // --- update player 2 score
                player2Score += 1;

                // use if statement to check to see if player 2 has won the game. If true run 
                if (player2Score == gameWinScore)
                {
                    GameOver("Player 2");
                }
                // GameOver method. Else change direction of ball and call SetParameters method.
                else
                {

                    SetParameters();
                    ballRandomRight = randGen.Next(1, 3);
                    ballRandomDown = randGen.Next(1, 3);
                    if (ballRandomRight == 1)
                    {
                        ballMoveRight = false;
                    }

                    else
                    {
                        ballMoveRight = true;
                    }

                    if (ballRandomDown == 1)
                    {
                        ballMoveDown = false;
                    }

                    else
                    {
                        ballMoveDown = true;
                    }
                    //ballMoveRight = !ballMoveRight;
                    this.Refresh();
                    Thread.Sleep(1000);
                }


            }

            // same as above but this time check for collision with the right wall
            if (ball.X >= this.Width - ball.Width)
            {
                // --- play score sound
                scoreSound.Play();

                // --- update player 1 score
                player1Score += 1;

                // use if statement to check to see if player 2 has won the game. If true run 
                if (player1Score == gameWinScore)
                {
                    GameOver("Player 1");
                }

                else
                {
                    SetParameters();
                    ballRandomRight = randGen.Next(1, 3);
                    ballRandomDown = randGen.Next(1, 3);
                    if (ballRandomRight == 1)
                    {
                        ballMoveRight = false;
                    }

                    else
                    {
                        ballMoveRight = true;
                    }

                    if (ballRandomDown == 1)
                    {
                        ballMoveDown = false;
                    }

                    else
                    {
                        ballMoveDown = true;
                    }
                    //ballMoveRight = !ballMoveRight;
                    this.Refresh();
                    Thread.Sleep(1000);
                }
            }

            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();

        }

        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;

            startLabel.Visible = true;
            // create game over logic
            // --- stop the gameUpdateLoop
            gameUpdateLoop.Stop();
            // --- show a message on the startLabel to indicate a winner, (need to Refresh).
            startLabel.Text = winner + " is the winner";
            this.Refresh();
            // --- pause for two seconds
            Thread.Sleep(2000);
            // --- use the startLabel to ask the user if they want to play again
            startLabel.Text = "Do you want to play again? Press SPACE to do so";
            this.Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (newGameOk == false)
            {
                // draw paddles using FillRectangle
                e.Graphics.FillRectangle(drawBrush, p1);
                e.Graphics.FillRectangle(drawBrush, p2);
                // draw ball using FillRectangle
                e.Graphics.FillEllipse(drawBrush, ball);

                // TODO draw scores to the screen using DrawString

                e.Graphics.DrawString("" + player1Score, drawFont, drawBrush, 50, 40);
                e.Graphics.DrawString("" + player2Score, drawFont, drawBrush, this.Width - 120, 40);

                //draw rally number
                e.Graphics.DrawString("Rally: " + intersectCount + " Hits", drawFontSmall, drawBrush, 190, 40);

                // if statements to draw the mode levels based on number of hits
                if (intersectCount < 8)
                {
                    e.Graphics.DrawString("Mode: EASY", drawFontSmall, drawBrush, 215, 70);
                }

                if (intersectCount >= 8 && intersectCount < 15)
                {
                    e.Graphics.DrawString("Mode: MEDIUM", drawFontSmall, drawBrush, 215, 70);
                }

                if (intersectCount >= 15 && intersectCount < 20)
                {
                    e.Graphics.DrawString("Mode: HARD", drawFontSmall, drawBrush, 215, 70);
                }

                if (intersectCount >= 20)
                {
                    e.Graphics.DrawString("Mode: INSANE", drawFontSmall, drawBrush, 215, 70);
                }
            }
        }

    }
}