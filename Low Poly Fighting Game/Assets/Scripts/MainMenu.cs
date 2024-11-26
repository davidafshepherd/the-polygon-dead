using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 0.5f;

    // starts the game
    public void playGame()
    {
        StartCoroutine(loadGame(SceneManager.GetActiveScene().buildIndex + 1));
    }

    // loads the game scene
    public IEnumerator loadGame(int scene)
    {
        transition.SetTrigger("start");
        yield return new WaitForSeconds(transitionTime);
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(scene);
    }

    // exits the game
    public void exitGame()
    {
        Application.Quit();
    }
}
