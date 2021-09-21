using UnityEngine;
using UnityEngine.Serialization;

namespace OpenSkiJumping.Hills.TerrainGenerator
{
    public class ResourcesController : MonoBehaviour
    {
        [SerializeField] private Terrain[] terrains;

        [FormerlySerializedAs("terrainData")] [SerializeField]
        private ElevationData elevationData;

        [SerializeField] private float tileSize;
        [SerializeField] private float minHeight;
        [SerializeField] private MapLocalTangentPlane mapLtp;
        [SerializeField] private TerrainScript terrainScript;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector2 centerCoords;

        public void Awake()
        {
            terrains = GetComponentsInChildren<Terrain>();
        }

        public void DownloadData()
        {
        }

        public void Generate()
        {
            elevationData.GetTileContainingCoord(centerCoords, out var tile);
            // var tile = new DataTile(HgtReader.ReadFile(elevationData.FilePath), elevationData.Coords,
            //     elevationData.Resolution);
            // var halfTileY = Mathf.CeilToInt((mapLtp.ToGeog(new Vector3(
            //     0, 0, tileSize), false) - mapLtp.centerCoords).x * 3600);
            //
            // var minY = halfTileY;
            // var maxY = -halfTileY;
            // var borderPoint = mapLtp.ToGeog(new Vector3(tileSize / 2, 0, 0), false);
            // var halfTileX = Mathf.CeilToInt((borderPoint - mapLtp.centerCoords).z * 1200);
            // var minX = halfTileX;
            // var maxX = -halfTileX;
            //
            // foreach (var terrain in terrains)
            // {
            //     var resolution = terrain.terrainData.heightmapResolution - 1;
            //     var terrainPosition = terrain.transform.position;
            //
            //     var heights = new float[resolution + 1, resolution + 1];
            //     for (var i = 0; i <= resolution; i++)
            //     {
            //         for (var j = 0; j <= resolution; j++)
            //         {
            //             var coords =
            //                 mapLtp.ToGeog(
            //                     mapLtp.centerCoords + terrainPosition + new Vector3(j, 0, i) * tileSize / resolution,
            //                     false);
            //
            //             heights[i, j] =
            //                 (tile.GetInterpolatedHeight(coords) - minHeight) / 800;
            //         }
            //     }
            //
            //     terrain.terrainData.SetHeights(0, 0, heights);
            // }
        }
    }
}