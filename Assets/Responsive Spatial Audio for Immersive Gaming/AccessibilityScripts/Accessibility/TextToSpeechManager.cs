using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System;
public enum TTSVoice
{
	DAVID,
	ZIRA
}

public class AudioClipWrapper
{
	public AudioClip clip;

	public AudioClipWrapper(AudioClip clip)
	{
		this.clip = clip;
	}
}

public enum PromptRate
{
    ExtraFast = 0,
    Fast = 1,
    Medium = 2,
    Slow = 3,
    ExtraSlow = 4
}

public struct TTSOptions
{
	public string speechText;
    public string voiceName;
    public string rate;


    public TTSOptions(string speechText,string voiceName,string rate)
	{
		this.speechText = speechText;
        this.voiceName = voiceName;
        this.rate = rate;
    }

    public static Dictionary<TTSVoice, string> voiceNameDict = new Dictionary<TTSVoice, string>
    {
        { TTSVoice.DAVID,"Microsoft David Desktop" },
        { TTSVoice.ZIRA,"Microsoft Zira Desktop" }

    };

}

/// <summary>
/// Class for interfacing with windows TTS engine and Unity, text is sent to windows TTS and gets a WAV which is converted to a temporary Unity audioclip for playback.
/// </summary>
public static class TextToSpeechManager {

	static Dictionary<TTSOptions, AudioClip> cache = new Dictionary<TTSOptions, AudioClip>();

	public static AudioClip GetClipAndCache(string speechText, PromptRate rate=PromptRate.Medium, Action<AudioClip> callback = null, TTSVoice voice = TTSVoice.DAVID)
	{
        if (!TTSOptions.voiceNameDict.ContainsKey(voice))
        {
            throw new System.ArgumentException("TextToSpeechManager: Invalid voice name");
        }

		return GetClipAndCache(new TTSOptions(speechText, TTSOptions.voiceNameDict[voice], rate.ToString()), callback);
	}

	public static AudioClip GetClipAndCache(TTSOptions options, Action<AudioClip> callback = null)
	{
       //UnityEngine.Debug.Log(JsonUtility.ToJson(options));
       if (cache.ContainsKey(options)) {
            //UnityEngine.Debug.Log("from cache");
            if (callback != null)
            {
                //UnityEngine.Debug.Log("got from cache " + options.speechText);
                callback.Invoke(cache[options]);
            }
			return cache[options];
		}
		else
		{
			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = Application.dataPath + @"/Responsive Spatial Audio for Immersive Gaming/ExternalCmd/VoiceGeneratorCLI.exe"; // Specify exe name.
			start.Arguments = "\"" + options.speechText + "\" \"" + options.voiceName + "\" \"" + options.rate + "\"";
			         start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			start.RedirectStandardError = true;
			start.CreateNoWindow = true;
			string res;
			try
			{
				using (Process process = Process.Start(start))
				{
					using (StreamReader reader = process.StandardOutput)
					{
						// Unable to correctly read bytes from pipe; Routing through file for now
						//res = reader.ReadToEnd(); 	
					}
					using (StreamReader reader = process.StandardError)
					{
						res = reader.ReadToEnd();
						if (res.Length > 0)
						{
							UnityEngine.Debug.LogError("External Text to Speech Engine: " + res);
						}
					}
				}
			}
			catch (Exception e)
			{
                UnityEngine.Debug.LogError(e.Message);
			}
			var bytes = File.ReadAllBytes(Application.dataPath + "/../output.wav");
			File.Delete(Application.dataPath + "/../output.wav");
			var clip = WAVtoUnityClip(bytes);
			cache[options] = clip;
            
            if(callback != null) {
				callback.Invoke(clip);
			}
			return clip;
		}
	}


    public static void ClearCache()
    {
        foreach (TTSOptions t in cache.Keys)
        {
            UnityEngine.Debug.Log(t.speechText);
        }
        cache.Clear();
        UnityEngine.Debug.Log("***********");
        foreach (TTSOptions t in cache.Keys)
        {
            UnityEngine.Debug.Log(t.speechText);
        }
    }
	static private AudioClip WAVtoUnityClip(byte[] bytes)
	{
		// Convert raw WAV data into Unity audio data
		int sampleCount = 0;
		int frequency = 0;
		var unityData = ToUnityAudio(bytes, out sampleCount, out frequency);

		// Convert to an audio clip
		var name = "generated_clip_" + bytes.Length.GetHashCode().ToString() + UnityEngine.Random.value.ToString();
		var clip = ToClip(name, unityData, sampleCount, frequency);
		return clip;
	}

	static private AudioClip ToClip(string name, float[] audioData, int sampleCount, int frequency)
	{
		// Create the audio clip
		var clip = AudioClip.Create(name, sampleCount, 1, frequency, false);

		// Set the data
		clip.SetData(audioData, 0);

		// Done
		return clip;
	}

	static private float[] ToUnityAudio(byte[] wavAudio, out int sampleCount, out int frequency)
	{
		// Determine if mono or stereo
		int channelCount = wavAudio[22];     // Speech audio data is always mono but read actual header value for processing

		// Get the frequency
		frequency = BytesToInt(wavAudio, 24);

		// Get past all the other sub chunks to get to the data subchunk:
		int pos = 12;   // First subchunk ID from 12 to 16

		// Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
		while (!(wavAudio[pos] == 100 && wavAudio[pos + 1] == 97 && wavAudio[pos + 2] == 116 && wavAudio[pos + 3] == 97))
		{
			pos += 4;
			int chunkSize = wavAudio[pos] + wavAudio[pos + 1] * 256 + wavAudio[pos + 2] * 65536 + wavAudio[pos + 3] * 16777216;
			pos += 4 + chunkSize;
		}
		pos += 8;

		// Pos is now positioned to start of actual sound data.
		sampleCount = (wavAudio.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
		if (channelCount == 2) sampleCount /= 2;      // 4 bytes per sample (16 bit stereo)

		// Allocate memory (supporting left channel only)
		float[] unityData = new float[sampleCount];

		// Write to double array/s:
		int i = 0;
		while (pos < wavAudio.Length)
		{
			unityData[i] = BytesToFloat(wavAudio[pos], wavAudio[pos + 1]);
			pos += 2;
			if (channelCount == 2)
			{
				pos += 2;
			}
			i++;
		}

		// Done
		return unityData;
	}

	static private float BytesToFloat(byte firstByte, byte secondByte)
	{
		// Convert two bytes to one short (little endian)
		short s = (short)((secondByte << 8) | firstByte);

		// Convert to range from -1 to (just below) 1
		return s / 32768.0F;
	}

	static private int BytesToInt(byte[] bytes, int offset = 0)
	{
		int value = 0;
		for (int i = 0; i < 4; i++)
		{
			value |= ((int)bytes[offset + i]) << (i * 8);
		}
		return value;
	}
}
