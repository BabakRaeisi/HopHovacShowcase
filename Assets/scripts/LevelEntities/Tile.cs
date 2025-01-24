using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Renderer tileRenderer;
    public Material UnoccupiedMat;
    public GameObject VFX;
    

    public void ResetMaterial()
    {
        if (tileRenderer != null)
        {
            SoundManager.Instance.PlayOneShotSoundClip("TileFlip", this.transform.position);
            tileRenderer.material = UnoccupiedMat;
            StartCoroutine(EnableVFX());
        }
    }

    public void SetMaterial(Material mat)
    {
        if (tileRenderer != null)
        {
            SoundManager.Instance.PlayOneShotSoundClip("Tile", this.transform.position);
            tileRenderer.material = mat;
            StartCoroutine(EnableVFX());
        }
    }

    private IEnumerator EnableVFX()
    {
        if (VFX != null)
        {
            VFX.SetActive(true);
            yield return new WaitForSeconds(1f);  
            VFX.SetActive(false);
        }
    }
}
