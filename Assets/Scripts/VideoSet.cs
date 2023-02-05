using System;
using UnityEngine.Video;

[Serializable]
public class VideoSet
{
    // video clip for idle state
    public VideoClip idleClip;

    // video clip for winning state
    public VideoClip winningClip;

    // video clip for losing state
    public VideoClip losingClip;

    // Start is called before the first frame update
    public VideoSet(VideoClip idleClip, VideoClip winningClip, VideoClip losingClip)
    {
        this.idleClip = idleClip;
        this.winningClip = winningClip;
        this.losingClip = losingClip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
