using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QueueFightGame;

namespace QueueFightersGameWinForm
{
    public class BattleForm : Form
    {
        private GameManager gameManager;
        private Team redTeam;
        private Team blueTeam;
        private Timer battleTimer;
        private RichTextBox battleLog;
        private Panel battlefieldPanel;
        private Button nextRoundButton;
        private Team attackingTeam;
        private Team defendingTeam;
        private bool battleEnded = false;

        // Словарь для хранения элементов управления юнитов
        private Dictionary<IUnit, Panel> unitPanels = new Dictionary<IUnit, Panel>();

        public BattleForm(GameManager manager)
        {
            gameManager = manager;
            redTeam = gameManager.RedTeam;
            blueTeam = gameManager.BlueTeam;
            SetupForm();
            battlefieldPanel.BackgroundImage = Properties.Resources.фон_битвы;
            battlefieldPanel.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private Image GetUnitImage(IUnit unit)
        {
            if (unit is StrongFighter)
                return Properties.Resources.hard_fihter;
            else if (unit is WeakFighter)
                return Properties.Resources.light_weight;
            else if (unit is Archer)
                return Properties.Resources.archier;
            else if (unit is Healer)
                return Properties.Resources.heal;
            else if (unit is Mage)
                return Properties.Resources.mag;
            else
                return null;


        }
        private void SetupForm()
        {
            // Настройка формы
            this.Text = "Битва - Queue Fighters Game";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);

            // Создание панели поля боя
            battlefieldPanel = new Panel
            {
                Location = new Point(50, 50),
                Size = new Size(1100, 400),
                BackColor = Color.FromArgb(30, 30, 35),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Создание лога битвы
            battleLog = new RichTextBox
            {
                Location = new Point(50, 470),
                Size = new Size(700, 250),
                BackColor = Color.FromArgb(30, 30, 35),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Multiline = true
            };

            // Кнопка следующего раунда
            nextRoundButton = new Button
            {
                Text = "Следующий раунд",
                Location = new Point(850, 550),
                Size = new Size(200, 60),
                BackColor = Color.FromArgb(86, 156, 214),
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            nextRoundButton.FlatAppearance.BorderSize = 0;
            nextRoundButton.Click += NextRoundButton_Click;

            // Кнопка выхода в главное меню
            Button mainMenuButton = new Button
            {
                Text = "Главное меню",
                Location = new Point(850, 650),
                Size = new Size(200, 60),
                BackColor = Color.FromArgb(214, 86, 86),
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            mainMenuButton.FlatAppearance.BorderSize = 0;
            mainMenuButton.Click += MainMenuButton_Click;

            // Добавление элементов на форму
            this.Controls.Add(battlefieldPanel);
            this.Controls.Add(battleLog);
            this.Controls.Add(nextRoundButton);
            this.Controls.Add(mainMenuButton);

            // Отображение команд
            DisplayTeams();

            // Инициализация битвы
            InitializeBattle();
        }

        private void DisplayTeams()
        {


            // Заголовок красной команды
            Label redTeamLabel = new Label
            {
                Text = "Красная команда",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.IndianRed,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(200, 30),
                Location = new Point(50, 10)
            };

            // Заголовок синей команды
            Label blueTeamLabel = new Label
            {
                Text = "Синяя команда",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.LightBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(200, 30),
                Location = new Point(950, 10)

            };

            this.Controls.Add(redTeamLabel);
            this.Controls.Add(blueTeamLabel);

            // Отображение бойцов красной команды
            DisplayTeamUnits(redTeam, true);

            // Отображение бойцов синей команды
            DisplayTeamUnits(blueTeam, false);
        }

        private void DisplayTeamUnits(Team team, bool isRedTeam)
        {
            int unitWidth = 120;
            int unitHeight = 180;
            int spacing = 20;
            int yPosition = 150;
            int position = 0;

            // Центрируем бойцов относительно середины панели
            int centerX = battlefieldPanel.Width / 2;
            int teamWidth = team.QueueFighters.Count * (unitWidth + spacing);
            int startX = isRedTeam ? centerX - teamWidth - 20 : centerX + 20; // Красные слева, синие справа от центра

            foreach (IUnit unit in team.QueueFighters)
            {
                int xPosition = startX + (position * (unitWidth + spacing));

                Panel unitPanel = new Panel
                {
                    Size = new Size(unitWidth, unitHeight),
                    Location = new Point(xPosition, yPosition),
                    BackColor = Color.Transparent,
                    Tag = unit
                };

                Label nameLabel = new Label
                {
                    Text = unit.Name,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(unitWidth, 30),
                    Location = new Point(0, 0)
                };

                ProgressBar healthBar = new ProgressBar
                {
                    Name = "healthBar",
                    Value = (int)unit.Health,
                    Maximum = 100,
                    Size = new Size(unitWidth - 10, 20),
                    Location = new Point(5, unitHeight - 30),
                    ForeColor = Color.Green
                };

                PictureBox unitIconPictureBox = new PictureBox
                {
                    Size = new Size(unitWidth, unitHeight - 50),
                    Location = new Point(0, 30),
                    Image = GetUnitImage(unit),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent
                };

                unitPanel.Controls.Add(nameLabel);
                unitPanel.Controls.Add(healthBar);
                unitPanel.Controls.Add(unitIconPictureBox);
                battlefieldPanel.Controls.Add(unitPanel);
                unitPanels[unit] = unitPanel;

                position++;
            }
        }

        private Color GetUnitColor(IUnit unit)
        {
            if (unit.Team.TeamName == "Red")
            {
                if (unit is StrongFighter) return Color.DarkRed;
                if (unit is WeakFighter) return Color.IndianRed;
                if (unit is Archer) return Color.Firebrick;
                if (unit is Healer) return Color.MediumVioletRed;
                if (unit is Mage) return Color.Crimson;
            }
            else
            {
                if (unit is StrongFighter) return Color.DarkBlue;
                if (unit is WeakFighter) return Color.RoyalBlue;
                if (unit is Archer) return Color.SteelBlue;
                if (unit is Healer) return Color.MediumSlateBlue;
                if (unit is Mage) return Color.DeepSkyBlue;
            }
            return Color.Gray;
        }


        private void InitializeBattle()
        {
            // Определение атакующей команды случайным образом
            Random random = new Random();
            attackingTeam = random.Next(2) == 0 ? redTeam : blueTeam;
            defendingTeam = attackingTeam == redTeam ? blueTeam : redTeam;

            // Вывод информации о начале боя
            AddLogMessage("=== НАЧАЛО БИТВЫ ===");
            AddLogMessage($"Команда {(attackingTeam == redTeam ? "Красных" : "Синих")} начинает атаку!");

            // Отображение текущего состояния команд
            UpdateTeamStatusInLog();
        }

        private void NextRoundButton_Click(object sender, EventArgs e)
        {
            if (battleEnded)
            {
                return;
            }

            PerformRound();
        }

        private void PerformRound()
        {
            if (!redTeam.HasFighters() || !blueTeam.HasFighters())
            {
                EndBattle();
                return;
            }

            // Получаем следующего бойца из каждой команды
            IUnit attacker = attackingTeam.QueueFighters.Last();
            IUnit defender = defendingTeam.QueueFighters.First();

            if (attacker == null || defender == null)
            {
                EndBattle();
                return;
            }

            // Добавляем информацию о текущем раунде
            AddLogMessage("\n=== НОВЫЙ РАУНД ===");
            AddLogMessage($"Атакует команда: {(attackingTeam == redTeam ? "Красных" : "Синих")}");
            AddLogMessage($"{attacker.Name} (HP: {attacker.Health}) атакует {defender.Name} (HP: {defender.Health})");

            // Проводим атаку
            float defenderHealthBefore = defender.Health;
            attacker.Attack(defender);
            float damageDone = defenderHealthBefore - defender.Health;

            AddLogMessage($"{attacker.Name} наносит {damageDone} урона!");

            // Анимация атаки
            AnimateAttack(attacker, defender);

            // Обновляем полоску здоровья защищающегося
            UpdateUnitHealthBar(defender);

            // Действия специальных юнитов (лучники и целители)
            PerformSpecialActions(attackingTeam, defender);

            // Проверяем, погиб ли защищающийся
            if (defender.Health <= 0)
            {
                AddLogMessage($"{defender.Name} пал в бою!");

                // Удаляем юнита из команды
                defendingTeam.RemoveFighter();

                // Удаляем панель юнита с поля боя
                if (unitPanels.ContainsKey(defender))
                {
                    battlefieldPanel.Controls.Remove(unitPanels[defender]);
                    unitPanels.Remove(defender);
                }

                // Проверяем, закончилась ли игра
                if (!defendingTeam.HasFighters())
                {
                    EndBattle();
                    return;
                }
            }

            // Меняем местами атакующую и защищающуюся команды
            Team temp = attackingTeam;
            attackingTeam = defendingTeam;
            defendingTeam = temp;

            // Обновляем статус команд в логе
            UpdateTeamStatusInLog();
        }

        private void PerformSpecialActions(Team team, IUnit target)
        {
            // Обработка специальных действий юнитов (Лучники, Целители)
            foreach (IUnit unit in team.QueueFighters.Skip(1))
            {
                if (unit is Archer archer)
                {
                    float targetHealthBefore = target.Health;
                    archer.DoSpecialAttack(target, unit.Team);
                    float damageDone = targetHealthBefore - target.Health;

                    if (damageDone > 0)
                    {
                        AddLogMessage($"{archer.Name} стреляет в {target.Name} и наносит {damageDone} урона!");
                        AnimateArcherAttack(archer, target);
                        UpdateUnitHealthBar(target);
                    }
                    else
                    {
                        AddLogMessage($"{archer.Name} стреляет в {target.Name}, но промахивается!");
                    }
                }

                if (unit is Healer healer)
                {
                    // Сохраняем здоровье союзников до лечения
                    Dictionary<IUnit, float> healthBefore = new Dictionary<IUnit, float>();
                    foreach (IUnit ally in unit.Team.QueueFighters)
                    {
                        healthBefore[ally] = ally.Health;
                    }

                    healer.DoHeal(unit.Team);

                    // Проверяем, кто был вылечен
                    bool healingPerformed = false;
                    foreach (IUnit ally in unit.Team.QueueFighters)
                    {
                        if (healthBefore[ally] < ally.Health)
                        {
                            float healingDone = ally.Health - healthBefore[ally];
                            AddLogMessage($"{healer.Name} лечит {ally.Name}, восстанавливая {healingDone} HP!");
                            AnimateHealing(healer, ally);
                            UpdateUnitHealthBar(ally);
                            healingPerformed = true;
                        }
                    }

                    if (!healingPerformed)
                    {
                        AddLogMessage($"{healer.Name} не нашел раненых союзников для лечения.");
                    }
                }
            }
        }

        private void AnimateAttack(IUnit attacker, IUnit defender)
        {
            if (!unitPanels.ContainsKey(attacker) || !unitPanels.ContainsKey(defender))
            {
                return;
            }

            // Получаем панели атакующего и защищающегося
            Panel attackerPanel = unitPanels[attacker];
            Panel defenderPanel = unitPanels[defender];

            // Мигаем цветом панели защищающегося для обозначения получения урона
            defenderPanel.BackColor = Color.Orange;
            Application.DoEvents();
            System.Threading.Thread.Sleep(100);
            defenderPanel.BackColor = GetUnitColor(defender);
        }

        private void AnimateArcherAttack(Archer archer, IUnit target)
        {
            if (!unitPanels.ContainsKey(archer) || !unitPanels.ContainsKey(target))
            {
                return;
            }

            // Мигаем цветом панели цели для обозначения получения урона от лучника
            Panel targetPanel = unitPanels[target];
            targetPanel.BackColor = Color.Yellow;
            Application.DoEvents();
            System.Threading.Thread.Sleep(100);
            targetPanel.BackColor = GetUnitColor(target);
        }

        private void AnimateHealing(Healer healer, IUnit target)
        {
            if (!unitPanels.ContainsKey(healer) || !unitPanels.ContainsKey(target))
            {
                return;
            }

            // Мигаем цветом панели цели для обозначения получения лечения
            Panel targetPanel = unitPanels[target];
            targetPanel.BackColor = Color.Green;
            Application.DoEvents();
            System.Threading.Thread.Sleep(100);
            targetPanel.BackColor = GetUnitColor(target);
        }

        private void UpdateUnitHealthBar(IUnit unit)
        {
            if (!unitPanels.ContainsKey(unit))
            {
                return;
            }

            // Обновляем полоску здоровья юнита
            Panel unitPanel = unitPanels[unit];
            foreach (Control control in unitPanel.Controls)
            {
                if (control is ProgressBar healthBar && control.Name == "healthBar")
                {
                    healthBar.Value = Math.Max(0, Math.Min(100, (int)unit.Health));

                    // Меняем цвет полоски здоровья в зависимости от оставшегося здоровья
                    if (unit.Health < 25)
                        healthBar.ForeColor = Color.Red;
                    else if (unit.Health < 50)
                        healthBar.ForeColor = Color.Orange;
                    else
                        healthBar.ForeColor = Color.Green;
                }
            }
        }

        private void UpdateTeamStatusInLog()
        {
            AddLogMessage("\n=== СОСТОЯНИЕ КОМАНД ===");

            // Отображаем состояние красной команды
            AddLogMessage("Красная команда:");
            if (redTeam.HasFighters())
            {
                int position = 1;
                foreach (IUnit unit in redTeam.QueueFighters)
                {
                    AddLogMessage($"  {position}. {unit.Name} - HP: {unit.Health}");
                    position++;
                }
            }
            else
            {
                AddLogMessage("  Нет бойцов");
            }

            // Отображаем состояние синей команды
            AddLogMessage("\nСиняя команда:");
            if (blueTeam.HasFighters())
            {
                int position = 1;
                foreach (IUnit unit in blueTeam.QueueFighters)
                {
                    AddLogMessage($"  {position}. {unit.Name} - HP: {unit.Health}");
                    position++;
                }
            }
            else
            {
                AddLogMessage("  Нет бойцов");
            }
        }

        private void AddLogMessage(string message)
        {
            battleLog.AppendText(message + Environment.NewLine);
            battleLog.ScrollToCaret();
        }

        private void EndBattle()
        {
            battleEnded = true;
            string winnerTeam = redTeam.HasFighters() ? "Красных" : "Синих";

            AddLogMessage("\n=== КОНЕЦ БИТВЫ ===");
            AddLogMessage($"Победила команда {winnerTeam}!");

            // Изменяем текст кнопки
            nextRoundButton.Text = "Битва окончена";
            nextRoundButton.Enabled = false;
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            // Переход в главное меню
            MainMenuForm mainMenuForm = new MainMenuForm();
            this.Hide();
            mainMenuForm.ShowDialog();
            this.Close();
        }
    }
}