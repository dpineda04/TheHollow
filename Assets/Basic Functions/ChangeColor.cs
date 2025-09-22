using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Color currentColor = Color.gray;
    private Renderer objectRenderer;
  
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        if(objectRenderer == null )
        {
            Debug.LogError("no renderer component found on this object :(");
        }

    }

    public void ChangeObjectColor(Color newColor)
    {
        objectRenderer.material.color = newColor;
    }
}