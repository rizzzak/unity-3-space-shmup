using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    public float waveFrequency = 2f;
    public float waveWidth = 4f;
    public float waveRotY = 45;

    private float x0; // начальное значение координаты x
    private float birthTime;

    // Start is called before the first frame update
    void Start()
    {
        x0 = pos.x;
        birthTime = Time.time;
    }
    public override void Move()
    {
        //синусоидальное движение
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        //вращение
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        //смещение вниз
        base.Move();
    }
}
