using UnityEngine;
using UnityEngine.UI;

public class CanvasHalfCircle : MonoBehaviour
{
    public Transform spaceship; // ÇáÓİíäÉ
    public RawImage rawImage; // ÇáÕæÑÉ ÇáÊí ÓäÚÑÖåÇ Úáì Ôßá äÕİ ÏÇÆÑÉ
    public float radius = 5f; // äÕİ ŞØÑ äÕİ ÇáÏÇÆÑÉ (ÇáãÓÇİÉ Èíä ÇáÕæÑÉ æÇáÓİíäÉ)
    public float angleStart = -90f; // ÇáÒÇæíÉ ÇáÊí íÈÏÃ ãäåÇ äÕİ ÇáÏÇÆÑÉ
    public float angleEnd = 90f; // ÇáÒÇæíÉ ÇáÊí íäÊåí ÚäÏåÇ äÕİ ÇáÏÇÆÑÉ

    private RectTransform rawImageRectTransform; // ááÅÔÇÑÉ Åáì RectTransform ÇáÎÇÕ ÈÜ RawImage

    void Start()
    {
        // ÇáÍÕæá Úáì RectTransform ÇáÎÇÕ ÈÜ RawImage
        rawImageRectTransform = rawImage.GetComponent<RectTransform>();

        // æÖÚ ÇáÕæÑÉ ÈÔßá ËÇÈÊ İí äÕİ ÇáÏÇÆÑÉ
        PositionRawImage();
    }

    // æÖÚ ÇáÕæÑÉ ÏÇÎá äÕİ ÏÇÆÑÉ Íæá ÇáÓİíäÉ
    void PositionRawImage()
    {
        // ÇÎÊíÇÑ ÒÇæíÉ ËÇÈÊÉ Öãä äÕİ ÇáÏÇÆÑÉ
        float angle = Random.Range(angleStart, angleEnd); // ÇÎÊíÇÑ ÒÇæíÉ ÚÔæÇÆíÉ ÏÇÎá äÕİ ÇáÏÇÆÑÉ

        // ÊÍæíá ÇáÒÇæíÉ Åáì ÑÇÏíÇä
        float radian = angle * Mathf.Deg2Rad;

        // ÍÓÇÈ ãæŞÚ ÇáÕæÑÉ ÈäÇÁğ Úáì ÇáÒÇæíÉ
        Vector3 position = new Vector3(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius, 0f);

        // æÖÚ ÇáÕæÑÉ İí ÇáãæŞÚ ÇáãÍÏÏ ÈÇáäÓÈÉ ááÓİíäÉ
        rawImageRectTransform.position = spaceship.position + position;

        // ÌÚá ÇáÕæÑÉ íæÇÌå ÇáßÇãíÑÇ ÏÇÆãğÇ
        rawImageRectTransform.LookAt(Camera.main.transform);
    }

    // ÅĞÇ ÊÍÑßÊ ÇáÓİíäÉ¡ Şã ÈÊÍÏíË ÇáÕæÑÉ
    void Update()
    {
        // ÅĞÇ ßÇäÊ ÇáÓİíäÉ ÊÊÍÑß¡ íãßä ÊÍÏíË ÇáãæÖÚ ÈÔßá ËÇÈÊ
        PositionRawImage();
    }
}
