using System.Collections;
using System.Data.Common;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState {
        Tutorial,
        Room1,
        Room2
    };

    public GameState state;
    public int progress;

    // tutorial flags
    private bool moved;
    private bool sprinted;
    private bool aimed;
    private bool fired;
    private bool reloaded;
    private bool survived;
    private bool survivedAgain;

    // ui elements
    [SerializeField] private GameObject taskText;
    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject subtitle;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text moveText;
    [SerializeField] private TMP_Text sprintText;
    [SerializeField] private TMP_Text aimText;
    [SerializeField] private TMP_Text fireText;
    [SerializeField] private TMP_Text reloadText;
    [SerializeField] private TMP_Text surviveText;

    // spawner
    [SerializeField] private GameObject enemySpawner;
    private EnemySpawner spawner;

    // doors
    [SerializeField] private GameObject room1Door1;
    [SerializeField] private GameObject room1Door2;
    [SerializeField] private GameObject room1Door3;
    [SerializeField] private GameObject room1Door4;
    [SerializeField] private GameObject room2Door1;
    [SerializeField] private GameObject room2Door2;

    // player
    [SerializeField] private GameObject playerArmature;

    // audio
    [SerializeField] private AudioSource music;

    // check input cuz i cba to do it properly
    public void Update()
    {
        // tutorial handling
        if ( state == GameState.Tutorial ) {
            if ( progress == 0 ) {
                movementTutorial(); return;
            }
            if ( progress == 1 ) {
                sprintTutorial(); return;
            }
            if ( progress == 2 ) {
                aimTutorial(); return;
            }
            if ( progress == 3 ) {
                shootTutorial(); return;
            }
            if ( progress == 4 ) {
                reloadTutorial(); return;
            }
            if ( progress == 5 ) {
                surviveTutorial(); return;
            }
        }

        // check proximity to door [1]
        if ( !survivedAgain && Vector3.Distance(playerArmature.transform.position, room1Door1.transform.position) < 10 ) {
            room1Door1.transform.rotation = Quaternion.Lerp(room1Door1.transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime * 4f);
            room1Door2.transform.rotation = Quaternion.Lerp(room1Door2.transform.rotation, Quaternion.Euler(0, -90, 0), Time.deltaTime * 4f);
            room1Door1.layer = 0;
            room1Door2.layer = 0;
        }
        if ( !survivedAgain && Vector3.Distance(playerArmature.transform.position, room1Door3.transform.position) < 10 ) {
            room1Door3.transform.rotation = Quaternion.Lerp(room1Door3.transform.rotation, Quaternion.Euler(0, 270, 0), Time.deltaTime * 4f);
            room1Door4.transform.rotation = Quaternion.Lerp(room1Door4.transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime * 4f);
            return;
        }
        if ( !survivedAgain ) {
            surviveRoom1();
            return;
        }

        // check proximity to door [2]
        if ( Vector3.Distance(playerArmature.transform.position, room2Door1.transform.position) < 10 ) {
            room2Door1.transform.rotation = Quaternion.Lerp(room2Door1.transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime * 4f);
            room2Door2.transform.rotation = Quaternion.Lerp(room2Door2.transform.rotation, Quaternion.Euler(0, 270, 0), Time.deltaTime * 4f);
            room2Door1.layer = 0;
            room2Door2.layer = 0;
        }
    }

    public void Start()
    {
        state = GameState.Tutorial;
        progress = 0;
        spawner = enemySpawner.GetComponent<EnemySpawner>();

        StartCoroutine(handleState());
    }

    public IEnumerator handleState() 
    {
        // tutorial
        yield return new WaitUntil(() => moved);
        moveText.fontStyle = FontStyles.Strikethrough;
        sprintText.alpha = (float)(200.0 / 255.0);
        tutorial.GetComponent<AudioSource>().Play();
        taskText.SetActive(false);
        progress++;
        yield return new WaitUntil(() => sprinted); 
        sprintText.fontStyle = FontStyles.Strikethrough;
        aimText.alpha = (float)(200.0 / 255.0);
        tutorial.GetComponent<AudioSource>().Play();
        progress++;
        yield return new WaitUntil(() => aimed);
        aimText.fontStyle = FontStyles.Strikethrough;
        fireText.alpha = (float)(200.0 / 255.0);
        tutorial.GetComponent<AudioSource>().Play();
        progress++;
        yield return new WaitUntil(() => fired); 
        fireText.fontStyle = FontStyles.Strikethrough;
        reloadText.alpha = (float)(200.0 / 255.0);
        tutorial.GetComponent<AudioSource>().Play();
        progress++;
        yield return new WaitUntil(() => reloaded); 
        reloadText.fontStyle = FontStyles.Strikethrough;
        surviveText.alpha = 1;
        tutorial.GetComponent<AudioSource>().Play();
        progress++;
        music.Play();
        spawner.TurnSpawnerOn();
        yield return new WaitUntil(() => survived);
        tutorial.SetActive(false);
        subtitle.SetActive(true);
        waveText.alpha = (float)(200.0 / 255.0);
        waveText.GetComponent<AudioSource>().Play();

        // door xray
        room1Door1.layer = 6;
        room1Door2.layer = 6;

        // room 1
        progress = 0;
        state = GameState.Room1;

        // caption
        yield return new WaitUntil(() => room1Door1.layer == 0);
        subtitle.SetActive(false);
        yield return new WaitForSeconds(1f);
        subtitleText.text = "the first wave will commence";
        subtitle.SetActive(true);
        yield return new WaitForSeconds(3f);
        subtitle.SetActive(false);

        yield return new WaitUntil(() => survivedAgain);
        waveText.GetComponent<AudioSource>().Play();

        // door xray
        room2Door1.layer = 6;
        room2Door2.layer = 6;

        // room 2
    }

    // input checks for tutorial
    public void movementTutorial()
    {
        moved = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D);
    }
    public void sprintTutorial()
    {
        sprinted = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && Input.GetKeyDown(KeyCode.LeftShift);
    }
    public void aimTutorial()
    {
        aimed = Input.GetKeyDown(KeyCode.Mouse1);
    }
    public void shootTutorial()
    {
        fired = Input.GetKey(KeyCode.Mouse1) && Input.GetKeyDown(KeyCode.Mouse0);
    }
    public void reloadTutorial()
    {
        reloaded = Input.GetKeyDown(KeyCode.R);
    }
    public void surviveTutorial()
    {
        survived = spawner.GetCurrentWave() > 0;
    }

    // input checks for room 1
    public void surviveRoom1()
    {
        survivedAgain = spawner.GetCurrentWave() > 2;
    }
}
