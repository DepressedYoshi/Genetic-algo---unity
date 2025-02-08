using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // UI References
    public InputField agentCountInput; // Drag the InputField from the Inspector
    public GameObject welcomePanel;   // Drag the welcome screen panel
    public GameObject inputPanel;     // Drag the input panel for agent count

    // Static variable to store the agent count for the simulation
    public static int agentCount = 50; // Default agent count

    void Start()
    {
        // Ensure only the welcome panel is active at the start
        welcomePanel.SetActive(true);
        inputPanel.SetActive(false);
    }

    // Called when the "Start Simulation" button is clicked
    public void ShowInputPanel()
    {
        welcomePanel.SetActive(false);
        inputPanel.SetActive(true);
    }

    // Called when the "Submit" button is clicked
    public void StartSimulation()
    {
        if (agentCountInput != null && !string.IsNullOrEmpty(agentCountInput.text))
        {
            // Validate and parse input
            if (int.TryParse(agentCountInput.text, out int count))
            {
                agentCount = Mathf.Max(1, count); // Ensure at least 1 agent
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                Debug.LogWarning("Invalid input! Using default agent count.");
            }
        }
        else
        {
            Debug.LogWarning("Input field is empty! Using default agent count.");
        }

        // Load the simulation scene
        
    }
}
