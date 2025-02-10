using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private SoundIntensityController _soundIntensityController;
    [SerializeField] private GoalType goal;
    [SerializeField] private CatCharacter playerCharacter;

    private readonly Dictionary<Collider2D, int> _hitColliders = new();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball") && !_hitColliders.ContainsKey(other))
        {
            Debug.Log("Despawning!");
            _hitColliders.Add(other, 1);
            if (playerCharacter) playerCharacter.Attack();
            
            other.gameObject.GetComponent<CatchinkoBall>().Despawn(goal);
            StartCoroutine(RemoveColliderFromCollection(other));
        }
    }

    private IEnumerator RemoveColliderFromCollection(Collider2D col)
    {
        yield return new WaitForSeconds(5);
        _hitColliders.Remove(col);
    }

    private void Start()
    {
        var audioSource = GetComponent<AudioSource>();
        _soundIntensityController = new SoundIntensityController(audioSource, this);
    }

    public void SetIntensity(float normalizedDistance)
    {
        _soundIntensityController.SetIntensity(normalizedDistance);
    }

    public void Stop()
    {
        if (_soundIntensityController != null) _soundIntensityController.Stop();
    }
}