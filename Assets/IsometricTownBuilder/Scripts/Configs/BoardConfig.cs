using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BoardConfig", menuName = "Scriptable Objects/BoardConfig")]
public class BoardConfig : ScriptableObject
{
    [SerializeField] private int SizeX = 30;
    [SerializeField] private int SizeY = 30;

    //TODO: allow to plot each of the cell type

    [SerializeField] private string JsonToString = "";
    [SerializeField] private string jsonStringToCellPositionConfig;

    [SerializeField] private List<CellConfigData> cellConfigDataList = new List<CellConfigData>();
    public List<CellConfigData> CellConfigDataList => cellConfigDataList;

    public bool AllowEditFromEditor = false;

#if false
    private void OnValidate()
    {
        
        if (!AllowEditFromEditor)
        {
            return;
        }
        
        if (string.IsNullOrEmpty(jsonStringToCellPositionConfig))
        {
            cellConfigDataList.Clear();
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cellConfigDataList.Add(new CellConfigData(0,i, j));
                }
            }
            JsonToString = Newtonsoft.Json.JsonConvert.SerializeObject(cellConfigDataList);
        }
        else
        {
            cellConfigDataList =
 Newtonsoft.Json.JsonConvert.DeserializeObject<List<CellConfigData>>(jsonStringToCellPositionConfig);
        }
    }
#endif
}