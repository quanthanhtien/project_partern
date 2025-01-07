using System;
using UnityEngine;

public class TestShader : MonoBehaviour
{
    public Material material;
    public Vector3 _color;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetColor();
        }
    }

    void SetColor()
    {
        _color += new Vector3(0.1f, 0.1f, 0.1f);
        Color color = new Color(_color.x, _color.y, _color.z);
        material.SetColor("_Color", color);
    }
}
