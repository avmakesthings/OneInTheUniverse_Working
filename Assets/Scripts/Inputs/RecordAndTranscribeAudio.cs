// ---- A class to record and transcirbe audio from a microphone. 
// 1 -- The transcription functionality only works with Windows 10
// 2 -- Microphone access needs to be grated through Windows privacy settings

using UnityEngine;
using UnityEngine.Windows.Speech;

public delegate void OnTextReceived(string dictation);

public static class RecordAndTranscribeAudio
{
    static AudioClip recording;

    private static DictationRecognizer _d;
    private static string microphoneName = "Microphone (Realtek(R) Audio)";

    // TODO: Kind of weird setting these static properties.
    // Would be better to make this class not-static, 
    // or somehow curry these values in the ensuing function calls
    private static string curRecordingFileName;
    private static OnTextReceived curOnTextReceived;

    public static void StartSpeechToText(string recordingFileName, OnTextReceived onTextReceived)
    {
        curRecordingFileName = recordingFileName;
        curOnTextReceived = onTextReceived;
        // TODO: Can we pass a longer duration? Will it cut things off at the right time? 
        recording = Microphone.Start(microphoneName, true, 10, 44100);

        _d = new DictationRecognizer();
        _d.Start();
        _d.DictationResult += HandleText;
    }

    public static void StopSpeechToText()
    {
        Microphone.End(microphoneName);
        AudioClip trimmedRecording = SavWav.TrimSilence(recording, 0.0f);
        
        _d.DictationResult -= HandleText;
        _d.Stop();
        _d.Dispose();
        SavWav.Save(curRecordingFileName, trimmedRecording);
    }

    public static void HandleText(string text, ConfidenceLevel confidence)
    {
        Debug.LogFormat("Dictation result: {0}", text);
        curOnTextReceived(text);
    }
}
