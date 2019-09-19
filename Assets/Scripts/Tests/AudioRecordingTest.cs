//Test class for testing audio recording and saving to file

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRecordingTest : MonoBehaviour
{

    public string micName = "Microphone (Realtek(R) Audio)";
    public string filepath = "Recordings/TestAudioFile";

    private int counter = 0;

    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Debug.Log("Recording audio");
            RecordAndTranscribeAudio.StartRecording(micName);
        }

        if (Input.GetKeyDown("o"))
        {
            Debug.Log("stopping recording");
            RecordAndTranscribeAudio.StopRecording(micName);
            RecordAndTranscribeAudio.WriteAudioToFile(string.Format("{0}_{1}",filepath,counter));
            counter++;
        }
    }
}
