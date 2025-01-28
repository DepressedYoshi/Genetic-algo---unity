using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public GameObject playerObject;
    public int agentCount;
    private List<GameObject> playerPool = new List<GameObject>(); // Pool for player objects
    private int generationCount = 1;

    void Start()
    {
     InitializePool();
     createPlayers();   
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
        OnGenComplete();
    }

    private void OnGenComplete()
    {
        DeactivateAll(playerPool);
        Selection(playerPool);
        Reproduce();
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


        int asian = agentCount/2;
        for (int i = playerPool.Count-1; i >= asian; i--){
            GameObject p = playerPool[i];
            playerPool.Remove(p);
            Destroy(p);
        }
    
    }
    private void Reproduce()
    {
        while (playerPool.Count < agentCount){
            int[] momDNA = playerPool[Random.Range(0,playerPool.Count)].GetComponent<PlayerScript>().getDNA();
            int[] dadDNA = playerPool[Random.Range(0,playerPool.Count)].GetComponent<PlayerScript>().getDNA();
            GameObject john = Instantiate(playerObject); 
            john.GetComponent<PlayerScript>().createDNA(momDNA, dadDNA);
            john.GetComponent<PlayerScript>().SetGameManager(this);
            playerPool.Add(john);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
    }
}