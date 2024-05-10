using System;
using System.Collections.Generic;
using UnityEngine;

public class CardPlacer : MonoBehaviour, IPlacer
{
    private readonly Dictionary<SpellType, Dictionary<Spell, int>> instanceCount = new()
    {
        {
            SpellType.Projectile, new()
            {
                { Spell.Explosion, 1 },
                { Spell.GunShot, 1 },
                { Spell.Grenade, 1 },
                { Spell.Tracker, 1 },
                { Spell.CanonBall, 1 },
            }
        },
        {
            SpellType.Modifier, new()
            {
                { Spell.BouncinessIncrease, 1 },
                { Spell.DamageIncreaseConstant, 1 },
                { Spell.Piercing, 1 },
            }
        },
        {
            SpellType.Branching, new()
            {
                { Spell.AscendTree, 1 },
                { Spell.AscendTreeTwice, 1 },
            }
        }
    };

    private CardFactory cardFactory;

    private void Awake()
    { 
        cardFactory = GameObject.FindGameObjectWithTag("CardFactory").GetComponent<CardFactory>();
    }

    public void Place(PlacementManager manager)
    {
        foreach (var (spellType, dict) in instanceCount)
        {
            foreach (var (spell, count)  in dict)
            {
                for (int i = 0; i < count; i++)
                {
                    PlaceInstance(spell, manager);
                }
            }
        }
    }

    private void PlaceInstance(Spell spell, PlacementManager manager)
    {
        var position = manager.GetPosition(0);
        var instance = cardFactory.CreateCard(spell);
        var positionInUnity = manager.GetPositionInUnity(position) + GetShift(manager);
        cardFactory.CreateCardAvatar(instance, positionInUnity);
        manager.PlaceOnPlacementMap(position, 1);
    }

    private Vector3 GetShift(PlacementManager manager)
    {
        var x = (int)(manager.MazeBuilder.PassageThickness - 1) / 2;
        if (manager.Rnd.NextDouble() < 0.5)
            x = -x;
        var z = (int)(manager.MazeBuilder.PassageThickness - 1) / 2;
        if (manager.Rnd.NextDouble() < 0.5)
            z = -z;
        return new Vector3(x, 0, z);
    }
}