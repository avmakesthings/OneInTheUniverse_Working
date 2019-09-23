// ---- A class to record and transcirbe audio from a microphone. 
// 1 -- The transcription functionality only works with Windows 10
// 2 -- Microphone access needs to be grated through Windows privacy settings

using System;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

public delegate void OnTextReceived(string dictation);

public static class RecordAndTranscribeAudio
{
    static AudioClip recording;

    private static string concatString;
    private static DictationRecognizer _d;
    private static string microphoneName = "Microphone (Realtek(R) Audio)";
    private static string curRecordingFileName;
    private static OnTextReceived curOnTextReceived;

    public static void StartSpeechToText(string recordingFileName, OnTextReceived onTextReceived)
    {
        curRecordingFileName = recordingFileName;
        curOnTextReceived = onTextReceived;
        concatString = "";
        recording = Microphone.Start(microphoneName, true, 30, 44100);
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
        Debug.LogFormat("HandleText: {0}", text);
        concatString += " " + text;
        curOnTextReceived(concatString);
    }
}


public static class AnalyzeKeyword {

    static string[] keywords = new string [4] {"helpful", "alone", "together", "testing" };
    private static KeywordRecognizer _k = new KeywordRecognizer(keywords);

    public static void StartKeywordAnalysis()
    {
        _k.OnPhraseRecognized += OnPhraseRecognized;
        _k.Start();
    }

    public static void StopKeywordAnalysis()
    {
        _k.OnPhraseRecognized -= OnPhraseRecognized;
        _k.Stop();
        _k.Dispose();
    }

    private static void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(builder.ToString());
    }

}

