using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QueueFightGame;

namespace QueueFightersGameWinForm
{
    public class TeamBuyForm : Form
    {
        private string currentTeam;
        private Team redTeam;
        private Team blueTeam;
        private Label moneyLabel;
        private const float INITIAL_MONEY = 70f;

        // Словарь для хранения количества каждого типа бойца
        private Dictionary<string, int> fighterCounts = new Dictionary<string, int>
        {
            { "WeakFighter", 0 },
            { "StrongFighter", 0 },
            { "Archer", 0 },
            { "Healer", 0 },
            { "Mage",0}
        };

        private Image GetFighterImage(string type)
        {
            switch (type)
            {
                case "WeakFighter":
                    return Properties.Resources.light_weight;
                case "StrongFighter":
                    return Properties.Resources.hard_fihter;
                case "Archer":
                    return Properties.Resources.archier;
                case "Healer":
                    return Properties.Resources.heal;
                case "Mage":
                    return Properties.Resources.mag;
                default:
                    return null;
            }
        }

        // Словарь для хранения стоимости каждого типа бойца
        private Dictionary<string, float> fighterCosts = new Dictionary<string, float>
        {
            { "WeakFighter", 15 },
            { "StrongFighter", 30 },
            { "Archer", 25 },
            { "Healer", 20 },
            { "Mage",35}
        };

        public TeamBuyForm(string team)
        {
            currentTeam = team;
            redTeam = new Team("Red", INITIAL_MONEY);
            blueTeam = new Team("Blue", INITIAL_MONEY);
            SetupForm();
        }

