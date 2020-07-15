namespace ExtractR.Interfaces
{
    interface IExtractRPDFExporter
    {
        bool ExportPDF(string directory, string resultingFileName);
    }
}
