using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // MOVEMENT
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _speedBoost = 2f;
    [SerializeField] private float _thrusterBoost = 1.5f;
    private bool _isSpeedBoostActive = false;

    // LASER
    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField] private GameObject _laserPrefab;
    private Vector3 _laserOffset;
    [SerializeField] private AudioSource _audioSourceLaser;
    private const int _maxShots = 15;
    private int _shotCount = 15;

    [SerializeField] private int _lives = 3;
    private int _maxLives;

    private SpawnManager _spawnManager;

    // TRIPLE SHOT
    [SerializeField] private GameObject _tripleShotPrefab;
    private bool _isTripleShotActive = false;

    // MAGIC FLAME BOMB
    [SerializeField] private GameObject _magicFlameBomb;
    private bool _isMagicFlameBombActive = false;

    // SHIELD
    private bool _isShieldActive = false;
    [SerializeField] private GameObject[] _shieldVisualizer;
    private int _shieldHitCount = 0;

    // UI
    [SerializeField] private int score = 0;
    private UIManager _uiManager;

    // DAMAGE
    [SerializeField] private GameObject _leftEngineFire;
    [SerializeField] private GameObject _rightEngineFire;
    [SerializeField] private AudioSource _audioSourceExplosion;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null) Debug.LogError("Player::Start(): The SpawnManager is NULL.");

        if (_shieldVisualizer == null) Debug.LogError("Player::Start(): The Shield Visualizer is NULL.");

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null) Debug.LogError("Player::Start(): UI_Manager is NULL.");

        _laserOffset = new Vector3(0, 1.05f, 0);

        if (_leftEngineFire == null)
            Debug.LogError("Player::Start(): The Left_Engine_Fire is NULL.");
        if (_rightEngineFire == null)
            Debug.LogError("Player::Start(): The Right_Engine_Fire is NULL.");

        if (_audioSourceLaser == null)
            Debug.LogError("Player::Start(): Laser Shot audio source is NULL.");
        if (_audioSourceExplosion == null)
            Debug.LogError("Player::Start(): Explosion audio source is NULL.");

        _maxLives = _lives;
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _shotCount > 0)
            FireLaser();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isSpeedBoostActive)
        {
            _speed *= _thrusterBoost;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !_isSpeedBoostActive)
        {
            _speed /= _thrusterBoost;
        }

        //transform.Translate(direction * (_isSpeedBoostActive ? _speed * _speedBoost : _speed) * Time.deltaTime);
        transform.Translate(direction * _speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4.66f, 0), 0);

        if (transform.position.x > 9f)
        {
            transform.position = new Vector3(-9f, transform.position.y, 0);
        }
        else if (transform.position.x < -9f)
        {
            transform.position = new Vector3(9f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else if (_isMagicFlameBombActive)
        {
            Instantiate(_magicFlameBomb, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
            _shotCount--;
            _uiManager.UpdateShotsText(_shotCount);
        }
        _audioSourceLaser.Play();
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            //_shieldHitCount--;
            /*if (_shieldHitCount == 0)
            {
                _isShieldActive = false;
                // reset shieldVisualizer to its start color
                _shieldVisualizer[_shieldHitCount].SetActive(false);
                return;
            }*/
            switch (_shieldHitCount)
            {
                case 0:
                case 1:
                    Debug.Log("Player:Damage(): Shield0-1: " + _shieldHitCount);
                    _shieldVisualizer[_shieldHitCount].SetActive(false);
                    _shieldHitCount++;
                    _shieldVisualizer[_shieldHitCount].SetActive(true);
                    break;
                case 2:
                    Debug.Log("Player:Damage(): Shield2: " + _shieldHitCount);
                    _shieldVisualizer[_shieldHitCount].SetActive(false);
                    _isShieldActive = false;
                    _shieldHitCount = 0;
                    break;
                default:
                    Debug.Log("Player::Damage(): Default shield hit count found.");
                    break;
            }
            return;
        }

        _lives--;
        // lives is 2 right engine
        // lives is 1 left engine
        //int _randomPick = Random.Range(0, 1);
        if (_lives == 2)
        {
            _rightEngineFire.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngineFire.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _audioSourceExplosion.Play();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedBoost;
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _speed /= _speedBoost;
        _isSpeedBoostActive = false;
    }

    public void ShieldActive()
    {
        Debug.Log("Player:ShieldActive()");
        _isShieldActive = true;
        _shieldVisualizer[_shieldHitCount].SetActive(true);
        //StartCoroutine(ShieldPowerDownRoutine());
    }

    IEnumerator ShieldPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isShieldActive = false;
        DisableShield();
    }

    private void DisableShield()
    {
        for (int i = 0; i < 3; i++)
        {
            _shieldVisualizer[i].SetActive(false);
        }
    }

    public void MagicFlameBombActive()
    {
        _isMagicFlameBombActive = true;
        StartCoroutine(MagicFlameBombPowerDownRoutine());
    }

    IEnumerator MagicFlameBombPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isMagicFlameBombActive = false;
    }

    public void UpdateScore(int points)
    {
        score += points;
        _uiManager.UpdateScoreText(score);
    }

    public void AmmoReload()
    {
        _shotCount = 15;
        _uiManager.UpdateShotsText(_shotCount);
    }

    public void HealthBoost()
    {
        if (_lives < _maxLives)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
            if (_leftEngineFire.activeSelf)
            {
                _leftEngineFire.SetActive(false);
            }
            else if (_rightEngineFire.activeSelf)
            {
                _rightEngineFire.SetActive(false);
            }
        }
    }
}
