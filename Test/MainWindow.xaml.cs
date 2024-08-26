using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Test
{
    public partial class MainWindow : Window
    {
        private enum GameMode
        {
            PlayerVsComputer,
            PlayerVsPlayer,
            ComputerVsComputer
        }

        private GameMode currentGameMode;
        private string player1Move;
        private string player2Move;

        public MainWindow()
        {
            InitializeComponent();
            currentGameMode = GameMode.PlayerVsComputer; 
        }

        private void OnPvCClick(object sender, RoutedEventArgs e)
        {
            currentGameMode = GameMode.PlayerVsComputer;
            ResetUI();
        }

        private void OnPvPClick(object sender, RoutedEventArgs e)
        {
            currentGameMode = GameMode.PlayerVsPlayer;
            ResetUI();
        }

        private void OnCvCClick(object sender, RoutedEventArgs e)
        {
            currentGameMode = GameMode.ComputerVsComputer;
            StartComputerVsComputerGame();
        }

        private void OnRockClick(object sender, RoutedEventArgs e)
        {
            HandlePlayerMove("Rock");
        }

        private void OnPaperClick(object sender, RoutedEventArgs e)
        {
            HandlePlayerMove("Paper");
        }

        private void OnScissorsClick(object sender, RoutedEventArgs e)
        {
            HandlePlayerMove("Scissors");
        }

        private void HandlePlayerMove(string move)
        {
            switch (currentGameMode)
            {
                case GameMode.PlayerVsComputer:
                    SendMoveToServer(move);
                    break;

                case GameMode.PlayerVsPlayer:
                    if (string.IsNullOrEmpty(player1Move))
                    {
                        player1Move = move;
                        MessageBox.Show("Player 1 has chosen. Now it's Player 2's turn.");
                    }
                    else
                    {
                        player2Move = move;
                        DetermineWinner();
                    }
                    break;

                case GameMode.ComputerVsComputer:
                    break;
            }
        }

        private void SendMoveToServer(string move)
        {
            try
            {
                using (var client = new TcpClient("localhost", 5000))
                {
                    var stream = client.GetStream();
                    byte[] data = Encoding.ASCII.GetBytes(move);
                    stream.Write(data, 0, data.Length);

                    byte[] buffer = new byte[256];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    var parts = response.Split(':');
                    string computerMove = parts[0];
                    string result = parts[1];

                    ComputerMoveText.Text = $"Computer chose: {computerMove}";
                    ResultText.Text = $"Result: {result}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void DetermineWinner()
        {
            string result;

            if (player1Move == player2Move)
            {
                result = "Draw";
            }
            else if ((player1Move == "Rock" && player2Move == "Scissors") ||
                     (player1Move == "Paper" && player2Move == "Rock") ||
                     (player1Move == "Scissors" && player2Move == "Paper"))
            {
                result = "Player 1 Wins!";
            }
            else
            {
                result = "Player 2 Wins!";
            }

            ResultText.Text = $"Result: {result}";
            ComputerMoveText.Text = $"Player 1: {player1Move}, Player 2: {player2Move}";

            player1Move = null;
            player2Move = null;
        }

        private void StartComputerVsComputerGame()
        {
            var computer1Move = GetRandomMove();
            var computer2Move = GetRandomMove();

            string result;

            if (computer1Move == computer2Move)
            {
                result = "Draw";
            }
            else if ((computer1Move == "Rock" && computer2Move == "Scissors") ||
                     (computer1Move == "Paper" && computer2Move == "Rock") ||
                     (computer1Move == "Scissors" && computer2Move == "Paper"))
            {
                result = "Computer 1 Wins!";
            }
            else
            {
                result = "Computer 2 Wins!";
            }

            ComputerMoveText.Text = $"Computer 1 chose: {computer1Move}, Computer 2 chose: {computer2Move}";
            ResultText.Text = $"Result: {result}";
        }

        private string GetRandomMove()
        {
            var moves = new[] { "Rock", "Paper", "Scissors" };
            var rand = new Random();
            return moves[rand.Next(moves.Length)];
        }

        private void ResetUI()
        {
            ComputerMoveText.Text = "Computer chose: ";
            ResultText.Text = "Result: ";
        }
    }
}