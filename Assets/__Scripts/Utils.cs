using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    //================ Функции для работы с материалами =================
    /// <summary>
    /// Вернуть список всех материалов в данном игровом объекте и его дочерних
    /// </summary>
    /// <param name="go">Целевой объект / родитель</param>
    /// <returns>Массив материалов всех объектов</returns>
    static public Material[] GetAllMaterials(GameObject go)
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();
        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends)
            mats.Add(rend.material);
        return (mats.ToArray());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
