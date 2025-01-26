using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverWorldPlayer : MonoBehaviour
{
    [SerializeField] private WorldSpacePanelController worldSpacePanelController;
    
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    
    [SerializeField] private int catCash;
    
    // stats object for the character being played
    // this will have various stats + rebirths

    public void ShowPanel() => worldSpacePanelController.ShowPanel();
    public void HidePanel() => worldSpacePanelController.MinimizePanel();
    
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
