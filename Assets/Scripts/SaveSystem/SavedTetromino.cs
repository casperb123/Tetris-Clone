using System;

[Serializable]
public class SavedTetromino
{
    public string Name { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public int RotationZ { get; set; }

    public SavedTetromino(string name)
    {
        Name = name;
    }

    public SavedTetromino(string name, float positionX, float positionY, int rotationZ) : this(name)
    {
        PositionX = positionX;
        PositionY = positionY;
        RotationZ = rotationZ;
    }
}
