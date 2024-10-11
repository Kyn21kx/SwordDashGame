using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Yes, even this is going to be physics based
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TextMeshPro))]
public class TextPopup : MonoBehaviour, IPhysicsAnimation
{
    private const float MIN_SPEED = 3f;
    private const float MAX_SPEED = 7f;

    public bool IsRunning => this.gameObject.activeInHierarchy;

    public string PopupText { get => this.tmpMesh.text;
        set {
            this.tmpMesh.text = value;
        }
    }
    
    [SerializeField]
    private TextMeshPro tmpMesh;

    [SerializeField]
    private Rigidbody2D rig;

    [SerializeField]
    private float opacityFalloutSpeed;

    private void Update()
    {
        //Reduce the text's opacity until we hit 0 and then destroy
        this.tmpMesh.alpha = SpartanMath.Lerp(this.tmpMesh.alpha, -1f, Time.deltaTime * opacityFalloutSpeed);
        if (this.tmpMesh.alpha <= 0f)
        {
            this.StopAnimation();
        }
    }

    public void StartAnimation()
    {
        //Add a force to the rig in a diagonal direction = (rand +/-, 1f)
        var direction = new Vector2(SpartanMath.RandSign(), 1f).normalized;
        float speed = Random.Range(MIN_SPEED, MAX_SPEED);
        this.rig.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    public void StopAnimation()
    {
        Destroy(this.gameObject);
    }
}
