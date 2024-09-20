using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using Auxiliars;

public class PathTravelController : MonoBehaviour
{

	[SerializeField]
	private EnemyMovement movementRef;

	[SerializeField]
	private EnemyBehaviour behaviourRef;

	private PathGraph pathGraph;

	private int currIndex;

	private void Start()
	{
		Assert.IsNotNull(this.movementRef);
		Assert.IsNotNull(this.behaviourRef);
		this.pathGraph = GetComponent<PathGraph>();
	}

    private void FixedUpdate()
    {
		if (this.behaviourRef.IsPlayerDetected) return;

		if (this.currIndex >= this.pathGraph.positions.Length) {
			if (this.pathGraph.ShouldLoop)
			{
				this.currIndex = 0;
				return;
			}
			this.movementRef.Stop();
			return;
		}
        Vector2 targetPos = this.pathGraph.positions[this.currIndex].position;

        if (SpartanMath.ArrivedAt(this.movementRef.transform.position, targetPos))
		{
			this.currIndex++;
			return;
		}
		Vector2 dir = (targetPos - (Vector2)this.movementRef.transform.position).normalized;

        this.movementRef.MoveTo(dir, this.movementRef.Speed);

    }
}

