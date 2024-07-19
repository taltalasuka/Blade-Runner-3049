using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    private List<SpawnPoint> _spawnPointList;
    private List<Character> _spawnedCharacters;
    private bool _hasSpawned;
    public UnityEvent onAllSpawnedCharacterEliminated;

    private void Awake()
    {
        var spawnPointArray = transform.parent.GetComponentsInChildren<SpawnPoint>();
        _spawnPointList = new List<SpawnPoint>(spawnPointArray);
        _spawnedCharacters = new List<Character>();
    }

    private void Update()
    {
        if (!_hasSpawned || _spawnedCharacters.Count == 0)
        {
            return;
        }
        bool allSpawnedAreDead = true;
        foreach (Character c in _spawnedCharacters)
        {
            if (c.currentState != Character.CharacterState.Dead)
            {
                allSpawnedAreDead = false;
                break;
            }
        }

        if (allSpawnedAreDead)
        {
            if (onAllSpawnedCharacterEliminated != null)
            {
                onAllSpawnedCharacterEliminated.Invoke();
            }
            _spawnedCharacters.Clear();
        }
    }

    public void SpawnCharacters()
    {
        if (_hasSpawned)
        {
            return;
        }
        _hasSpawned = true;
        foreach (SpawnPoint sp in _spawnPointList)
        {
            if (sp.enemyToSpawn != null)
            {
                GameObject spawnedGameObject = Instantiate(sp.enemyToSpawn, sp.transform.position, sp.transform.rotation);
                _spawnedCharacters.Add(spawnedGameObject.GetComponent<Character>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnCharacters();
        }
    }
}
