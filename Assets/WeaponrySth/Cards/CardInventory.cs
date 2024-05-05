using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardInventory
{
    public List<ICard> Cards { get; }

    public int Capasity { get; }

    public int Count { get; private set; } = 0;

    public CardInventory(int capasity = 20)
    {
        Capasity = capasity;
        Cards = new List<ICard>();
        for (int i = 0; i < Capasity; i++)
        {
            Cards.Add(null);
        }
    }

    public bool TryAddCard(ICard card)
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            if (Cards[i] == null)
            {
                Cards[i] = card;
                Count++;
                return true;
            }
        }
        return false;
    }

    public void RemoveCard(ICard card)
    {
        if (Cards.Remove(card))
        {
            Count--;
        }
    }
}
