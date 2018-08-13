namespace MyCode.Projector
{
    using UnityEngine;
    public class ProjectorManager
    {
        public static void Setsize(Projector _projector, float _scale, float _width, bool _open)
        {
            if (_projector != null)
            {
                _projector.aspectRatio = _width / _scale;
                _projector.orthographicSize = _scale;
                _projector.enabled = _open;
            }
        }

        public static void Setsize(Projector[] _projectors, float _scale, float _width, bool _open)
        {
            foreach (Projector p in _projectors)
                Setsize(p, _scale, _width, _open);
        }

        public static void SwitchPorjector(Projector[] _projectors, bool _open)
        {
            foreach (Projector p in _projectors)
                p.enabled = _open;
        }

        public static void ChangePos(Projector _projector, Transform _originalPos, Vector3 _pos, float _maxRange)
        {
            if (_projector != null)
            {
                if (Vector3.Distance(_pos, _originalPos.position) <= _maxRange)
                    _projector.transform.position = _pos;
            }
        }
    }
}
