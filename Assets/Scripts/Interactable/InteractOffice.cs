using UnityEngine;

public class InteractOffice : InteractObject
{
    public InteractReactor reactor;
    string code;

    void Start()
    {
        
        GenerateRandomCode();
        SetReactorCode();
        messages[0] = messages[0].Replace("{code}", code);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateRandomCode()
    {
        int min = 100000;
        int max = 999999;
        int randomNumber = Random.Range(min, max);
        code = randomNumber.ToString();
    }

    private void SetReactorCode()
    {
        reactor.SetReactorCode(code);
    }
}
