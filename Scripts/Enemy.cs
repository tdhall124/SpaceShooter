﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private Player _player;

    private Animator _animExplosion;

    [SerializeField] private AudioSource _audioSourceExplosion;
    [SerializeField] private AudioSource _audioSourceLaser;

    [SerializeField] private GameObject _laserPrefab;

    [SerializeField] private float _fireRate = 3.0f;
    //value we are going to use to compare Time.time which is
    //how long the game has been running
  
    private float _canFire = -1;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.LogError("Enemy::Start(): Player is NULL.");
        _animExplosion = GetComponent<Animator>();
        if (_animExplosion == null)
            Debug.LogError("Enemy::Start(): Animator is NULL.");
        if (_audioSourceExplosion == null)
            Debug.LogError("Enemy::Start(): Audio Source Explosion clip is NULL.");
        if (_audioSourceLaser == null)
            Debug.LogError("Enemy::Start(): Audio Source Laser is NULL.");
    }
    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
            FireLaser();
    }
    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -7f)
        {
            float randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 7f, 0);
        }
    }
    void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
        _audioSourceLaser.Play();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            
            if (player != null)
                player.Damage();
            _animExplosion.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSourceExplosion.Play();
            Destroy(this.gameObject, 2.8f);
        }
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.UpdateScore(10);
            }
            _animExplosion.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSourceExplosion.Play();

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }
}
