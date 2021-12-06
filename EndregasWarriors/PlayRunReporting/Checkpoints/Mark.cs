using System;
using UnityEngine;

namespace UnityUXTesting.EndregasWarriors.Common.Model
{
    public class Mark : UXTool
    {
        [Tooltip(
            "If the wire frame does not appear after changing colors, increase the alpha value in the color picker.")]
        public Color color = Color.magenta;

        public float size = 3f;
        [NonSerialized] public GameObject trackedObject;
        [NonSerialized] public LayerMask trackedObjectLayerMask;

        public event Action Detection;

        protected override void Start()
        {
            trackedObjectLayerMask = trackedObject.layer;
            base.Start();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(gameObject.transform.position, size);
        }

        protected override void Update()
        {
            if (DetectTrackedObject())
                Detection?.Invoke();
        }

        private bool DetectTrackedObject()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, size, trackedObjectLayerMask);

            foreach (var collider in colliders)
            {
                if (trackedObject.gameObject == collider.gameObject)
                {
                    color = Color.cyan;
                    return true;
                }
            }

            return false;
        }
    }
}