using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StarterAssets;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    //Spawner status
    private bool spawnerOn = false;

    //Zombie & spawner arrays
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform[] spawners1;
    [SerializeField] private Transform[] spawners2;
    [SerializeField] private Transform[] spawners3;
    [SerializeField] private Transform[] spawners4;

    //Zombie stats list
    [SerializeField] private List<CharacterStats> zombieList;

    //Waves
    [SerializeField] private Wave[] waves;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private float waveCountDown = 0f;
    private int currentWave;

    [SerializeField] private TMP_Text waveText;

    //States
    public enum SpawnState { SPAWNING, WAITING, COUNTING }
    private SpawnState state = SpawnState.COUNTING;

    private void Start() {
        waveCountDown = timeBetweenWaves;
        currentWave = 0;
    }

    private void Update() {
        if (spawnerOn) {
            //Checks if the spawner is in the waiting state
            if (state == SpawnState.WAITING)
            {
                //Checks if the player has killed all zombies
                //If so, it completes the wave
                if (!ZombiesAreDead()) { return; }
                else { CompleteWave(); }
            }

            //Checks if the wave start count down has finished
            if (waveCountDown <= 0)
            {
                //Checks if the spawner is not in the spawning state
                //If so, it spawns a wave
                if (state != SpawnState.SPAWNING) { StartCoroutine(SpawnWave(waves[currentWave])); }
            }
            //If not, decreases wave start count down
            else { waveCountDown -= Time.deltaTime; }
        }
    }

    /*
     * Activates spawner
     */
    public void TurnSpawnerOn() {
        spawnerOn = true;
    }

    /*
     * Spawns a wave  
     */
    private IEnumerator SpawnWave(Wave wave) {
        //Sets the spawn state to spawning
        state = SpawnState.SPAWNING;

        //Spawns zombies
        for (int i = 0; i < wave.enemiesAmount; i++) {
            int randomInt = Random.RandomRange(0, enemies.Length);
            SpawnZombie(enemies[randomInt]);
            yield return new WaitForSeconds(wave.delay);
        }

        //Sets the spawn state as waiting
        state = SpawnState.WAITING;
        yield break;
    }

    /*
     * Spawns a zombie at a random spawner
     */
    private void SpawnZombie(GameObject zombie) {
        //Chooses set of spawners
        Transform[] spawners;
        if (currentWave == 0) { spawners = spawners1;}
        else if (0 < currentWave && currentWave < 3) { spawners = spawners2;}
        else if (2 < currentWave && currentWave < 5) { spawners = spawners3;}
        else { spawners = spawners4; }

        //Chooses a random spawner
        int randomInt = Random.RandomRange(0, spawners.Length);
        Transform randomSpawner = spawners[randomInt];

        //Spawns a zombie at the spawner
        GameObject newZombie = Instantiate(zombie, randomSpawner.position, randomSpawner.rotation);
        CharacterStats newZombieStats = newZombie.GetComponent<CharacterStats>();

        //Adds the zombie's stats to the zombie stats list
        zombieList.Add(newZombieStats);
    }

    /*
     * Checks if all zombies are dead or not
     */
    private bool ZombiesAreDead() {
        int i = 0;
        foreach(CharacterStats zombie in zombieList) {
            if (zombie.IsDead()) {i++;} 
            else {return false;}
        }
        return true;
    }

    /*
     * 
     */
    private void CompleteWave() {
        state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        // generate new wave if all waves cleared
        if (waves.Length-1 < currentWave+1) {
            Wave wave = new Wave();
            wave.name = "Wave" + (currentWave + 1);
            wave.enemiesAmount = 12 + (8 * (currentWave));
            wave.delay = 1;
            
            waves = waves.Concat(new Wave[] {wave}).ToArray();
        }
        currentWave++;

        // signal to player that new wave is starting
        waveText.text = "Wave " + currentWave;
    }

    /*
     * Gets the current wave number
     */
    public int GetCurrentWave() {
        return currentWave;
    }
}
