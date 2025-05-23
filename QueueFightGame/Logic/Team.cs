﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueFightGame
{
    public class Team
    {
        public List<IUnit> Fighters { get; private set; }
        public string TeamName { get; private set; }
        public float InitialMoney { get; private set; }
        public float CurrentMoney { get; private set; }
        public bool IsPlayerControlled { get; private set; }

        public Team(string teamName, float initialMoney, bool isPlayer = true)
        {
            TeamName = teamName;
            Fighters = new List<IUnit>();
            InitialMoney = initialMoney;
            CurrentMoney = initialMoney;
            IsPlayerControlled = isPlayer;
        }

        public bool CanAfford(IUnit unit)
        {
            return CurrentMoney >= unit.Cost;
        }
        public bool CanAfford(float cost)
        {
            return CurrentMoney >= cost;
        }
        public bool CanAfford(string unitTypeName)
        {
            if (UnitConfig.Stats.TryGetValue(unitTypeName, out var data))
            {
                return CurrentMoney >= data.Cost;
            }
            return false;
        }

        public bool AddFighter(IUnit fighter, ILogger logger)
        {
            if (CanAfford(fighter))
            {
                fighter.Team = this;
                CurrentMoney -= fighter.Cost;
                Fighters.Add(fighter);
                logger?.Log($"Добавлен {fighter.Name} в команду {TeamName}. Осталось денег: {CurrentMoney}");
                return true;
            }
            else
            {
                logger?.Log($"Недостаточно денег у команды {TeamName} для покупки {fighter.Name} (Нужно: {fighter.Cost}, Есть: {CurrentMoney})");
                return false;
            }
        }

        public void RemoveFighter(IUnit fighter, ILogger logger, bool refund = false)
        {
            if (Fighters.Remove(fighter))
            {
                logger?.Log($"{fighter.Name} удален из команды {TeamName}.");
                if (refund)
                {
                    CurrentMoney += fighter.Cost;
                    logger?.Log($"Возвращено {fighter.Cost} денег. Текущий баланс: {CurrentMoney}");
                }
                fighter.Team = null;
            }
        }


        public bool HasFighters()
        {
            return Fighters.Any(f => f.Health > 0);
        }

        public List<IUnit> GetLivingFighters()
        {
            return Fighters.Where(f => f.Health > 0).ToList();
        }

        public void AddFighterAt(int index, IUnit fighter)
        {
            if (fighter == null)
                throw new ArgumentNullException(nameof(fighter));
                
            if (index < 0 || index > Fighters.Count)
                index = Fighters.Count;
                
            fighter.Team = this;
            Fighters.Insert(index, fighter);
        }


        public IUnit GetNextFighter()
        {
            return Fighters.FirstOrDefault(f => f.Health > 0);
        }

        public void RemoveDeadFighters(ILogger logger)
        {
            List<IUnit> deadFighters = Fighters.Where(f => f.Health <= 0).ToList();
            foreach (var deadUnit in deadFighters)
            {
                logger.Log($"{deadUnit.Name} ({TeamName}) погибает!");
                Fighters.Remove(deadUnit);
            }
        }

        public void ResetUnitsForNewBattle()
        {
            foreach (var unit in Fighters)
            {
                unit.Health = unit.MaxHealth;
                if (unit is ISpecialActionUnit specialUnit)
                {
                    specialUnit.HasUsedSpecial = false;
                }
                if (unit is ISpecialActionWeakFighter squire){ }
                if (unit is StrongFighter knight) { }
            }
        }
    }
}