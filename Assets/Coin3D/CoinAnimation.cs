using System.Collections;

using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    new MeshRenderer renderer;
    
    [SerializeField ]
    float speedDegrees = 90;

    [SerializeField]
    float shineTime = 2;

    [SerializeField]
    [Range(0, 1)]
    float shineWidth = 0.2f;

    [SerializeField]
    [Range(0.5f, 2)]
    float shineSpeed = 1;

    [SerializeField]
    new Camera camera;

    float angle = 0;

    bool changedColor;

    bool isShining = false;
    Vector2 startEnd = Vector2.zero;
    
    float lastShineTime = 0;
    float lastColorSwitchTime = 0;

    const float COLOR_SWITCH_TIME = 0.5f;

    void Awake()
    {
        TryGetComponent(out renderer);
    }

    void Update()
    {
        angle += Time.deltaTime * speedDegrees;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        if (!isShining) 
        {
            if (Time.time - lastShineTime > shineTime)
            {
                StartCoroutine(Shining());
                lastShineTime = Time.time;
            }
        }

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1)
        {
            Ray screenRay = camera.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(screenRay))
            {
                if (Time.time - lastColorSwitchTime > COLOR_SWITCH_TIME)
                {
                    if (changedColor)
                    { 
                        renderer.material.SetColor("_BaseColor", Color.white);
                        changedColor = false;
                    }
                    else
                    {
                        renderer.material.SetColor("_BaseColor", Color.red);
                        changedColor = true;
                    }

                    lastColorSwitchTime = Time.time;
                }
            }
        }
#endif
#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray screenRay = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenRay))
            {
                if (changedColor)
                { 
                    renderer.material.SetColor("_BaseColor", Color.white);
                    changedColor = false;
                }
                else
                {
                    renderer.material.SetColor("_BaseColor", Color.red);
                    changedColor = true;
                }
            }
        }
#endif
    }

    IEnumerator Shining()
    {
        isShining = true;
        startEnd.x = 0;
        startEnd.y = shineWidth;
        while (startEnd.x < 1)
        {
            ApplyShine();
            startEnd.x += Time.deltaTime * shineSpeed;
            startEnd.y += Time.deltaTime * shineSpeed;
            yield return null;
        }

        isShining = false;

        void ApplyShine()
        {
            renderer.material.SetFloat("_Start", startEnd.x);
            renderer.material.SetFloat("_End", startEnd.y);
        }
    }
}