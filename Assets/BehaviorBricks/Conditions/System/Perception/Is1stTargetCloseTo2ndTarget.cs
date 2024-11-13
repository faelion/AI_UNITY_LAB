using Pada1.BBCore;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace BBUnity.Conditions
{
    /// <summary>
    /// It is a perception condition to check if the objective is close depending on a given distance.
    /// </summary>
    [Condition("Perception/Is1stTargetCloseTo2ndTarget")]
    [Help("Checks whether two targets are close depending on a given distance")]
    public class Is1stTargetCloseTo2ndTarget : GOCondition
    {
        ///<value>Input Target Parameter to to check the distance.</value>
        [InParam("1stTarget")]
        [Help("Target to check the distance")]
        public GameObject target1;

        ///<value>Input Target Parameter to to check the distance.</value>
        [InParam("2ndTarget")]
        [Help("Target to check the distance")]
        public GameObject target2;

        ///<value>Input maximun distance Parameter to consider that the target is close.</value>
        [InParam("closeDistance")]
        [Help("The maximun distance between targets")]
        public float closeDistance;

        /// <summary>
        /// Checks whether a target is close depending on a given distance,
        /// calculates the magnitude between the gameobject and the target and then compares with the given distance.
        /// </summary>
        /// <returns>True if the magnitude between the gameobject and de target is lower that the given distance.</returns>
        public override bool Check()
		{
            return (target1.transform.position - target2.transform.position).sqrMagnitude > closeDistance * closeDistance;
		}
    }
}