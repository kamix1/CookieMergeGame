using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3Int cellPosition;
    public CellType type;
    private bool isOccupied;
    public CookieType cookieType;
    public bool isPlayable;
    public bool isEmpty;
    public enum CookieType
    {
        cookie,
        toast,
        mafin,
        pankeki,
        cake,
        doubleCake,
        doubleGlazurCake,
        gingerbreadMan,
        gingerbreadManCoctail,
        gingerbreadManSet,
        unknown
    }

    public void Merge()
    {
        if (cookieType < CookieType.doubleGlazurCake)
        {
            cookieType++;
        }
        if (cookieType > CookieType.doubleGlazurCake && cookieType < CookieType.unknown)
        {
            cookieType++;
        }
    }
    public enum CellType
    {
        empty,
        cookie,
        plate,
    }
}
