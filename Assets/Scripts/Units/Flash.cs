using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Mostly stole this from the top down RPG course. Thanks Stephan.
public class Flash : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = .2f;

    private Color defaultColor;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    public IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = defaultColor;
    }
}
