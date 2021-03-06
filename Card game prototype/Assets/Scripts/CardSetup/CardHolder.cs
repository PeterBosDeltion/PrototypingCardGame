﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardHolder : MonoBehaviour
{

    public Card card;
    public CurrentDeck deck;
    public ManaCount mana;

    public enum Side
    {
        Enemy,
        Player
    }
    public Side side;

    protected bool canDissolve;

    [Header("Card UI Setup")]
    public TextMeshProUGUI nameText;
    public Image iconImage;
    [Space(10)]
    public Transform manaCrystalHolder;
    public GameObject manaCrystal;
    [Space(10)]
    public Transform attributeHolder;
    public GameObject attributePrefab;
    public List<Sprite> attributeIcons = new List<Sprite>();
    public List<Sprite> cardIcons = new List<Sprite>();

    protected Image[] images;
    protected List<Image> toDissolve = new List<Image>();

    private void Awake()
    {
        if (!FightManager.inFight)
        {
            LoadCard();
        }
    }

    private void Update()
    {
        if (canDissolve)
        {
            DissolveCard();
        }
    }

    public void LoadCard()
    {
        for (int i = 0; i < card.manaCost; i++)
        {
            Instantiate(manaCrystal, manaCrystalHolder.position, Quaternion.identity, manaCrystalHolder);
        }

        nameText.text = card.cardName;

        if (card.categories.Contains(Card.Category.Damage) && card.categories.Count == 1)
        {
            iconImage.sprite = cardIcons[0];
        }
        if (card.categories.Contains(Card.Category.DOT) && card.categories.Count == 1)
        {
            iconImage.sprite = cardIcons[1];
        }
        if (card.categories.Contains(Card.Category.Heal) && card.categories.Count == 1)
        {
            iconImage.sprite = cardIcons[3];
        }
        if (card.categories.Contains(Card.Category.HOT) && card.categories.Count == 1)
        {
            iconImage.sprite = cardIcons[4];
        }
        if (card.categories.Contains(Card.Category.Draw) && card.categories.Count == 1)
        {
            iconImage.sprite = cardIcons[2];
        }
        if (card.categories.Contains(Card.Category.SpellPower) && card.categories.Count == 1)
        {
            iconImage.sprite = cardIcons[7];
        }
        if (card.categories.Contains(Card.Category.StealCard) && card.categories.Count == 1)
        {
            iconImage.sprite = cardIcons[8];
        }
        if (card.categories.Count > 1)
        {
            iconImage.sprite = cardIcons[6];
        }

        card.Setup(this);

        images = GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            if (image.material.name == "Card")
            {
                image.material = new Material(image.material);
                toDissolve.Add(image);
            }
        }
    }

    public void CreateAttribute(CardAttribute.Type type, int value1)
    {
        GameObject newAttribute = Instantiate(attributePrefab, attributeHolder.position, Quaternion.identity, attributeHolder);
        CardAttribute attribute = newAttribute.GetComponent<CardAttribute>();

        switch (type)
        {
            case CardAttribute.Type.Damage:

                attribute.Setup(attributeIcons[0], value1.ToString(), FightManager.instance.damageColor);
                break;
            case CardAttribute.Type.Heal:

                attribute.Setup(attributeIcons[1], value1.ToString(), FightManager.instance.healColor);
                break;
            case CardAttribute.Type.SpellPower:

                if (value1 > 0)
                {
                    attribute.Setup(attributeIcons[5], value1.ToString(), FightManager.instance.healColor);
                }
                else
                {
                    attribute.Setup(attributeIcons[5], value1.ToString(), FightManager.instance.damageColor);
                }
                break;
            case CardAttribute.Type.StealCard:

                attribute.Setup(attributeIcons[7], value1.ToString(), FightManager.instance.healColor);
                break;
        }
    }

    public void CreateAttribute(CardAttribute.Type type, int value1, int value2)
    {
        GameObject newAttribute = Instantiate(attributePrefab, attributeHolder.position, Quaternion.identity, attributeHolder);
        CardAttribute attribute = newAttribute.GetComponent<CardAttribute>();

        switch (type)
        {
            case CardAttribute.Type.DOT:

                attribute.Setup(attributeIcons[2], value1.ToString(), attributeIcons[3], value2.ToString(), FightManager.instance.damageColor);
                break;
            case CardAttribute.Type.HOT:

                attribute.Setup(attributeIcons[1], value1.ToString(), attributeIcons[3], value2.ToString(), FightManager.instance.healColor);
                break;
        }
    }

    public void CreateAttribute(CardAttribute.Type type, int value1, Character target)
    {
        GameObject newAttribute = Instantiate(attributePrefab, attributeHolder.position, Quaternion.identity, attributeHolder);
        CardAttribute attribute = newAttribute.GetComponent<CardAttribute>();

        switch (type)
        {
            case CardAttribute.Type.Draw:

                if (side == Side.Enemy && target == FightManager.instance.enemy)
                {
                    attribute.Setup(attributeIcons[6], value1.ToString(), FightManager.instance.healColor);
                }
                if (side == Side.Enemy && target == FightManager.instance.player)
                {
                    attribute.Setup(attributeIcons[6], value1.ToString(), FightManager.instance.damageColor);
                }

                if (side == Side.Player && target == FightManager.instance.enemy)
                {
                    attribute.Setup(attributeIcons[6], value1.ToString(), FightManager.instance.damageColor);
                }
                if (side == Side.Player && target == FightManager.instance.player)
                {
                    attribute.Setup(attributeIcons[6], value1.ToString(), FightManager.instance.healColor);
                }
                break;
        }
    }

    public void DissolveCard()
    {
        canDissolve = true;

        TextMeshProUGUI[] textObjects = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textObject in textObjects)
        {
            textObject.gameObject.SetActive(false);
        }

        foreach (Image image in toDissolve)
        {
            image.material.SetFloat("_Threshold", image.material.GetFloat("_Threshold") + (Time.deltaTime * 1.5f));
        }

        if (images[0].material.GetFloat("_Threshold") >= 0.85f)
        {
            Destroy(gameObject);
        }
    }

    public void UseButton()
    {
        if (FightManager.instance.turn == FightManager.Turn.player && side == Side.Player)
        {
            if (mana.CheckMana(card.manaCost) == true)
            {
                deck.RemoveFromHand(card);
                card.Use(this);
                TriggerUseCardAudio();
            }
            else
            {
                GetComponent<Animator>().SetTrigger("Normal");
            }
        }
        else if (FightManager.instance.turn == FightManager.Turn.enemy && side == Side.Enemy)
        {
            if (mana.CheckMana(card.manaCost) == true)
            {
                deck.RemoveFromHand(card);
                card.Use(this);
                TriggerUseCardAudio();
            }
            else
            {
                GetComponent<Animator>().SetTrigger("Normal");
            }
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Normal");
            print("test print voor animation bug");
        }
    }

    public void TriggerAudio()
    {
        AudioManager.instance.PlayClickSound();
    }

    public void TriggerUseCardAudio()
    {
        AudioManager.instance.PlayCardUseSound();
    }
}
