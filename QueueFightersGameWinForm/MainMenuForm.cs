using System;
using System.Drawing;
using System.Windows.Forms;
using QueueFightGame;

namespace QueueFightersGameWinForm
{
    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            SetupForm();
        }

        private void SetupForm()
        {
            // Настройка формы
            this.Text = "Kingdom of the Figthers";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackgroundImage = Properties.Resources.zastavka; // Устанавливаем фон
            this.BackgroundImageLayout = ImageLayout.Stretch;

           

            // Создание кнопки "Новая игра"
            PictureBox newGameButton = new PictureBox
            {
                Size = new Size(200, 100),
                Location = new Point((this.ClientSize.Width - 200) / 2, 250),
                Image = Properties.Resources.knopkaplay, // Используем кнопку "ИГРАТЬ"
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent // Прозрачный фон для кнопки
            };
            newGameButton.Click += NewGameButton_Click;


            // Добавление элементов на форму
            this.Controls.Add(newGameButton);
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            // Переход к форме выбора режима игры
            GameModeForm gameModeForm = new GameModeForm();
            this.Hide();
            gameModeForm.ShowDialog();
            this.Close();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            // Выход из игры
            Application.Exit();
        }
    }
}