using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    private Rigidbody rb;
    private bool isRolling = false;
    private int currentResult = 0;

    private Dictionary<Vector3, int> diceFaces = new Dictionary<Vector3, int>
    {
        { Vector3.up, 6 },
        { Vector3.down, 1 },
        { Vector3.forward, 5 },
        { Vector3.back, 2 },
        { Vector3.right, 4 },
        { Vector3.left, 3 }
    };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void RollDice()
    {
        isRolling = true;
        rb.AddForce(new Vector3(Random.Range(-5, 5), 10, Random.Range(-5, 5)), ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
        StartCoroutine(WaitForDiceToStop());
    }

    IEnumerator WaitForDiceToStop()
    {
        yield return new WaitForSeconds(1f);
        while (rb.linearVelocity.magnitude > 0.1f || rb.angularVelocity.magnitude > 0.1f)
        {
            yield return null;
        }
        DetermineResult();
        isRolling = false;
    }

    void DetermineResult()
    {
        Vector3 upDirection = Vector3.up;
        float maxDot = -1f;
        int bestFace = 0;

        foreach (var face in diceFaces)
        {
            float dot = Vector3.Dot(transform.TransformDirection(face.Key), upDirection);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestFace = face.Value;
            }
        }

        currentResult = bestFace;
    }

    public bool IsRolling() => isRolling;
    public int GetResult() => currentResult;
}
