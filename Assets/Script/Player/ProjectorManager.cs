namespace MyCode.Projector
{
    using UnityEngine;
    public class ProjectorManager
    {
        public static void Setsize(Projector _projector, float _scale, float _width, bool _open)
        {
            if (_projector != null)
            {
                _projector.aspectRatio = _width;
                _projector.orthographicSize = _scale;
                _projector.enabled = _open;
            }
        }

        public static void Setsize(Projector[] _projectors, float _scale, float _width, bool _open)
        {
            for (int i = 0; i < _projectors.Length; i++)
            {
                Setsize(_projectors[i], _scale, _width, _open);
            }
        }

        public static void SwitchPorjector(Projector[] _projectors, bool _open)
        {
            for (int i = 0; i < _projectors.Length; i++)
            {
                _projectors[i].enabled = _open;
            }
        }
    }
}
