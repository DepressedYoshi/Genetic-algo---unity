/*
TODO: 
- multi screen simulation 
- user input absefd agent count 
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class GameManager : MonoBehaviour
{
    public GameObject playerObject;
    public int agentCount;
    private List<GameObject> playerPool = new List<GameObject>(); // Pool for player objects
    private int generationCount = 1;
    public TextMeshProUGUI  genCounterTextr; // UI shows generation number 
    public Button pauseButton;
    public Image buttonIcon;
    public Sprite pauseSprite;
    public Sprite playSprite;
    private bool isPaused = false;

    void Start()
    {
     InitializePool();
     createPlayers();   
     Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
     if (genCounterTextr != null){
            genCounterTextr.text = "GEN: " + generationCount;
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < agentCount; i++){
            GameObject player = Instantiate(playerObject);
            player.GetComponent<PlayerScript>().SetGameManager(this);
            playerPool.Add(player);
        }
    }

    private void createPlayers()
    {
        for (int i = 0; i < agentCount; i++){
            GameObject p = GetPlayerFromPool(i);
            if (p != null){
                p.transform.position = transform.position;
                p.transform.rotation = transform.rotation;
                p.SetActive(true);
            }
        }
    }

    // Gets a player object from the pool
    private GameObject GetPlayerFromPool( int index)
    {
        if (playerPool.Count > 0)
        {
            return playerPool[index];
        }

        Debug.LogWarning("Player pool is empty!");
        return null;
    }

    // Returns a player object to the pool
    public void ReturnPlayerToPool(GameObject player)
    {
        player.SetActive(false); // Deactivate the player object
        playerPool.Add(player); // Add it back to the pool
    }

    public List<GameObject> getPlayerPool(){
        return playerPool;
    }

    public void alertPlayerCompletion(){
        Debug.Log("Gen " + generationCount + " has comepleted");
        generationCount++;
        if (genCounterTextr != null){
            genCounterTextr.text = "GEN: " + generationCount;
        }
        OnGenComplete();
    }

    private void OnGenComplete()
    {
        DeactivateAll(playerPool);
        Selection(playerPool);
        Reproduce(playerPool);
        RespawnPlayer(playerPool);
    }

    private void RespawnPlayer(List<GameObject> playerPool)
    {
        for (int i = 0; i < playerPool.Count; i++){
            GameObject player = playerPool[i];
            player.GetComponent<PlayerScript>().Reset();
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
            player.SetActive(true);
        }
    }

    private void DeactivateAll(List<GameObject> playerPool)
    {
        foreach (GameObject p in playerPool)
        {
            p.SetActive(false);
        }
    }

    private void Selection(List<GameObject> playerPool)
    {
        if (playerPool.Count != agentCount){
            Debug.LogWarning("SOMETHING WENT WRONG - the amount of starting player do not match the ones in slection process");
        }
        playerPool.Sort((a, b) =>b.GetComponent<PlayerScript>().getScore().CompareTo(a.GetComponent<PlayerScript>().getScore()));


        int asian = agentCount/5;
        for (int i = playerPool.Count-1; i >= asian; i--){
            GameObject p = playerPool[i];
            playerPool.Remove(p);
            Destroy(p);
        }
    
    }
    private void Reproduce(List<GameObject> playerPool)
    {
        int elitIndex = playerPool.Count/4;
        if(elitIndex < 1){
            elitIndex = 1;
        }

        List<GameObject> bestGene = playerPool.GetRange(0, elitIndex);

        while (playerPool.Count < agentCount){

            int[] momDNA = bestGene[UnityEngine.Random.Range(0,elitIndex)].GetComponent<PlayerScript>().getDNA();
            int[] dadDNA = bestGene[UnityEngine.Random.Range(0,elitIndex)].GetComponent<PlayerScript>().getDNA();
            GameObject john = Instantiate(playerObject); 
            john.GetComponent<PlayerScript>().createDNA(momDNA, dadDNA);
            john.GetComponent<PlayerScript>().SetGameManager(this);
            playerPool.Add(john);
        }
        
    }
    public void TogglePause(){
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pauseButton != null){
            buttonIcon.sprite = isPaused ? pauseSprite : playSprite;
        }
    }

    void Update(){
        handleKeyInpute();
    }

    private void handleKeyInpute()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            TogglePause();
        }
    }
}