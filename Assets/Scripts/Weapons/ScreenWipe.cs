using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenWipe : MonoBehaviour
{
    private List<Ai> aiObjects;
    private List<Bullet> bulletObjects;

    private void Start()
    {
        aiObjects = FindObjectsOfType<Ai>(true).ToList();
        bulletObjects = FindObjectsOfType<Bullet>(true).ToList();
    }

    /// <summary>
    /// Clears all bullets and enemies from the screen
    /// </summary>
    public void Trigger(PlayerHealth ph)
    {
        // TODO: check if the bar max has increased
        foreach (Ai ai in aiObjects)
        {
            ai.ResetGameObject();
        }
        foreach(Bullet bullet in bulletObjects)
        {
            bullet.ResetBullet();
        }
    }
}
