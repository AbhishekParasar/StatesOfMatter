using UnityEngine;
using static MatterManager;

public enum Molecules
{
    Argon,Nitrogen,Oxygen,Water,Intro,StartScreen
}

public class StateStatus : MonoBehaviour
{
    public static StateStatus Instance;

    public bool isArgonCompleted;
    public bool isNitrogenCompleted;
    public bool isOxygenCompleted;
    public bool isWaterCompleted;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }


    public void SetMoleculeState(Molecules molecules)
    {
        switch (molecules)
        {
            case Molecules.Argon:
                isArgonCompleted = true;
                break;
            case Molecules.Nitrogen:
                isNitrogenCompleted = true;
                break;
            case Molecules.Oxygen:
                isOxygenCompleted = true;
                break;
            case Molecules.Water:
                isWaterCompleted = true;
                break;
        }
    }

    public void ResetData()
    {
        Invoke("ResetAllStates",0.5f);
    }

    void ResetAllStates()
    {
        isArgonCompleted = false;
        isNitrogenCompleted = false;
        isOxygenCompleted = false;
        isWaterCompleted = false;
    }

    

    }
