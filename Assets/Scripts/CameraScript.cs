using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private void Start()
    {
        Vector3 gridCenter =  GameField.Instance.GetGridCenter(GameManager.Instance.GetWidth(), GameManager.Instance.GetHeight());
        transform.position = new Vector3(gridCenter.x, gridCenter.y, gridCenter.z - 5);
    }
}
