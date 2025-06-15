using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPlacing : MonoBehaviour
{

    [SerializeField] private Sprite cookie;
    [SerializeField] private Sprite toast;
    [SerializeField] private Sprite mafin;
    [SerializeField] private Sprite pancake;
    private string[] cookies = { "cookie", "toast", "mafin", "pancake" };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Sprite GetNextPlacingSprite()
    {
        string name = GameManager.Instance.GetNextPlacible();
        if(name != null)
        {
            if (name == cookies[0])
            {
                return cookie;
            }
            else if (name == cookies[1])
            {
                return toast;
            }
            else if (name == cookies[2])
            {
                return mafin;
            }
            else if (name == cookies[3])
            {
                return pancake;
            }
        }
        else
        {
            Debug.Log("name is null");
        }
        return cookie;
    }
}
