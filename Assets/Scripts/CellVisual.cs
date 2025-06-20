using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellVisual
{
    public Vector3Int cellPosition;
    public GameObject gameObject;
    public SpriteRenderer spriteRenderer;

    public CellVisual(Vector3Int position, Sprite sprite, GameObject prefab)
    {
        cellPosition = position;

        // Создание объекта на позиции (в мире)
        Vector3 worldPosition = GameField.Instance.Tilemap.GetCellCenterWorld(position);
        Vector3 pos = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z-0.11f); 
        gameObject = GameObject.Instantiate(prefab, pos, Quaternion.identity);

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite; 
    }
}
