using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimator : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    public RawImage rawImage;

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rawImage.uvRect = new Rect(offset, offset, 1, 1);
    }
}