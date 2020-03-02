using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PointPositionInfo
{
    public Dictionary<VectorInfo, KeyPointType> Points
    {
        get
        {
            if (points == null)
            {
                points = new Dictionary<VectorInfo, KeyPointType>();
            }
            return points;
        }

        private set
        {
            points = value;
        }
    }

    private Dictionary<VectorInfo, KeyPointType> points;

    public void AddKeyPoint(Vector3 position, KeyPointType type)
    {
        VectorInfo pos = new VectorInfo(position);
        if (!Points.ContainsKey(pos))
            Points[pos] = type;
        if (IsEmpty)
            IsEmpty = false;
    }

    public void RemoveKeyPoint(Vector3 position)
    {
        VectorInfo pos = new VectorInfo(position);
        Points.Remove(pos);
    }

    public bool IsEmpty { get; private set; }

    public PointPositionInfo()
    {
        IsEmpty = true;
    }

    public static void ClearSave()
    {
        if (File.Exists(Application.persistentDataPath + "/positionInfo.save"))
        {
            File.Delete(Application.persistentDataPath + "/positionInfo.save");
        }

    }

    public static void CreateSave()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/positionInfo.save");
        bf.Serialize(file, PlayerManager.PositionInfo);
        file.Close();
    }

    public static void LoadSave()
    {
        if (File.Exists(Application.persistentDataPath + "/positionInfo.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/positionInfo.save", FileMode.Open);
            PlayerManager.PositionInfo = (PointPositionInfo)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void SaveGame()
    {
        ClearSave();

        CreateSave();
    }
}

[System.Serializable]
public class VectorInfo
{
    public Vector3 Position
    {
        private set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
        get
        {
            return new Vector3(x, y, z);
        }
    }

    private float x;
    private float y;
    private float z;

    public VectorInfo(Vector3 position)
    {
        Position = position;
    }
}


