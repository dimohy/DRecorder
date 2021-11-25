using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace DMRecorder.Core
{
    public class FilenamePattern
    {
        private Dictionary<string, string> _patternParams = new();
        private Regex _regex = new(@"{([^}]+)}");

        public string FilePath { get; }
        public string Pattern { get; set; }
        public string Filename => GetName();
        public string Extension { get; }

        public FilenamePattern(string filePath, string pattern, string extension)
        {
            FilePath = filePath;
            Pattern = pattern;
            Extension = extension;
        }
        private string GetName()
        {
            var name = _regex.Replace(Pattern, match =>
            {
                var bResult = Enum.TryParse<FilenamePatternParams>(match.Groups[1].Value, out var patternParam);
                if (bResult == false)
                    return match.Value;

                return patternParam switch
                {
                    FilenamePatternParams.DATE => DateTime.Now.ToShortDateString(),
                    _ => ""
                };
            });

            var filename = $"{name}{Extension}";

            if (File.Exists(Path.Combine(FilePath, filename)) == false)
                return filename;

            var count = 1;
            while (true)
            {
                filename = $"{name}({count}){Extension}";
                if (File.Exists(Path.Combine(FilePath, filename)) == false)
                    return filename;

                count++;
            }
        }
    }

    public enum FilenamePatternParams
    {
        /// <summary>
        /// 오늘 날짜
        /// </summary>
        DATE,
    }
}
