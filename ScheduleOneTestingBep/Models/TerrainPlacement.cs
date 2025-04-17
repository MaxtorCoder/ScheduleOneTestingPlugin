using UnityEngine;

namespace ScheduleOneTestingBep.Models;

public class TerrainPlacement
{
    public string TerrainName { get; set; }
    public TerrainData TerrainData { get; set; }
    public Material TerrainMaterial { get; set; }
    public Vector3 Position { get; set; }
}
