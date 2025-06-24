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


    public IEnumerator MoveToPosition(CellVisual startCell, CellVisual targetCell, float duration)
    {
        CellVisual cell = new CellVisual(
            startCell.cellPosition,
            startCell.spriteRenderer.sprite,
            startCell.gameObject
        );

        startCell.spriteRenderer.sprite = null;

        Vector3 startPos = startCell.gameObject.transform.position;
        Vector3 targetPos = targetCell.gameObject.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            cell.gameObject.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        cell.gameObject.transform.position = targetPos;
        cell.Destroy();
    }

    public IEnumerator MergeAnimation(CellVisual startCell, CellVisual targetCell, float duration)
    {
        CellVisual cell = new CellVisual(
            startCell.cellPosition,
            startCell.spriteRenderer.sprite,
            startCell.gameObject
        );

        startCell.spriteRenderer.sprite = null;

        Vector3 startPos = startCell.gameObject.transform.position;
        Vector3 targetPos = targetCell.gameObject.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            Color color = cell.spriteRenderer.color;
            color.a -= Time.deltaTime / duration;
            cell.spriteRenderer.color = color;
            cell.gameObject.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        cell.gameObject.transform.position = targetPos;
        cell.Destroy();
    }

    public IEnumerator MoveJumperToPosition(CellVisual startCell, CellVisual targetCell, float duration)
    {
        CellVisual cell = new CellVisual(
            startCell.cellPosition,
            startCell.spriteRenderer.sprite,
            startCell.gameObject
        );

        startCell.spriteRenderer.sprite = null;

        Vector3 startPos = startCell.gameObject.transform.position;
        Vector3 targetPos = targetCell.gameObject.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration/2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration/2);
            cell.gameObject.transform.position = Vector3.Lerp(startPos, new Vector3(startPos.x, startPos.y+15), t);
            yield return null;
        }
        while (elapsedTime < duration && elapsedTime > duration/2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            cell.gameObject.transform.position = Vector3.Lerp(new Vector3(targetPos.x, targetPos.y+15), targetPos, t);
            yield return null;
        }

        cell.gameObject.transform.position = targetPos;
        cell.Destroy();
    }
    public void Destroy()
    {
        UnityEngine.Object.Destroy(gameObject);
    }
}
