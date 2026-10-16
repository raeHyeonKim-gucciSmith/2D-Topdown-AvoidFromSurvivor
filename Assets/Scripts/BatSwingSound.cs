using UnityEngine;

public class BatSwingSound : MonoBehaviour
{
    public void PlaySwingSound()
    {
        if (GameAudioManager.I != null)
        {
            GameAudioManager.I.PlayBatSwing();
        }
    }
}