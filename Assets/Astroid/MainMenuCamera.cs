using UnityEngine;

public class CameraSwitcher11 : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;
    public Canvas[] canvasesForCamera1;  // «·ﬂ«‰›”«  «·Œ«’… »«·ﬂ«„Ì—« «·√Ê·Ï
    public Canvas[] canvasesForCamera2;  // «·ﬂ«‰›”«  «·Œ«’… »«·ﬂ«„Ì—« «·À«‰Ì…

    private bool isCamera1Active = true;

    void Start()
    {
        //  √ﬂœ √‰ «·ﬂ«„Ì—« «·√Ê·Ï ‰‘ÿ… Ê«·À«‰Ì… „⁄ÿ·…
        camera1.enabled = true;
        camera2.enabled = false;

        //  ›⁄Ì· ﬂ«‰›”«  «·ﬂ«„Ì—« «·√Ê·Ï Ê≈Œ›«¡ «·À«‰Ì…
        SetCanvasesState(canvasesForCamera1, true);
        SetCanvasesState(canvasesForCamera2, false);
    }

    // Â–Â «·œ«·… ‰—»ÿÂ« »«·“—
    public void SwitchCamera()
    {
        isCamera1Active = !isCamera1Active;

        camera1.enabled = isCamera1Active;
        camera2.enabled = !isCamera1Active;

        //  »œÌ· Õ«·… «·ﬂ«‰›”« 
        SetCanvasesState(canvasesForCamera1, isCamera1Active);
        SetCanvasesState(canvasesForCamera2, !isCamera1Active);

        Debug.Log("«·ﬂ«„Ì—« «·‰‘ÿ… «·¬‰: " + (isCamera1Active ? "«·ﬂ«„Ì—« 1" : "«·ﬂ«„Ì—« 2"));
    }

    // œ«·… „”«⁄œ… · €ÌÌ— Õ«·… „Ã„Ê⁄… „‰ «·ﬂ«‰›”« 
    private void SetCanvasesState(Canvas[] canvases, bool state)
    {
        if (canvases != null)
        {
            foreach (Canvas canvas in canvases)
            {
                if (canvas != null)
                {
                    canvas.enabled = state;
                }
            }
        }
    }
}