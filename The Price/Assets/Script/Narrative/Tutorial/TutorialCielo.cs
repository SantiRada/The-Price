using System.Collections;
using UnityEngine;

public class TutorialCielo : Tutorial {

    public CallVoice[] voices;

    [Header("Scene 02")]
    public GameObject nivelator;
    public GameObject[] bars;
    public Vector3[] posOfBars;

    public override void CallMadeInteraction() { print("Not required call made specific interaction in heaven"); }
    protected override IEnumerator ChangesInZeroRoom()
    {
        yield return new WaitForSeconds(0.1f);

        Instantiate(voices[0].gameObject, voices[0].positionToCreate, Quaternion.identity);

        // ---- CANCELAR NECESIDAD DE INTERACCIÓN ---- //
        _roomManager.MadeInteraction();
    }
    protected override IEnumerator ChangesInFirstRoom()
    {
        Instantiate(bars[0].gameObject, posOfBars[0], Quaternion.identity);
        Instantiate(bars[1].gameObject, posOfBars[1], Quaternion.identity);

        yield return new WaitForSeconds(0.1f);

        Instantiate(nivelator.gameObject, posOfBars[2], Quaternion.identity);
    }
    protected override IEnumerator ChangesInSecondRoom()
    {
        yield return new WaitForSeconds(0.1f);


    }
    protected override IEnumerator ChangesInThirdRoom()
    {
        yield return new WaitForSeconds(0.1f);


    }
    protected override IEnumerator ChangesInFourthRoom()
    {
        yield return new WaitForSeconds(0.1f);


    }
}