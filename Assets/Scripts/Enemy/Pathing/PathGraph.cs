using UnityEngine;
using System.Collections;
using UnityEditor;

public class PathGraph : MonoBehaviour
{
    public Transform[] positions = new Transform[2];

    [SerializeField]
    private bool shouldLoop;

    [SerializeField]
    private Color nodeColor = Color.black;

    [SerializeField]
    private Color lineColor = Color.black;

    [SerializeField]
    private Transform target;

    public Transform Target => this.target;

    public bool ShouldLoop => shouldLoop;

    private void Start()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        //Dettach all transforms on play mode
        for (int i = 0; i < this.positions.Length; i++)
        {
            this.positions[i].parent = null;
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (positions == null) return;
        
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i] == null)
            {
                continue;
            }
            Vector2 initialPos = i > 0 ? positions[i - 1].position : this.target.position;
            Gizmos.color = this.lineColor;
            Gizmos.DrawLine(initialPos, positions[i].position);
            //Draw a circle there
            Gizmos.color = this.nodeColor;
            Gizmos.DrawWireSphere(positions[i].position, 0.25f);
            Handles.Label(positions[i].position, $"{this.target.name}'s position #{i + 1}");
        }
    }

    private void OnDestroy()
    {
        this.ClearPaths();
    }

    public void ClearPaths()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            Destroy(this.positions[i].gameObject);
        }
    }

}

