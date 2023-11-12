using UnityEngine;

public interface IMovable
{
        public void Hover();
        public void UnHover();
        public bool TryGrab(Transform grabTransform);
        public bool TryPlace();
}