using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DRecorder;
public sealed partial class RecordingVisualizer : UserControl
{
    private readonly Queue<float> _valueQueue = new();
    private readonly object _queueLock = new();

    private Size _size;

    // 렌더링 관련 필드 {{{
    private bool _isRendererTask;
    private CancellationTokenSource? _renderTaskCts;
    private readonly ManualResetEventSlim _renderTaskEvent = new();
    private Task? _renderTask;
    // }}}

    private int MaxQueueCount => (int)_size.Width;

    public RecordingVisualizer()
    {
        InitializeComponent();

        Unloaded += (s, e) =>
        {
            if (_renderTaskCts is not null)
            {
                _renderTaskCts.Cancel();
                _renderTask!.Wait();
            }

            canvas.RemoveFromVisualTree();
            canvas = null;
        };
    }

    public void ClearValues()
    {
        lock (_queueLock)
        {
            _valueQueue.Clear();
        }

        Invalidate();
    }

    public void AddValue(float value)
    {
        lock (_queueLock)
        {
            _valueQueue.Enqueue(value);

            while (_valueQueue.Count > MaxQueueCount)
                _valueQueue.Dequeue();
        }

        Invalidate();
    }

    public void Invalidate()
    {
        _renderTaskEvent.Set();
    }

    private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _size = e.NewSize;

        if (_isRendererTask == false)
        {
            Render(_size, 0);

            _isRendererTask = true;

            _renderTaskCts = new();
            _renderTask = Task.Run(() => {
                while (_renderTaskCts.Token.IsCancellationRequested == false)
                {
                    try
                    {
                        _renderTaskEvent.Wait(_renderTaskCts.Token);
                        _renderTaskEvent.Reset();

                        Render(_size, 0);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }

                // Canvas 자원 해제
                Render(new(0, 0), 0);
            }, _renderTaskCts.Token);
        }
    }

    private bool Render(Windows.Foundation.Size size, int syncInterval)
    {
        var bResult = false;

        // 보이지 않는 사이즈면 SwapChain 정리
        if (size.Width <= 0 || size.Height <= 0)
        {
            canvas.SwapChain?.Dispose();
            //canvas.SwapChain = null;

            return false;
        }
        // SwapChain이 없으면 생성
        else if (canvas.SwapChain is null)
        {
            var device = CanvasDevice.GetSharedDevice();
            var swapChain = new CanvasSwapChain(device, (float)size.Width, (float)size.Height, 96);
            canvas.SwapChain = swapChain;
        }
        // 사이즈가 변경되었으면 버퍼 재조정
        else if (canvas.SwapChain.Size != size)
        {
            canvas.SwapChain.ResizeBuffers(size);
        }

        // 그리기
        using (var ds = canvas.SwapChain!.CreateDrawingSession(Colors.Transparent))
        {
            var x = (float)_size.Width;
            var oX = x;
            var centerY = (float)_size.Height / 2;
            var oY = centerY;

            lock (_queueLock)
            {
                foreach (var value in _valueQueue.Reverse())
                {

                    var y = (float)(value * 50 + centerY);

                    ds.DrawLine(oX, oY, x, y, Colors.Red);

                    (oX, oY) = (x, y);
                    x--;
                }
            }
        }

        // 1 = 모니터 주사율에 맞춤, 0 = 최대 속도
        canvas.SwapChain.Present(syncInterval);

        return bResult;
    }
}
