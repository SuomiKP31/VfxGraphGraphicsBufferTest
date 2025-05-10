using UnityEngine;
using UnityEngine.VFX;

public class WaveAnimation : MonoBehaviour
{
    [SerializeField] ComputeShader _compute = null;

    GraphicsBuffer _buffer;

    public int Count = 512 * 512;
    
    void Start()
    {
        _buffer = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured, 512*512, 4 * sizeof(float));
        GetComponent<VisualEffect>().SetGraphicsBuffer("PointBuffer", _buffer);
        
        
        _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, Count, 4 * sizeof(float));


        GetComponent<VisualEffect>().SetGraphicsBuffer("PointBuffer", _buffer);
    }


    void OnDestroy()
    {
        _buffer?.Dispose();
        _buffer = null;
    }

    void Update()
    {
        _compute.SetFloat("Time", Time.time);
        _compute.SetBuffer(0, "PointBuffer", _buffer);
        // _compute.Dispatch(0, 32, 32, 1);
        
        Vector4[] data = new Vector4[Count];
        for (int i = 0; i < Count; i++)
        {
            data[i] = new Vector4(Random.value, Random.value, Random.value, Random.value);
        }

        _buffer.SetData(data);
        
    }
}
