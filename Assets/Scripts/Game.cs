using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Transform playerTransform = null;

    public Canvas gameCanvas = null;
    public Button restartButton = null;
    private AudioSource _audioSource = null;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        gameCanvas.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void OnEnable()
    {
    }

    private void FixedUpdate()
    {
        if (playerTransform.position.y < -25.0f)
        {
            Debug.Log("game over");
            gameCanvas.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
            _audioSource.Play();
        }
    }

    private void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
