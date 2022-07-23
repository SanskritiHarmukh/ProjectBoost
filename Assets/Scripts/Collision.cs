using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collision : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip explosion;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] ParticleSystem successParticles;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    bool isTransitioning = false;
    bool disableCollision = false;

    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (isTransitioning || disableCollision)
            return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("This thing is Friendly");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "Fuel":
                Debug.Log("You picked up fuel!");
                break;
            default:
                StartCrashSequence();  
                break;
        }
    }

    private void Update()
    {
        RespondToDebugKeys();
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKey(KeyCode.C))
        {
            disableCollision = !disableCollision;   //toggle Collision
        }
    }

    void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    // We'll implement a crash sequence
    void StartCrashSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(explosion);
        explosionParticles.Play();
        // Disabling movement script 
        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel",levelLoadDelay);   // producing a time delay so player knows how they reverted back
    }

    // If rocket smashes ground or obstacle it refreshes its position back to launching pad
    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // Teleport
    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if( nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
}
