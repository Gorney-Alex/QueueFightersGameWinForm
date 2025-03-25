using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal class GameManager
    {

        private Team redTeam;
        private Team blueTeam;

        public GameManager()
        {
            redTeam = new Team("Red", 100);
            blueTeam = new Team("Blue", 100);

            CreatFighters();
            Battle();
        }

        public void CreatFighters()
        {
            WeakFighter weakFighter1 = new WeakFighter();
            WeakFighter weakFighter2 = new WeakFighter();
            StrongFighter strongFighter1 = new StrongFighter();
            StrongFighter strongFighter2 = new StrongFighter();
            Archer archer1 = new Archer("Red_Archer");
            Archer archer2 = new Archer("Blue_Archer");
            Archer archer3 = new Archer("Blue_Archer");
            Archer archer4 = new Archer("Red_Archer");
            Healer healer1 = new Healer("Red_Healer");
            Healer healer2 = new Healer("Blue_Healer");

            redTeam.AddFighter(strongFighter2);
            redTeam.AddFighter(weakFighter1);
            redTeam.AddFighter(archer1);
            redTeam.AddFighter(healer1);
            redTeam.AddFighter(archer4);

            Console.WriteLine($"Money RedTeam {redTeam.Money}");

            Console.WriteLine("---");

            blueTeam.AddFighter(weakFighter2);
            blueTeam.AddFighter(strongFighter1);
            blueTeam.AddFighter(archer2);
            blueTeam.AddFighter(healer2);
            blueTeam.AddFighter(archer3);


            Console.WriteLine($"Money BlueTeam {blueTeam.Money}");
        }

        private Team RandomStartAttack()
        {
            Random randomStartAttack = new Random();
            bool redTeamStarts = randomStartAttack.Next(2) == 0;
            if (redTeamStarts)
            {
                return redTeam;
            }
            return blueTeam;
        }

        public void Battle()
        {
            Team attackingTeam = RandomStartAttack();
            Team defendingTeam = (attackingTeam == redTeam) ? blueTeam : redTeam;

            while (redTeam.HasFighters() && blueTeam.HasFighters())
            {
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey();

                redTeam.ShowTeam();
                blueTeam.ShowTeam();

                Console.WriteLine("\n--- Новый раунд ---");
                Console.WriteLine($"Ходит команда: {(attackingTeam == redTeam ? "Красная" : "Синяя")}");

                IUnit attacker = attackingTeam.GetNextFighter();
                IUnit defender = defendingTeam.GetNextFighter();

                Console.WriteLine($"\n{attacker.Name} | HP: {attacker.Health} атакует {defender.Name}| HP: {defender.Health}");
                attacker.Attack(defender);

                foreach (IUnit unit in attackingTeam.QueueFighters.Skip(1))
                {
                    if (unit is Archer archer)
                    {
                        archer.DoSpecialAttack(defender, attacker.Team);
                    }
                    if (unit is Healer healer)
                    {
                        healer.DoHeal(attacker.Team);
                    }
                }

                if (defender.Health <= 0)
                {
                    Console.WriteLine($"{defender.Name} пал в бою!");
                    defendingTeam.RemoveFighter();
                }

                (attackingTeam, defendingTeam) = (defendingTeam, attackingTeam);
            }

            Console.WriteLine(redTeam.HasFighters() ? "Красная команда победила!" : "Синяя команда победила!");
        }



    }
}
