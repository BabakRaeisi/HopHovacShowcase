using UnityEngine;

public class SFXSelector : MonoBehaviour
{
    [SerializeField]private string StartSound;
    [SerializeField]private string FinishSound;
    public void Initialize()
    {
        SoundManager.Instance.PlayOneShotSound(StartSound, this.transform.position);
    }
    public void Finish(Vector3 position)
    {
        SoundManager.Instance.PlayOneShotSound(FinishSound, position);
    }
}
