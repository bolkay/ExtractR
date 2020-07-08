using System;

namespace ExtractR.Events
{
    public class ImageSavedEventArgs : EventArgs
    {
        public int Progress { get; set; } 
        public int TotalElementsProcessed { get; set; }
    }
}
