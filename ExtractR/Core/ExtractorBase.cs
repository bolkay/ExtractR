using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtractR.Core
{
    public abstract class ExtractorBase
    {
        public abstract Task<Dictionary<byte[], string>> ExtractElementsData(string sourceFilePath = null);

        public abstract Task<List<string>> ProcessData(Dictionary<byte[], string> keyValues, string savePath);
        public static TExtractor CreateExtractor<TExtractor>() where TExtractor : ExtractorBase
        {
            return Activator.CreateInstance<TExtractor>();
        }
    }
}
