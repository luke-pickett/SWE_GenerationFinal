using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class HighScoreUploader : MonoBehaviour
{
    [Header("Server Configuration")]
    [SerializeField] private string phpUploadURL = "https://yourdomain.com/upload_score.php";
    [SerializeField] private string phpLeaderboardURL = "https://yourdomain.com/get_leaderboard.php";

    [Header("UI Feedback")]
    [SerializeField] private UnityEngine.UI.Button uploadButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private bool isUploading = false;

    private void Start()
    {
        if (uploadButton != null)
        {
            uploadButton.onClick.AddListener(OnUploadButtonClicked);
        }
    }

    public void OnUploadButtonClicked()
    {
        if (isUploading)
        {
            UpdateStatus("Already uploading...");
            return;
        }

        if (RoundManager.Instance == null || GameOverManager.Instance == null)
        {
            UpdateStatus("Error: Game managers not found!");
            return;
        }

        int finalScore = RoundManager.Instance.GetCurrentScore();
        string playerName = GetPlayerName();

        StartCoroutine(UploadScore(playerName, finalScore));
    }

    private IEnumerator UploadScore(string playerName, int score)
    {
        isUploading = true;
        UpdateStatus("Uploading...");

        // Create form data
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName);
        form.AddField("score", score);
        form.AddField("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        // Send request
        using (UnityWebRequest www = UnityWebRequest.Post(phpUploadURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Server Response: {response}");

                if (response.Contains("success"))
                {
                    UpdateStatus("Score uploaded successfully!");
                }
                else
                {
                    UpdateStatus($"Upload failed: {response}");
                }
            }
            else
            {
                Debug.LogError($"Upload Error: {www.error}");
                UpdateStatus($"Upload failed: {www.error}");
            }
        }

        isUploading = false;

        // Re-enable button after 3 seconds
        if (uploadButton != null)
        {
            uploadButton.interactable = false;
            yield return new WaitForSeconds(3f);
            uploadButton.interactable = true;
        }
    }

    public IEnumerator FetchLeaderboard(Action<string> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(phpLeaderboardURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Leaderboard fetch error: {www.error}");
                callback?.Invoke(null);
            }
        }
    }

    private string GetPlayerName()
    {
        // Try to get saved name, otherwise use "Anonymous"
        string savedName = PlayerPrefs.GetString("PlayerName", "Anonymous");
        return savedName;
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log(message);
    }

    // Public method to set player name
    public void SetPlayerName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            PlayerPrefs.SetString("PlayerName", name);
            PlayerPrefs.Save();
            Debug.Log($"Player name set to: {name}");
        }
    }
}
