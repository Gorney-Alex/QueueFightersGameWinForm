using System;
using System.Drawing;
using System.Windows.Forms;
using QueueFightGame;

namespace QueueFightersGameWinForm
{
    public class GameModeForm : Form
    {
        public GameModeForm()
        {
            SetupForm();
        }

        private void SetupForm()
        {
            // Настройка формы
            this.Text = "Выбор режима - Queue Fighters Game";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);

            // Создание заголовка
            Label titleLabel = new Label
            {
                Text = "Выберите режим игры",
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(500, 80),
                Location = new Point((this.ClientSize.Width - 500) / 2, 80)
            };

            // Создание кнопки "Купить самому"
            Button buyManualButton = new Button
            {
                Text = "Купить самому",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(300, 60),
                Location = new Point((this.ClientSize.Width - 300) / 2, 200),
                BackColor = Color.FromArgb(86, 156, 214),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buyManualButton.FlatAppearance.BorderSize = 0;
            buyManualButton.Click += BuyManualButton_Click;

            // Создание кнопки "Рандом"
            Button randomButton = new Button
            {
                Text = "Рандом",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(300, 60),
                Location = new Point((this.ClientSize.Width - 300) / 2, 300),
                BackColor = Color.FromArgb(156, 214, 86),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            randomButton.FlatAppearance.BorderSize = 0;
            randomButton.Click += RandomButton_Click;

            // Создание кнопки "Выход"
            Button exitButton = new Button
            {
                Text = "Выход",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(300, 60),
                Location = new Point((this.ClientSize.Width - 300) / 2, 400),
                BackColor = Color.FromArgb(214, 86, 86),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += ExitButton_Click;

            // Добавление элементов на форму
            this.Controls.Add(titleLabel);
            this.Controls.Add(buyManualButton);
            this.Controls.Add(randomButton);
            this.Controls.Add(exitButton);
        }

        private void BuyManualButton_Click(object sender, EventArgs e)
        {
            // Переход к форме покупки бойцов (для красной команды)
            TeamBuyForm teamBuyForm = new TeamBuyForm("Red");
            this.Hide();
            teamBuyForm.ShowDialog();
            this.Close();
        }

        private void RandomButton_Click(object sender, EventArgs e)
        {
            GameManager gameManager = new GameManager();
            Team redTeam = new Team("Red", 100);
            Team blueTeam = new Team("Blue", 100);

            BattleForm battleForm = new BattleForm(gameManager);
            this.Hide();
            battleForm.ShowDialog();
            this.Close();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            // Возврат в главное меню
            MainMenuForm mainMenuForm = new MainMenuForm();
            this.Hide();
            mainMenuForm.ShowDialog();
            this.Close();
        }
    }
}