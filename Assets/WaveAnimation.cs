using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using yutokun;
using CSVEntry = System.Collections.Generic.List<string>;
using CSVTable = System.Collections.Generic.List<System.Collections.Generic.List<string>>;

public class WaveAnimation : MonoBehaviour
{
    [SerializeField] ComputeShader _compute = null;

    GraphicsBuffer _buffer;
    private GraphicsBuffer _sBuffer;

    public int Count = 512 * 512;

    private List<CSVTable> _allSteps;

    private int _curTimeStep = 0;

    public Text Frame;
    
    void Start()
    {
        _buffer = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured, 512*512, 4 * sizeof(float));
        GetComponent<VisualEffect>().SetGraphicsBuffer("PointBuffer", _buffer);

        Time.fixedDeltaTime = 0.2f;
        
        
        _curTimeStep = 0;
        _allSteps = new List<CSVTable>();

        GetComponent<VisualEffect>().SetGraphicsBuffer("PointBuffer", _buffer);

        var fullPath = Path.GetFullPath("./Assets/Resources/ply2/");

        var allFileNames = Directory.GetFiles(fullPath, "*.csv");
        //CSVTable testLoad = CSVParser.LoadFromPath(allFileNames[0]);
        //Debug.Log(testLoad[0][0]);
        
        // Position xyz -> 14, 15, 16
        // Mask -> 5 ParticleId -> 6
        // Time -> 1

        for (int i = 0; i < allFileNames.Length; i++)
        {
            string csvName = $"p_{i}";
            var csv = Resources.Load($"ply2/{csvName}").ToString();
            _allSteps.Add(CSVParser.LoadFromString(csv));
            // Debug.Log("?");
        }
    }


    void OnDestroy()
    {
        _buffer?.Dispose();
        _buffer = null;
    }

    void FixedUpdate()
    {
        //_compute.SetFloat("Time", Time.time);
        //_compute.SetBuffer(0, "PointBuffer", _buffer);
        // _compute.Dispatch(0, 32, 32, 1);

        _curTimeStep++;
        if (_curTimeStep < _allSteps.Count)
        {
            Vector4[] data = new Vector4[Count];
            //float[] sizeData = new float[Count];
            CSVTable inputData = _allSteps[_curTimeStep];
            for (int i = 1; i < Count; i++)
            {
                if (i < inputData.Count)
                {
                    data[i] = new Vector4(float.Parse(inputData[i][2]), float.Parse(inputData[i][3]), float.Parse(inputData[i][4]), GetHue(int.Parse(inputData[i][1])));
                }
                else
                {
                    data[i] = new Vector4(99999, 99999, 99999, 0);
                }
            }
            _buffer.SetData(data);
            //_sBuffer.SetData(sizeData);
            Frame.text = $"{_curTimeStep}";
        }
        
        
    }

    float GetHue(int idx)
    {
        int colorIdx = Mathf.FloorToInt(idx / 30000f);
        return colorIdx * 0.1f;
    }

    public void Replay()
    {
        _curTimeStep = 0;
    }
}
