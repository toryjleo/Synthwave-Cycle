using UnityEngine;

/// <summary>Class <c>MovementManager</c> A class that needs revising</summary>
public class MovementManager : MonoBehaviour
{
    public ScoreTracker scoreTracker;
    public BikeScript bike;
    public GameObject Cactus;
    public CactusScript cacscrip;
    public GameObject[] cacti;
    public Vector3 curVec;

    public Gun InitialPlayerGun;

    private void Awake()
    {
        UpdateUIEnergy();
    }

    // Start is called before the first frame update
    void Start()
    {
        bike.EquipGun(InitialPlayerGun);
        //Spawn Cactai 
        cacti = new GameObject[10];

        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnP = new Vector3(Random.Range(-80, 80), -2, Random.Range(-80, 80));
            cacti[i] = Instantiate(Cactus, spawnP, Quaternion.identity);
            cacti[i].GetComponent<CactusScript>().grow(spawnP);

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyForces();
    }

    private void Update()
    {

        bike.UpdateRegenEnergy();
        UpdateUIEnergy();  // Update the UI with the HP
    }

    private void UpdateUIEnergy() 
    {
        scoreTracker.Energy = bike.Energy;
    }

    /// <summary>Applies all forces to the Player bike this frame before position is updated.</summary>
    private void ApplyForces()
    {      
        bike.ApplyForces();       
    }

}
