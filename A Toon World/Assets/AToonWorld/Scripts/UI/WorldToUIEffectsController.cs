using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToUIEffectsController : MonoBehaviour
{
    [SerializeField] private GameObject _worldToUIEffect = null;

    // Start is called before the first frame update
    void Awake()
    {
        ObjectPoolingManager<string>.Instance.CreatePool(nameof(_worldToUIEffect), _worldToUIEffect, 5, 10, true, true);
    }

    public void SpawnEffects(int number, Sprite sprite, Transform from, ScreenToWorldPointComponent toUI) =>
        SpawnEffects(number, sprite, Vector3.one, Color.white, from, toUI);
    public void SpawnEffects(int number, Sprite sprite, Color color, Transform from, ScreenToWorldPointComponent toUI) =>
        SpawnEffects(number, sprite, Vector3.one, color, from, toUI);
    public void SpawnEffects(int number, Sprite sprite, Vector3 scale, Transform from, ScreenToWorldPointComponent toUI) =>
        SpawnEffects(number, sprite, scale, Color.white, from, toUI);
    public void SpawnEffects(int number, Sprite sprite, Vector3 scale, Color spriteColor, Transform from, ScreenToWorldPointComponent toUI)
    {
        for(int i = 0; i < number; i++)
        {
            GameObject effect = ObjectPoolingManager<string>.Instance.GetObject(nameof(_worldToUIEffect));
            var interpolator = effect.GetComponent<WorldToUIInterpolatorComponent>();
            var renderer = effect.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = spriteColor;
            effect.GetComponent<Animator>().speed = Random.Range(0.75f, 1.25f);
            effect.transform.localScale = scale;
            interpolator.Bind(toUI, from);
        }
    }
}
