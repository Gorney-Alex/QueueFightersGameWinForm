using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    // Меняем модификатор доступа с internal на public
    public interface IUnit
    {
        string Name { get; }
        float Health { get; set; }
        float Protection { get; }
        float Damage { get; }
        float Cost { get; set; }
        Team Team { get; set; }
        void Attack(IUnit target);
    }
}