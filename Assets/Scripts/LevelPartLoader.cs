using System.Collections.Generic;
using UnityEngine;

namespace TwoPersonProject
{
    public class LevelPartLoader : MonoBehaviour
    {
        public int partsLoadedAtOnce = 30;

        private GameObject[] _parts;
        private readonly List<LevelPart> _loadedParts = new();

        private void Awake()
        {
            _parts = Resources.LoadAll<GameObject>("LevelParts");
            _loadedParts.Add(Instantiate(_parts[0]).GetComponent<LevelPart>());
        }

        private void Update()
        {
            for (int i = 0; i < _loadedParts.Count; i++)
            {
                if (_loadedParts[i] == null)
                {
                    _loadedParts.RemoveAt(i);
                    i--;
                }
            }

            while (_loadedParts.Count < partsLoadedAtOnce)
            {
                GameObject newPart = Instantiate(_parts[Random.Range(0, _parts.Length - 1)]);
                LevelPart part = newPart.GetComponent<LevelPart>();

                LevelPart lastPart = _loadedParts[^1];
                Debug.Log(lastPart.transform.position);
                newPart.transform.position = (Vector2)lastPart.transform.position + lastPart.rightConnection - part.leftConnection;
                _loadedParts.Add(part);
            }
        }
    }
}