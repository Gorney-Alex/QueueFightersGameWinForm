using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    // Меняем модификатор доступа с internal на public
    public abstract class BaseUnit : IUnit
    {
        public string Name { get; private set; }
        public float Health { get; set; }
        public float Protection { get; private set; }
        public float Damage { get; private set; }
        public float Cost { get; set; }
        public Team Team { get; set; }

        public BaseUnit(string name, float health, float protection, float damage, float cost)
        {
            Name = name;
            Health = health;
            Protection = protection;
            Damage = damage;
            Cost = cost;
            Team = null;
        }

        public void Attack(IUnit target)
        {
            float newDamage = Damage * target.Protection;
            target.Health -= newDamage;
            Console.WriteLine($"Наносит урон {newDamage} {target.Name}");
        }
    }
}