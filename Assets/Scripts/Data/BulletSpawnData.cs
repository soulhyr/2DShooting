using UnityEngine;

public struct BulletSpawnData
{
    public ObjectType prefab;
    public Vector3 pos;
    public Vector3 dirVec;
    public int speed;
    public Quaternion? rotPrev;
    public Vector3? rotNext;

    public BulletSpawnData(ObjectType prefab, Vector3 pos, Vector3 dirVec, int speed, Quaternion? rotPrev = null, Vector3? rotNext = null)
    {
        this.prefab = prefab;
        this.pos = pos;
        this.dirVec = dirVec;
        this.speed = speed;
        this.rotPrev = rotPrev;
        this.rotNext = rotNext;
    }
}