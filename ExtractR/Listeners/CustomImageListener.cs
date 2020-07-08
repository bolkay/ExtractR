using ExtractR.Implementations;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using System;
using System.Collections.Generic;

namespace ExtractR.Listeners
{
    public class CustomImageListener : IEventListener
    {
        public CustomImageListener(Dictionary<byte[], string> imageDataAndExtension)
        {
            ImageDataAndExtension = imageDataAndExtension;
        }

        public Dictionary<byte[], string> ImageDataAndExtension { get; }

        public void EventOccurred(IEventData data, EventType type)
        {
            switch (type)
            {
                case EventType.RENDER_IMAGE:
                    try
                    {
                        //Get the image render information.
                        ImageRenderInfo imageRenderInfo = data as ImageRenderInfo;

                        //Get the image object from the render information.
                        PdfImageXObject pdfImageXObject = imageRenderInfo.GetImage();
                        if (pdfImageXObject != null)
                        {
                            byte[] imageData = pdfImageXObject.GetImageBytes(true);

                            //Store.
                            ImageDataAndExtension.Add(imageData, pdfImageXObject.IdentifyImageFileExtension());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                default:
                    return;
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return null;
        }
    }
}
