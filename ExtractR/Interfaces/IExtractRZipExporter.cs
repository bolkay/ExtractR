namespace ExtractR.Interfaces
{
    public interface IExtractRZipExporter
    {
        bool ExportZip(string to, string fromDir);
        bool ExportZip();
    }
}
