using UnityEngine;

public class VoiceRecorder : MonoBehaviour
{
    private bool isRecording = false;
    private AudioClip recordedClip;

    public void StartRecording()
    {
        if (!isRecording)
        {
            isRecording = true;
            recordedClip = Microphone.Start(null, false, 5, 44100);
            Debug.Log("Recording Started");
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            Microphone.End(null);
            Debug.Log("Recording Stopped");

            if (recordedClip != null)
            {
                ProcessRecording();
            }
        }
    }

    public AudioClip GetRecordedClip()
    {
        return recordedClip;
    }

    private void ProcessRecording()
    {
        if (recordedClip == null)
        {
            Debug.LogError("No recorded clip available!");
            return;
        }
        Debug.Log("Processing Recorded Audio...");
    }
}
