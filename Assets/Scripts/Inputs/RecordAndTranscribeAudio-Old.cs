// ---- A class to record and transcirbe audio from a microphone. 
// 1 -- The transcription functionality only works with Windows 10
// 2 -- Microphone access needs to be grated through Windows privacy settings



using UnityEngine;
using UnityEngine.Windows.Speech;

public static class RecordAndTranscribeAudioOld
{
    static AudioClip recording;
    static string dictation;
    private static DictationRecognizer _d = new DictationRecognizer(); 


    public static void StartRecording(string microphoneName)
    {
        //TODO - this needs to be modified to record over a variable number of seconds
        recording = Microphone.Start(microphoneName, true, 10, 44100);
        SpeechToText();
    }

    public static void StopRecording(string microphoneName)
    {
        Debug.LogFormat("Saved dictation result is: {0}", dictation);
        Microphone.End(microphoneName);
        _d.Stop();
        _d.Dispose();
    }

    static string SpeechToText()
    {

        _d.Start();
        _d.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            dictation = text;
            // call stop

        };
        return dictation;
    }

    public static void WriteAudioToFile(string filename)
    {
        SavWav.Save(filename, recording);
    }
}
