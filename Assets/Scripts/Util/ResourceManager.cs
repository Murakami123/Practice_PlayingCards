using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingletonMonoBehaviour<ResourceManager>
{
    [SerializeField] private Sprite[] spadeSprites;
    [SerializeField] private Sprite[] heartSprites;
    [SerializeField] private Sprite[] diamondSprites;
    [SerializeField] private Sprite[] cloverSprites;
    [SerializeField] private Sprite jokerSprite;

    public Sprite GetSprite(CardSoot soot, int cardNo)
    {
        Sprite[] array = new Sprite[0];
        switch (soot)
        {
            case CardSoot.Spade:   array = spadeSprites; break;
            case CardSoot.Heart:   array = heartSprites; break;
            case CardSoot.Diamond: array = diamondSprites; break;
            case CardSoot.Clover:  array = cloverSprites; break;
            case CardSoot.Joker:  return jokerSprite;
            default: Debug.LogError("Soot がおかしい"); break;
        }
        return array[cardNo - 1];
    }
}
