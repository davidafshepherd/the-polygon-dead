using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private bool wasTutorial = false;
    private bool shouldFollow = false;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject subtitle;

    // animation
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 0.5f;

    [SerializeField] private Transform player;
    private ThirdPersonController thirdPersonController;

    public void Awake()
    {
        thirdPersonController = player.GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Escape) ) {
            if ( isPaused ) resume();
            else pause();
        }
    }

    public void pause()
    {
        HUD.SetActive(false);
        crosshair.SetActive(false);

        wasTutorial = tutorial.activeSelf;
        tutorial.SetActive(false);

        shouldFollow = subtitle.activeSelf;
        subtitle.SetActive(false);

        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;

        thirdPersonController.SetCamera(false);
    }

    public void resume()
    {
        thirdPersonController.SetCamera(true);
        pauseMenu.SetActive(false);
        HUD.SetActive(true);
        tutorial.SetActive(wasTutorial);
        subtitle.SetActive(shouldFollow);
        crosshair.SetActive(true);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }

    public void returnToMenu()
    {
        StartCoroutine(loadMenu(0));
    }

    public IEnumerator loadMenu(int scene)
    {
        Time.timeScale = 1f;
        transition.SetTrigger("start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(scene);
    }
}
