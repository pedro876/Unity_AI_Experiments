using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrainDisplay : MonoBehaviour
{
    [SerializeField] float updateTime = 1f;
    [SerializeField] float zoom = 1f;
    [SerializeField] Vector2 displacement = Vector2.zero;
    [SerializeField] float resMultiplier = 0.2f;
    [SerializeField] TileAgentInputBrain target;
    Texture2D tex;
    RectTransform rect;
    RawImage image;
    BrainImageGenerator imgGenerator;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<RawImage>();
    }

    void Start()
    {
        tex = new Texture2D((int)(rect.rect.width * resMultiplier), (int)(rect.rect.height*resMultiplier));
        tex.filterMode = FilterMode.Point;
        imgGenerator = new BrainImageGenerator(target.brain.state, tex.width, tex.height, resMultiplier);
        StartCoroutine(UpdateImage());
        image.texture = tex;
    }

    IEnumerator UpdateImage()
    {
        while (true)
        {
            if(resMultiplier > 0.01f)
            {
                tex = new Texture2D((int)(rect.rect.width * resMultiplier), (int)(rect.rect.height * resMultiplier));
                tex.filterMode = FilterMode.Point;
                imgGenerator = new BrainImageGenerator(target.brain.state, tex.width, tex.height, resMultiplier);
                image.texture = tex;

                imgGenerator.zoom = zoom * resMultiplier;
                imgGenerator.displacementX = displacement.x;
                imgGenerator.displacementY = displacement.y;
                //Debug.Log(imgGenerator.zoom);
                Color[] colors = imgGenerator.GenerateImage(out double generationSeconds);
                //Debug.Log(generationSeconds);
                tex.SetPixels(colors);
                tex.Apply();
            }
            yield return new WaitForSeconds(updateTime);
        }
    }
}
