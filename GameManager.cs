/*
TODO: 
- multi screen simulation 
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
    public TextMeshProUGUI agentCountText;
    public Button pauseButton;
    public Image buttonIcon;
    public Sprite pauseSprite;
    public Sprite playSprite;
    private bool isPaused = false;

    void Start()
    {
        agentCount = MenuManager.agentCount; // Use the agent count set in the menu

     InitializePool();
     createPlayers();   
     Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
     if (genCounterTextr != null){
            genCounterTextr.text = "GEN: " + generationCount;
        }
        if (agentCountText != null){
            agentCountText.text = "Population: " + agentCount;
        }else{
            Debug.Log("cant find the agent count ");
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

    private void SortByScore(List<GameObject> playerpool){

        if (playerPool.Count != agentCount){
            Debug.LogWarning("SOMETHING WENT WRONG - the amount of starting player do not match the ones in slection process");
        }
        playerPool.Sort((a, b) =>b.GetComponent<PlayerScript>().getScore().CompareTo(a.GetComponent<PlayerScript>().getScore()));
    }
    private void Selection(List<GameObject> playerPool)
    {
        SortByScore(playerPool);    
        List<float> scores = new List<float>();
        foreach (GameObject p in playerPool){
            scores.Add(p.GetComponent<PlayerScript>().getScore());
        }

         // assume a normal distribtuion of score 
        float mean = CalculateMean(scores);
        float stdDev = CalculateStandardDeviation(scores, mean);
        float statisticalThreshold = mean + stdDev;

        // if the score is not normal distribution - when all player converged to 
        scores.Sort();
        int percentileIndex = Mathf.FloorToInt(scores.Count * 0.7f); // Top 30%
        float percentileThreshold = scores[percentileIndex];

        //Choose the one with stringer selection pressure 
        float finalThreshold = Mathf.Max(statisticalThreshold, percentileThreshold);
        Debug.Log($"Selection Threshold: {finalThreshold} (Mean: {mean}, StdDev: {stdDev}, 30th Percentile: {percentileThreshold})");

        for (int i = playerPool.Count-1; i >= 0; i--){
            float score = playerPool[i].GetComponent<PlayerScript>().getScore();
            if (score < finalThreshold){
                GameObject p = playerPool[i];
                playerPool.Remove(p);
                Destroy(p);
            }else{
                break;
            }
            
        }
    
    }
    private void Reproduce(List<GameObject> playerPool)
    {
        int elitIndex = playerPool.Count;
        if(elitIndex < 1){
            Debug.Log("something went really wrong");
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

    private float CalculateMean(List<float> scores)
    {
        float sum = 0f;
        foreach (float score in scores)
        {
            sum += score;
        }
        return sum / scores.Count;
    }

    private float CalculateStandardDeviation(List<float> scores, float mean)
    {
        float sumSquaredDiffs = 0f;
        foreach (float score in scores)
        {
            sumSquaredDiffs += Mathf.Pow(score - mean, 2);
        }
        return Mathf.Sqrt(sumSquaredDiffs / scores.Count);
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