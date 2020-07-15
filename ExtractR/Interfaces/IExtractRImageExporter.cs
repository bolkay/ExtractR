namespace ExtractR.Interfaces
{
    interface IExtractRImageExporter
    {
        bool ExportImages(string destinationDirectory, string fromDirectory);
    }
}
