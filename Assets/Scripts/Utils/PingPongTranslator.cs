using UnityEngine;

public class PingPongTranslator : Translator
{
    // Update is called once per frame
    protected override void Update()
    {
        float translationOffset = Mathf.PingPong(Time.time * _translationFrequency, _translationAmplitude) * _dirSign;
        switch (_axis)
        {
            case 'x':
                transform.position = _startingPosition + new Vector3(translationOffset, 0f, 0f);
                break;
            case 'y':
                transform.position = _startingPosition + new Vector3(0f, translationOffset, 0f);
                break;
            case 'z':
                transform.position = _startingPosition + new Vector3(0f, 0f, translationOffset);
                break;
        }
    }
}
