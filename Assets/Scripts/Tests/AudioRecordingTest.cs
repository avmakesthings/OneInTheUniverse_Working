//Test class for testing audio recording and saving to file

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRecordingTest : MonoBehaviour
{

    public string micName = "Microphone (Realtek(R) Audio)";
    public string filepath = "Recordings/TestAudioFile";

    private bool isRecording = false;
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
        if (Input.GetKeyDown("p") && !isRecording)
        {
            isRecording = true;
            Debug.Log("Recording audio");
            RecordAndTranscribeAudio.StartSpeechToText(string.Format("{0}_{1}", filepath, counter), (string text) => {
                Debug.LogFormat("In the Delegate: {0}", text);
            }) ;
            AnalyzeKeyword.StartKeywordAnalysis();
        }

        if (Input.GetKeyDown("o") && isRecording)
        {
            Debug.Log("stopping recording");

            RecordAndTranscribeAudio.StopSpeechToText();
            AnalyzeKeyword.StopKeywordAnalysis();
            counter++;
            isRecording = false;
        }
    }
}
