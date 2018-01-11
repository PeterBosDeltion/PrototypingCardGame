﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Draw")]
public class Card_Draw : Card
{

    [Header("Draw Card Attributes")]
    public int amountToDraw;

    public override void Setup(CardHolder myHolder)
    {
        base.Setup(myHolder);

        //myHolder.CreateAttribute(0, amountToDraw, 1);
    }

    public override void Use(CardHolder myHolder)
    {
        base.Use(myHolder);

        for (int i = 0; i < amountToDraw; i++)
        {
            FightManager.instance.myDeck.GetNewCard();
        }
    }
}