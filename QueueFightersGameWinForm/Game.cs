using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueueFightersGameWinForm;

namespace QueueFightGame
{
    internal class Game
    {
        private GameManager Manager;
        private MainMenuForm MainMenuForm;

        public Game()
        {
            Manager = new GameManager();
        }

        public void Initialize()
        {
            // Создание и отображение главного меню
            MainMenuForm = new MainMenuForm();
        }

        public void Play()
        {
            // Запуск формы главного меню
            MainMenuForm.ShowDialog();
        }
    }
}