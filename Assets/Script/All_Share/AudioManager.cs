using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Queue<AudioSource> audioSourcesPool = new Queue<AudioSource>();
    public audioContainer[] audioClipPool;
    public GameObject audioPrefab;

    [System.Serializable]
    public class audioContainer
    {
        public byte whichOne;
        public AudioClip audioClip;
        [Range(0, 256)]
        public int _priorty;
        [Range(0, 1)]
        public float _volume;
        [Range(-3, 3)]
        public float _pitch;
        public float _min3D;        
        public float _max3D;
    }



    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            AddOneAudioSource();
        }
    }

    void AddOneAudioSource()
    {
        GameObject tmpPrefab = Instantiate(audioPrefab);
        tmpPrefab.transform.SetParent(transform);
        audioSourcesPool.Enqueue(tmpPrefab.GetComponent<AudioSource>());
    }

    public AudioSource GetOneAudioPlay(byte _whichClip, Vector3 _pos)
    {
        AudioSource tmpAudioSource;
        audioContainer tmpAudioClip;

        tmpAudioSource = audioSourcesPool.Dequeue();
        if (audioSourcesPool.Count == 0)
        {
            AddOneAudioSource();
        }
        tmpAudioClip = audioClipPool[_whichClip];

        tmpAudioSource.transform.localPosition = _pos;
        tmpAudioSource.priority = tmpAudioClip._priorty;
        tmpAudioSource.pitch = tmpAudioClip._pitch;
        tmpAudioSource.volume = tmpAudioClip._volume;
        tmpAudioSource.minDistance = tmpAudioClip._min3D;
        tmpAudioSource.maxDistance = tmpAudioClip._max3D;

        tmpAudioSource.PlayOneShot(tmpAudioClip.audioClip);
        //Debug.Log(audioSourcesPool.Count);
        return tmpAudioSource;
    }

    public void ReturnAudioPool(AudioSource _container)
    {

        audioSourcesPool.Enqueue(_container);
        //Debug.Log(audioSourcesPool.Count);
    }



    /*[SerializeField] Transform TestPos;
    public AudioSource testAudioS;
    private void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            testAudioS = GetOneAudioPlay(0, TestPos.position);
            Debug.Log(audioSourcesPool.Count);
        }

        if (Input.GetKeyDown("w"))
        {
            ReturnAudioPool(testAudioS);
            Debug.Log(audioSourcesPool.Count);
        }
    }*/
}
