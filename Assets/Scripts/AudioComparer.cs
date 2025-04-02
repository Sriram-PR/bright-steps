using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AudioComparer : MonoBehaviour
{
    public VoiceRecorder voiceRecorder;
    public AudioSource targetAudio;
    public TMP_Text resultText;
    public Leaderboard leaderboard;

    public void CompareAudio()
    {
        AudioClip recordedClip = voiceRecorder.GetRecordedClip();

        if (recordedClip == null || targetAudio.clip == null)
        {
            Debug.LogError("AudioSource or Word AudioClip is missing!");
            return;
        }

        float matchPercentage = CalculateMatch(recordedClip, targetAudio.clip);
        resultText.text = "Match: " + matchPercentage.ToString("F2") + "%";

        leaderboard.UpdateScore(matchPercentage);
    }

    private float CalculateMatch(AudioClip recorded, AudioClip target)
    {
        return Random.Range(50f, 100f); // Placeholder logic, replace with actual ML-based audio matching
    }
}
