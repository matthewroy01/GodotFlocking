using Godot;
using System;
using System.Collections.Generic;

public class BulletPooler : Node
{
    private List<Projectile> bulletsFree = new List<Projectile>();
    private List<Projectile> bulletsUsed = new List<Projectile>();

    public void Initialize(PackedScene bulletScene, int num, Node parent)
    {
        CallDeferred("InitializeTrue", bulletScene, num, parent);
    }

    private void InitializeTrue(PackedScene bulletScene, int num, Node parent)
    {
        for (int i = 0; i < num; ++i)
        {
            // create and position bullet
            Projectile tmp = (Projectile)bulletScene.Instance() as Projectile;
            tmp.Translation = new Vector3(0.0f, -1000.0f, 0.0f);
            tmp.Name = "Bullet" + i;
            tmp.Sleeping = true;

            // add it to the list of free bullets
            bulletsFree.Add(tmp);

            // add it to the scene
            parent.AddChild(tmp);
        }

        GD.Print(bulletsFree.Count);
    }

    public Projectile GetBullet()
    {
        Projectile bullet = null;

        if (bulletsFree.Count > 0)
        {
            bullet = bulletsFree[0];

            bullet.Sleeping = false;
            bulletsFree.Remove(bullet);
            bulletsUsed.Add(bullet);
        }

        return bullet;
    }
}
