using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Goal zone: Score += " + scoreValue);
            Destroy(other.gameObject);
            GameResultsManager.GameOver(true);
        }
    }
}
