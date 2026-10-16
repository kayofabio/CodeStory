
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private int faseAtual = 1;

    public int FaseAtual => faseAtual;

    public void DefinirFase(int novaFase)
    {
        if (novaFase < 1 || novaFase > 2)
        {
            Debug.LogWarning("Fase inválida: " + novaFase);
            return;
        }

        faseAtual = novaFase;
    }
}
