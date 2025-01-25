using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    private Material _material;
    [SerializeField] private Vector2 zoomPulse = new(1.1f, 1.4f);
    [SerializeField] private bool pulsing = true;
    [Range(0, 1)]
    [SerializeField] private float timescale = 1f;
    
    void Start()
    {
    }

    void Update()
    {
        
        if (pulsing)
        {
            var scale = Mathf.PingPong(Time.time * timescale, zoomPulse.y - zoomPulse.x) + zoomPulse.x;
            transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}