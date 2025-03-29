using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{

    public class UnitFactory
    {
        public IUnit CreateUnit(string type, string teamName = "")
        {
            switch (type.ToLower())
            {
                case "weakfighter":
                    return new WeakFighter();
                case "strongfighter":
                    return new StrongFighter();
                case "archer":
                    return new Archer($"{teamName}_Archer");
                case "healer":
                    return new Healer($"{teamName}_Healer");
                case "mage":
                    return new Mage($"{teamName}_Mage");
                default:
                    throw new ArgumentException($"Неизвестный тип бойца: {type}");
            }
        }
    }

    // Явно устанавливаем public для класса GameManager
    public class GameManager
    {
        // Публичный доступ к командам для использования в UI
        public Team RedTeam { get; private set; }
        public Team BlueTeam { get; private set; }

        // Конструктор для ручного создания команд (используется при покупке юнитов)
        public GameManager(Team redTeam, Team blueTeam)
        {
            RedTeam = redTeam;
            BlueTeam = blueTeam;
        }

        // Конструктор для автоматического создания команд (для режима "Рандом")
        public GameManager()
        {
            RedTeam = new Team("Red", 100);
            BlueTeam = new Team("Blue", 100);
            CreateRandomTeams();
        }

        // Метод для случайного создания команд
        private void CreateRandomTeams()
        {
            // Типы доступных бойцов
            Type[] fighterTypes = new Type[]
            {
                typeof(WeakFighter),
                typeof(StrongFighter),
                typeof(Archer),
                typeof(Healer),
                typeof(Mage)
            };

            // Стоимость каждого типа бойца
            Dictionary<Type, float> costs = new Dictionary<Type, float>
            {
                { typeof(WeakFighter), 15 },
                { typeof(StrongFighter), 30 },
                { typeof(Archer), 25 },
                { typeof(Healer), 20 },
                { typeof(Mage), 35 },
            };

            // Случайное создание бойцов для красной команды
            CreateRandomFightersForTeam(RedTeam, fighterTypes, costs);

            // Случайное создание бойцов для синей команды
            CreateRandomFightersForTeam(BlueTeam, fighterTypes, costs);
        }

        // Создание случайных бойцов для команды
        private void CreateRandomFightersForTeam(Team team, Type[] fighterTypes, Dictionary<Type, float> costs)
        {
            Random random = new Random();

            // Продолжаем добавлять бойцов, пока у команды есть деньги для самого дешёвого бойца
            while (team.Money >= costs.Values.Min())
            {
                // Выбираем случайный тип бойца
                Type randomType = fighterTypes[random.Next(fighterTypes.Length)];

                // Проверяем, достаточно ли денег для этого типа
                if (team.Money >= costs[randomType])
                {
                    // Создаем бойца и добавляем в команду
                    IUnit fighter = CreateFighter(randomType, team.TeamName);
                    team.AddFighter(fighter);
                }
                else
                {
                    // Пробуем выбрать другой тип бойца в следующей итерации
                    continue;
                }
            }
        }

        // Вспомогательный метод для создания бойца по типу
        private IUnit CreateFighter(Type fighterType, string teamName)
        {
            if (fighterType == typeof(WeakFighter))
                return new WeakFighter();
            else if (fighterType == typeof(StrongFighter))
                return new StrongFighter();
            else if (fighterType == typeof(Archer))
                return new Archer($"{teamName}_Archer");
            else if (fighterType == typeof(Healer))
                return new Healer($"{teamName}_Healer");
            else if (fighterType == typeof(Mage))
                return new Mage($"{teamName}_Mage");
            else
                throw new ArgumentException($"Неизвестный тип бойца: {fighterType.Name}");
        }

        // Метод для определения случайной атакующей команды
        public Team RandomStartAttack()
        {
            Random randomStartAttack = new Random();
            bool redTeamStarts = randomStartAttack.Next(2) == 0;
            return redTeamStarts ? RedTeam : BlueTeam;
        }
    }
}