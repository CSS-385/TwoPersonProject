using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 8.0f;
    private float localPositionY = 0.0f;

    private AudioSource _audioSource = null;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        localPositionY = transform.localPosition.y;
    }

    void Update()
    {
        float posY = transform.localPosition.y;
        posY -= speed * Time.deltaTime;
        if (posY < localPositionY - 1.0f)
        {
            posY = localPositionY - (posY % 1.0f);
        }
        transform.localPosition = new Vector3(transform.localPosition.x, posY, transform.localPosition.z);

        if (Time.timeScale == 0.0f)
        {
            _audioSource.mute = true;
        }
        else
        {
            _audioSource.mute = false;
        }
    }
}
