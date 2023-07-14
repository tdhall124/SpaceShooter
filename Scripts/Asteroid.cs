using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 20.0f;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private SpawnManager _spawnManager;
    //[SerializeField] private AudioSource _audioSourceExplosion;

    private void Start()
    {
        //_spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_explosionPrefab == null)
            Debug.LogError("The Explosion prefab is NULL.");
        if (_spawnManager == null)
            Debug.LogError("The Spawn_Manager is NULL.");
        //if (_audioSourceExplosion == null)
            //Debug.LogError("Asteroid::Start(): Audio Source Explosion clip is NULL.");
    }
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }
    // check for Laser collision (Trigger)
    // instantiate explosion at the position of the astroid
    // destroy the explosion after 3 seconds
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject); // but Laser.cs has self-destruct in it already
            _spawnManager.StartSpawning();
            //Destroy(_explosion.transform.gameObject, 2.8f);
            //_audioSourceExplosion.Play();
            Destroy(this.gameObject, 0.25f);
        }
    }
}
