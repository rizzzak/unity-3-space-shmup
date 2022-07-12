using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Сериализуемый класс, который будет настроен в инспекторе
/// </summary>
[System.Serializable]
public class Part
{
    public string name;     //имя части корабля
    public float health;    // прочность части
    public string[] protectedBy;// другие части, защищающие эту

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material mat;
}

/// <summary>
/// BOSS
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;

    private Vector3 p0, p1; //точки для интерполяции
    private float timeStart;
    private float duration = 4;

    // Start is called before the first frame update
    void Start()
    {
        p0 = p1 = pos;
        InitMovement();

        //кэшировать игровой объект и материал каждой части в parts[]
        Transform t;
        foreach(Part prt in parts)
        {
            t = transform.Find(prt.name);
            if(t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }
    void InitMovement()
    {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        timeStart = Time.time;
    }
    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;
        if(u>=1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2); // создает плавное замедление
        pos = (1 - u) * p0 + u * p1;
    }

    Part FindPart(string n)
    {
        foreach(Part prt in parts)
        {
            if (prt.name == n)
                return(prt);
        }
        return(null);
    }
    Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
                return(prt);
        }
        return(null);
    }
    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt)
    {
        if (prt == null)
            return (true);
        return (prt.health <= 0);
    }
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                //не повреждать корабль, если он за границей экрана
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                //получаем ссылку на пораженную часть корабля
                GameObject goHit = collision.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if(prtHit == null)
                {
                    //случай, когда goHit ссылается на снаряд Projectile, в этом случае выбирается другой коллайдер
                    goHit = collision.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                //попадание в защищенную часть - не наносить урона
                if (prtHit.protectedBy != null)
                {
                    foreach(string s in prtHit.protectedBy)
                    {
                        if(!Destroyed(s))
                        {
                            Destroy(otherGO);
                            return;
                        }
                    }
                }

                //попадание в незащищенную часть - нанести урон
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                    prtHit.go.SetActive(false);
                bool allDestroyed = true;
                foreach(Part prt in parts)
                {
                    if(!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if(allDestroyed)
                {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                break;
        }
    }

}
