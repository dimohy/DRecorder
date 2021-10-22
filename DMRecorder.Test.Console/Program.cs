//using System.Text.RegularExpressions;

//var text = $"record({{{FilenamePatternParams.DATE}}})({{{FilenamePatternParams.DUPNUM}}})";
//Console.WriteLine(text);

//Regex regex = new(@"{([^}]+)}");

//var result = regex.Replace(text, match =>
//{
//    var bResult = Enum.TryParse<FilenamePatternParams>(match.Groups[1].Value, out var patternParam);
//    if (bResult == false)
//        return match.Value;

//    return patternParam switch
//    {
//        FilenamePatternParams.DATE => DateTime.Now.ToShortDateString(),
//        FilenamePatternParams.DUPNUM => "2",
//        _ => match.Value
//    };
//});
//Console.WriteLine(result);

//public enum FilenamePatternParams
//{
//    /// <summary>
//    /// 오늘 날짜
//    /// </summary>
//    DATE,
//    /// <summary>
//    /// 중복 파일이름 순번
//    /// </summary>
//    DUPNUM
//}


using NAudio.Wave;

Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

var drivers = AsioOut.GetDriverNames();

foreach (var driver in drivers)
{
    Console.WriteLine(driver);
}

var driverName = "Focusrite USB ASIO";
var sampleRate = 96000;

using var asioOut = new AsioOut(driverName);
asioOut.InputChannelOffset = 0;
asioOut.InitRecordAndPlayback(null, 1, sampleRate);


var buffer = new float[512];
using var writer = new WaveFileWriter(@"W:\output.wav", new WaveFormat(sampleRate, 1));
asioOut.AudioAvailable += (s, e) =>
{
    var count = e.GetAsInterleavedSamples(buffer);

    writer.WriteSamples(buffer, 0, count);
};

asioOut.Play();

Console.ReadLine();

asioOut.Dispose();
