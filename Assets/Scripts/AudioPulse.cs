using UnityEngine;

public class AudioPulse : MonoBehaviour
{
    private const int SAMPLES = 512; // Needs to be a multiplier of 2
    
    public float rms = 0; // Root Mean Square

    public float speed = 5;

    private AudioSource _audioSource; // AudioSource to get the audio samples
    
    private float _lastRMS = 1; // Cache of the previous RMS frame

    public Color lowColor, highColor; // Target colors

    private Renderer rend; // Renderer to modify the material color

    private void Start()
    {
        rend = GetComponent<Renderer>();
        _audioSource = Camera.main.gameObject.GetComponent<AudioSource>();
        _lastRMS = transform.localScale.y / 10;
    }

    private void Update()
    {
        // Lerp the RMS value to be smooth
        rms = Mathf.Lerp(_lastRMS, GetRMS(), Time.deltaTime * speed);
        
        // Calculate the new scale of the GameObject
        Vector3 scale = new Vector3(transform.localScale.x, rms * 10, transform.localScale.z);
        transform.localScale = scale;

        // Determine which color we want to go
        if (rms < _lastRMS)
        {
            rend.material.color = Color.Lerp(rend.material.color, lowColor, Time.deltaTime * speed);
        }
        else
        {
            rend.material.color = Color.Lerp(rend.material.color, highColor, Time.deltaTime * speed);
        }

        // Cache the new RMS value
        _lastRMS = rms;
    }

    /**
     * This is where the magic happens
     * It returns the RMS of the audio source based on samples
     */
    private float GetRMS()
    {
        float[] samples = new float[SAMPLES];
        _audioSource.GetOutputData(samples, 0);
        float total = 0;
        foreach (float sample in samples)
        {
            total += sample * sample;
        }

        return Mathf.Sqrt(total / samples.Length);
    }
}
