using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteMeshOverrideComponent : MonoBehaviour
{
    [SerializeField] private Mesh _meshOverride = null;
    [SerializeField] private int _maximumGraphicShaderLevel = 46; //Tessellation Support missing
    
    void Start()
    {
        if(SystemInfo.graphicsShaderLevel >= _maximumGraphicShaderLevel)
            return;
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Vector2 scaleFactor = new Vector2(spriteRenderer.sprite.rect.width/2, spriteRenderer.sprite.rect.height/2);
        Vector2 offset = new Vector2(spriteRenderer.sprite.rect.width/2, spriteRenderer.sprite.rect.height/2);
        Vector2[] newVertices = Array.ConvertAll<Vector3, Vector2>(_meshOverride.vertices, v => ((Vector2)v * scaleFactor) + offset);
        ushort[] triangles = Array.ConvertAll<int, ushort>(_meshOverride.triangles, t => (ushort)t);
        spriteRenderer.sprite.OverrideGeometry(
            newVertices,
            triangles
        );
    }
}
