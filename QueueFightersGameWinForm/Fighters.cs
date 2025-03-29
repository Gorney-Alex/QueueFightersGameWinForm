using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    // Меняем модификатор доступа с internal на public для всех классов бойцов
    public class WeakFighter : BaseUnit, ICanBeHealed
    {
        public WeakFighter() : base("WeakFighter", 100f, 0.7f, 40, 15) { }
    }

    public class StrongFighter : BaseUnit
    {
        public StrongFighter() : base("StrongFighter", 100f, 0.5f, 60, 30) { }
    }

    public class Healer : BaseUnit, ISpecialActionHealer
    {
        public int Range { get; private set; }
        public int Power { get; private set; }

        public Healer(string name) : base(name, 100f, 1f, 5, 20)
        {
            Range = 3;
            Power = 15;
        }

        public void DoHeal(Team ownTeam)
        {
            // Получаем индекс целителя в очереди
            int healerIndex = ownTeam.QueueFighters.ToList().FindIndex(unit => unit == this);

            // Получаем список всех юнитов
            List<IUnit> allUnits = ownTeam.QueueFighters.ToList();

            // Проверяем юнита справа (если он существует и находится в радиусе)
            ICanBeHealed targetToHeal = null;

            // Сначала проверяем юнита справа (индекс healerIndex - 1)
            if (healerIndex > 0 && healerIndex - 1 < allUnits.Count)
            {
                IUnit rightUnit = allUnits[healerIndex - 1];
                if (rightUnit is ICanBeHealed healableUnit && rightUnit.Health < 100)
                {
                    targetToHeal = healableUnit;
                }
            }

            // Если справа нет подходящего юнита, проверяем слева (индекс healerIndex + 1)
            if (targetToHeal == null && healerIndex + 1 < allUnits.Count)
            {
                IUnit leftUnit = allUnits[healerIndex + 1];
                if (leftUnit is ICanBeHealed healableUnit && leftUnit.Health < 100)
                {
                    targetToHeal = healableUnit;
                }
            }

            // Если нашли кого лечить
            if (targetToHeal != null)
            {
                Random random = new Random();
                int amountHealth = random.Next(0, Power + 1);

                targetToHeal.Health += amountHealth;
                if (targetToHeal.Health > 100) targetToHeal.Health = 100;

                Console.WriteLine($"{Name} лечит {((IUnit)targetToHeal).Name}, восстанавливая {amountHealth} HP!");
            }
            else
            {
                Console.WriteLine($"{Name} не нашел раненых союзников справа или слева от себя.");
            }
        }
    }

    public class Mage : BaseUnit, ICanBeHealed
    {
        public Mage() : base("Mage", 100f, 0.8f, 20, 25) { }
        public Mage(string name) : base(name, 100f, 0.8f, 20, 25) { }

        // Rest of the class remains the same
        public void Attack(IUnit target)
        {
            float magicDamage = Damage * 0.7f; // 70% урона проходит сквозь защиту
            float physicalDamage = Damage * 0.3f * target.Protection;
            float totalDamage = magicDamage + physicalDamage;
            target.Health -= totalDamage;
            Console.WriteLine($"{Name} использует магию, нанося {totalDamage} урона {target.Name}!");
        }
    }

    public class Archer : BaseUnit, ISpecialActionArcher, ICanBeHealed
    {
        public int Range { get; set; }
        public int Power { get; set; }

        public Archer(string name) : base(name, 100f, 0.9f, 5, 25)
        {
            Range = 3;
            Power = 15;
        }

        public void DoSpecialAttack(IUnit target, Team ownTeam)
        {
            int archerIndex = ownTeam.QueueFighters.ToList().FindIndex(unit => unit == this);
            if (archerIndex >= Range)
            {
                Console.WriteLine($"{Name} не может стрелять, его обзор закрыт!");
                return;
            }

            Random random = new Random();
            bool isHit = random.Next(100) < 70;

            if (isHit)
            {
                float newDamage = Power * target.Protection;
                Console.WriteLine($"{Name} стреляет в {target.Name} и попадает, нанося {newDamage} урона!");
                target.Health -= newDamage;
            }
            else
            {
                Console.WriteLine($"{Name} стреляет в {target.Name}, но промахивается!");
            }
        }
    }
}