using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighlightAllButtons : MonoBehaviour
{
    public Button[] buttons;

    [Header("Highlight Settings")]
    public float scaleMultiplier = 1.05f; // max scale
    public float pulseSpeed = 2f;         // how fast it pulses
    public Color outlineColor = Color.white;
    public float outlineSize = 4f;

    private bool isHighlighting = false;

 

    public void HighlightAll()
    {
        isHighlighting = true;

        foreach (Button btn in buttons)
        {
            // Ensure Outline exists
            Outline outline = btn.GetComponent<Outline>();
            if (outline == null)
                outline = btn.gameObject.AddComponent<Outline>();

            outline.effectColor = outlineColor;
            outline.effectDistance = new Vector2(outlineSize, outlineSize);
        }

        // Start pulsing
        StopAllCoroutines();
        StartCoroutine(PulseScale());
    }

    public void RemoveHighlight()
    {
        isHighlighting = false;
        StopAllCoroutines();

        foreach (Button btn in buttons)
        {
            btn.transform.localScale = Vector3.one;
            Destroy(btn.GetComponent<Outline>());
        }
    }

    private IEnumerator PulseScale()
    {
        while (isHighlighting)
        {
            float timer = 0f;

            while (isHighlighting)
            {
                timer += Time.deltaTime * pulseSpeed;
                float scale = 1f + Mathf.Sin(timer) * (scaleMultiplier - 1f); // pulsate
                foreach (Button btn in buttons)
                {
                    btn.transform.localScale = Vector3.one * scale;
                }
                yield return null;
            }
        }
    }
}