        private void SetupForm()
        {
            // Настройка формы
            this.Text = $"Покупка бойцов - Команда {(currentTeam == "Red" ? "Красных" : "Синих")}";
            this.Size = new Size(1600, 1400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);

            // Создание заголовка команды
            Label teamLabel = new Label
            {
                Text = $"Команда {(currentTeam == "Red" ? "Красных" : "Синих")}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = currentTeam == "Red" ? Color.IndianRed : Color.LightBlue,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(400, 50),
                Location = new Point(20, 20)
            };

            // Создание индикатора денег
            moneyLabel = new Label
            {
                Text = $"Деньги: {(currentTeam == "Red" ? redTeam.Money : blueTeam.Money)}",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                TextAlign = ContentAlignment.MiddleRight,
                Size = new Size(200, 50),
                Location = new Point(this.ClientSize.Width - 220, 20)
            };

            // Кнопка "Далее"
            Button nextButton = new Button
            {
                Text = "Далее",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Size = new Size(200, 60),
                Location = new Point(this.ClientSize.Width - 220, this.ClientSize.Height - 80),
                BackColor = Color.FromArgb(86, 156, 214),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            nextButton.FlatAppearance.BorderSize = 0;
            nextButton.Click += NextButton_Click;

            // Добавление элементов на форму
            this.Controls.Add(teamLabel);
            this.Controls.Add(moneyLabel);
            this.Controls.Add(nextButton);

            // Создание карточек бойцов
            CreateFighterCards();
        }

        private void CreateFighterCards()
        {
            // Структура данных для бойцов (имя, описание, изображение)
            var fighterData = new[]
            {
                new {
                    Type = "WeakFighter",
                    Name = "Слабый боец",
                    Description = "Стоит мало, мало защиты, малый урон, мало здоровья",
                    Cost = 15f
                },
                new {
                    Type = "StrongFighter",
                    Name = "Сильный боец",
                    Description = "Стоит дорого, много защиты, большой урон, много здоровья",
                    Cost = 30f
                },
                new {
                    Type = "Archer",
                    Name = "Лучник",
                    Description = "Может бить на расстоянии, стоит средне, малый урон, мало защиты",
                    Cost = 25f
                },
                new {
                    Type = "Healer",
                    Name = "Целитель",
                    Description = "Может лечить союзников рядом с собой, стоит средне, мало защиты",
                    Cost = 20f
                },
                new {
                Type = "Mage",
                    Name = "Маг",
                    Description = "Просто гигачад с крутой атакой, которую мы опишем через 5000 лет",
                    Cost = 35f

                }
            };

            // Размеры и отступы для карточек
            int cardWidth = 200;
            int cardHeight = 350;
            int horizontalSpacing = 30;
            int startX = 50;
            int startY = 100;

            for (int i = 0; i < fighterData.Length; i++)
            {
                var fighter = fighterData[i];
                int x = startX + i * (cardWidth + horizontalSpacing);

                // Создание панели карточки
                Panel cardPanel = new Panel
                {
                    Width = cardWidth,
                    Height = cardHeight,
                    Location = new Point(x, startY),
                    BackColor = Color.FromArgb(60, 60, 65),
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Заголовок карточки
                Label nameLabel = new Label
                {
                    Text = fighter.Name,
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(cardWidth - 20, 30),
                    Location = new Point(10, 10)
                };

                // Стоимость бойца
                Label costLabel = new Label
                {
                    Text = $"Стоимость: {fighter.Cost}",
                    Font = new Font("Arial", 12),
                    ForeColor = Color.Gold,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(cardWidth - 20, 30),
                    Location = new Point(10, 40)
                };

                // Изображение бойца (заглушка - цветной прямоугольник)
                PictureBox imagePictureBox = new PictureBox
                {
                    Size = new Size(120, 120),
                    Location = new Point((cardWidth - 120) / 2, 80),
                    Image = GetFighterImage(fighter.Type),
                    SizeMode = PictureBoxSizeMode.Zoom // Используем Zoom для сохранения пропорций
                };

                // Описание бойца
                Label descriptionLabel = new Label
                {
                    Text = fighter.Description,
                    Font = new Font("Arial", 10),
                    ForeColor = Color.LightGray,
                    TextAlign = ContentAlignment.TopLeft,
                    Size = new Size(cardWidth - 20, 60),
                    Location = new Point(10, 210),
                    AutoSize = false
                };

                // Счетчик количества
                Label countLabel = new Label
                {
                    Text = "0",
                    Name = $"{fighter.Type}Count",
                    Font = new Font("Arial", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(50, 30),
                    Location = new Point((cardWidth - 50) / 2, 280)
                };

                // Кнопка добавления
                Button addButton = new Button
                {
                    Text = "+",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    Size = new Size(40, 40),
                    Location = new Point(cardWidth - 50, 275),
                    BackColor = Color.FromArgb(86, 156, 86),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Tag = fighter.Type
                };
                addButton.FlatAppearance.BorderSize = 0;
                addButton.Click += AddFighter_Click;

                // Кнопка удаления
                Button removeButton = new Button
                {
                    Text = "-",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    Size = new Size(40, 40),
                    Location = new Point(10, 275),
                    BackColor = Color.FromArgb(156, 86, 86),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Tag = fighter.Type
                };
                removeButton.FlatAppearance.BorderSize = 0;
                removeButton.Click += RemoveFighter_Click;

                // Добавление элементов на карточку
                cardPanel.Controls.Add(nameLabel);
                cardPanel.Controls.Add(costLabel);
                cardPanel.Controls.Add(imagePictureBox);
                cardPanel.Controls.Add(descriptionLabel);
                cardPanel.Controls.Add(countLabel);
                cardPanel.Controls.Add(addButton);
                cardPanel.Controls.Add(removeButton);

                // Добавление карточки на форму
                this.Controls.Add(cardPanel);
            }
        }


        private void AddFighter_Click(object sender, EventArgs e)
        {
            string fighterType = (sender as Button).Tag.ToString();
            float cost = fighterCosts[fighterType];
            Team currentTeamObj = currentTeam == "Red" ? redTeam : blueTeam;
            float currentMoney = teamMoneyTracker.ContainsKey(currentTeamObj) ? teamMoneyTracker[currentTeamObj] : currentTeamObj.Money;

            if (currentMoney >= cost)
            {
                // Обновляем счетчик бойцов
                fighterCounts[fighterType]++;

                // Обновляем деньги команды
                // Мы создадим временное решение, т.к. Money только для чтения
                if (currentTeam == "Red")
                    UpdateTeamMoney(redTeam, -cost);
                else
                    UpdateTeamMoney(blueTeam, -cost);

                // Обновляем интерфейс
                UpdateCounterLabel(fighterType);
                UpdateMoneyLabel();
            }
            else
            {
                MessageBox.Show("Недостаточно денег для покупки этого бойца!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RemoveFighter_Click(object sender, EventArgs e)
        {
            string fighterType = (sender as Button).Tag.ToString();

            if (fighterCounts[fighterType] > 0)
            {
                // Обновляем счетчик бойцов
                fighterCounts[fighterType]--;

                // Возвращаем деньги команде
                float cost = fighterCosts[fighterType];
                if (currentTeam == "Red")
                    UpdateTeamMoney(redTeam, cost);
                else
                    UpdateTeamMoney(blueTeam, cost);

                // Обновляем интерфейс
                UpdateCounterLabel(fighterType);
                UpdateMoneyLabel();
            }
        }

        private void UpdateCounterLabel(string fighterType)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel)
                {
                    foreach (Control panelControl in panel.Controls)
                    {
                        if (panelControl is Label label && label.Name == $"{fighterType}Count")
                        {
                            label.Text = fighterCounts[fighterType].ToString();
                            break;
                        }
                    }
                }
            }
        }

        // Создаем словарь для хранения текущего количества денег
        private Dictionary<Team, float> teamMoneyTracker = new Dictionary<Team, float>();

        // Метод для обновления денег команды (работаем через наш трекер)
        private void UpdateTeamMoney(Team team, float amount)
        {
            if (!teamMoneyTracker.ContainsKey(team))
            {
                teamMoneyTracker[team] = team.Money;
            }

            teamMoneyTracker[team] += amount;
        }

        private void UpdateMoneyLabel()
        {
            if (currentTeam == "Red")
            {
                if (!teamMoneyTracker.ContainsKey(redTeam))
                {
                    teamMoneyTracker[redTeam] = redTeam.Money;
                }
                moneyLabel.Text = $"Деньги: {teamMoneyTracker[redTeam]}";
            }
            else
            {
                if (!teamMoneyTracker.ContainsKey(blueTeam))
                {
                    teamMoneyTracker[blueTeam] = blueTeam.Money;
                }
                moneyLabel.Text = $"Деньги: {teamMoneyTracker[blueTeam]}";
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            // Добавляем выбранных бойцов в команду
            AddFightersToTeam();

            if (currentTeam == "Red")
            {
                // Переход к форме закупки для синей команды
                TeamBuyForm blueBuyForm = new TeamBuyForm("Blue");
                blueBuyForm.SetRedTeam(redTeam); // Передаем красную команду
                this.Hide();
                blueBuyForm.ShowDialog();
                this.Close();
            }
            else
            {
                // Обе команды укомплектованы, можно начинать бой
                GameManager gameManager = new GameManager(redTeam, blueTeam);
                BattleForm battleForm = new BattleForm(gameManager);
                this.Hide();
                battleForm.ShowDialog();
                this.Close();
            }
        }

        // Метод для добавления бойцов в команду
        private void AddFightersToTeam()
        {
            Team team = currentTeam == "Red" ? redTeam : blueTeam;

            // Добавляем бойцов в порядке очереди
            for (int i = 0; i < fighterCounts["WeakFighter"]; i++)
            {
                team.AddFighter(new WeakFighter());
            }

            for (int i = 0; i < fighterCounts["StrongFighter"]; i++)
            {
                team.AddFighter(new StrongFighter());
            }

            for (int i = 0; i < fighterCounts["Archer"]; i++)
            {
                team.AddFighter(new Archer(currentTeam + "_Archer"));
            }

            for (int i = 0; i < fighterCounts["Healer"]; i++)
            {
                team.AddFighter(new Healer(currentTeam + "_Healer"));
            }

            for (int i = 0; i < fighterCounts["Mage"]; i++)
            {
                team.AddFighter(new Healer(currentTeam + "_Mage"));
            }
        }

        // Метод для получения красной команды от предыдущей формы
        public void SetRedTeam(Team team)
        {
            redTeam = team;
        }
    }
}


