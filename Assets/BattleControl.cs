using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Battle
{
    public List<Unit> units = new List<Unit>();
    public List<Faction> factions = new List<Faction>();
    public int maxRound;
    public int currRound;
    public int turn;

    [Serializable]
    public class Faction
    {
        public int faction;
        public List<Unit> units = new List<Unit>();

        public bool AllDead()
        {
            return units.Sum(x => x.health) <= 0;
        }
    }

    private void PopulateFactions()
    {
        var dict = new Dictionary<int, Faction>();
        foreach (var unit in units)
        {
            if (!dict.TryGetValue(unit.faction, out var faction))
            {
                faction = new Faction();
                factions.Add(faction);
                dict.Add(unit.faction, faction);
            }
            faction.units.Add(unit);
        }
    }

    public Faction GetEnemyFaction(Unit unit)
    {
        return factions.FirstOrDefault(x=>x.faction!=unit.faction);
    }
    public IEnumerator StartBattle()
    {
        factions = new List<Faction>();
        
        Debug.Log("Battle begin");
        var order = units.OrderByDescending(x => x.speed).ToList();
        while (currRound < maxRound && !factions.Any(x => x.AllDead()))
        {
            Debug.Log($"Round {currRound}/{maxRound} started");
            foreach (var unit in order)
            {
                turn++;
                unit.TakeTurn(this);
            }

            currRound++;
        }
        yield return null;
    }
}


[Serializable]
public class Unit
{
    public string displayName;
    public int health;
    public int maxHealth;
    public int speed;
    public int position;
    public int faction;

    public void TakeTurn(Battle battle)
    {
        Debug.Log($"{displayName} takes turn {battle.turn}!");
    }
}
public class BattleControl : MonoBehaviour
{
    public Battle battle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(battle.StartBattle());
    }

    
}
