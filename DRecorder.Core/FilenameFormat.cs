using System.Text.RegularExpressions;

namespace DRecorder.Core
{
    public class FilenameFormat
    {
        //private Dictionary<string, string> _formatParams = new();
        private readonly Regex _regex = new(@"{([^}]+)}");

        public string FilePath { get; }
        public string Format { get; set; }
        public string Filename => GetName();
        public string Extension { get; }

        public FilenameFormat(string filePath, string format, string extension)
        {
            FilePath = filePath;
            Format = format;
            Extension = extension;
        }
        private string GetName()
        {
            var name = _regex.Replace(Format, match =>
            {
                var bResult = Enum.TryParse<FilenamePatternParams>(match.Groups[1].Value, out var patternParam);
                if (bResult is false)
                {
                    return match.Value;
                }

                return patternParam switch
                {
                    FilenamePatternParams.DATE => DateTime.Now.ToShortDateString(),
                    _ => ""
                };
            });

            var filename = $"{name}{Extension}";

            if (File.Exists(Path.Combine(FilePath, filename)) is false)
            {
                return filename;
            }

            var count = 1;
            while (true)
            {
                filename = $"{name}({count}){Extension}";
                if (File.Exists(Path.Combine(FilePath, filename)) is false)
                {
                    return filename;
                }

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
