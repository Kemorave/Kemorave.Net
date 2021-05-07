using System;
using System.Diagnostics;
using System.Timers;

namespace Kemorave.Normalization
{

    public class TransferRateCalculatour : IDisposable
 {
  public delegate int Action();
  public enum CalculationRate { PerMil, PerSec, PerMin, PerHou }
  public TransferRateCalculatour(int updateInterval, Func<int> getElabsed, Action<int> callback)
  {  Timer = new Timer(updateInterval);
   Stopwatch = new Stopwatch();
   Timer.Elapsed += Timer_Elapsed;
 
   _getElabsed = getElabsed;
   _callback = callback;
  }

        readonly Func<int> _getElabsed;
        readonly Action<int> _callback;
  public CalculationRate CalculationPerTime { get; set; } = CalculationRate.PerSec;
  public void Start() { Timer.Start(); Stopwatch.Start(); }
  public void Stop() { Timer.Stop(); Stopwatch.Stop(); }
  public bool IsRunning { get => Stopwatch.IsRunning; }
  private void Timer_Elapsed(object sender, ElapsedEventArgs e) => OnTimeInterval(e);


  protected virtual void OnTimeInterval(ElapsedEventArgs e)
  {
   if (!Stopwatch.IsRunning)
   {
    Stopwatch.Start();
   }
   switch (CalculationPerTime)
   {
    case CalculationRate.PerMil:

     if ((int) Stopwatch.Elapsed.TotalMilliseconds > 0)
      TransferRate = _getElabsed.Invoke() / (int)Stopwatch.Elapsed.TotalMilliseconds; break;
    case CalculationRate.PerSec:
     if((int)Stopwatch.Elapsed.TotalSeconds>0)
     TransferRate = _getElabsed.Invoke() / (int)Stopwatch.Elapsed.TotalSeconds; break;
    case CalculationRate.PerMin:

     if ((int)Stopwatch.Elapsed.TotalMinutes > 0)
      TransferRate = _getElabsed.Invoke() / (int)Stopwatch.Elapsed.TotalMinutes; break;
    case CalculationRate.PerHou:

     if ((int)Stopwatch.Elapsed.TotalHours > 0)
      TransferRate = _getElabsed.Invoke() / (int)Stopwatch.Elapsed.TotalHours; break;
    default:
     break;
   }
   _callback.Invoke(TransferRate);
  }

  public void Dispose() {
   Timer.Stop();
   Stopwatch.Stop();
   Timer.Dispose();
  }

  private readonly Stopwatch Stopwatch;
  private readonly Timer Timer;

  public virtual int TransferRate { get; protected set; }
 }
}