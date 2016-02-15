using System;
using System.Threading;
using Windows.System.Threading;

//namespace BrickPi.Tools
//{
//	public class PIDAbstraction : IDisposable //this was abstract
//    {
		
//		private float k1;
//	    private float k2;
//	    private float k3;
	   
//	    private float Kp;
//	    private float Ki;
//	    private float Kd;
	   
//	    private float Ts;
	   
//	    private float ek2;
//	    private float ek1;
	   
//	    private float uk;
//	    private float uk1;
//	    private float max;
//	    private float min;
	   
//	    private float maxChange;
//	    private float minChange;
		
//	    private bool maxMinChangeSet;
//		protected float currentError { get; set; }
//		protected float currentOutput { get; set; }
//		protected ManualResetEvent done = new ManualResetEvent(false);
//		private ThreadPoolTimer timer;
//        protected virtual void ApplyOutput(float output) //; //this was abstract
//        { }
//        protected virtual float CalculateError() //;//this was abstract
//        { return (float)0.0; }
//        protected virtual bool StopLoop() //;//this was abstract
//        { return false; }

//        public PIDAbstraction(float P, float I, float D, float newSampleTime, float maxOut, float minOut)
//        {
//            pPIDAbstraction(P, I, D, newSampleTime, maxOut, minOut, 0.0f, 0.0f);
//        }
//        public PIDAbstraction(float P, float I, float D, float newSampleTime)
//        {
//            //default float maxOut = 100.0f, float minOut = -100.0f, float maxChangePerSec = 0.0f, float minChangePerSec = 0.0f)
//            pPIDAbstraction(P, I, D, newSampleTime, 100.0f, -100.0f, 0.0f, 0.0f);
//        }
//        public PIDAbstraction(float P, float I, float D, float newSampleTime, float maxOut, float minOut, float maxChangePerSec, float minChangePerSec)
//        {
//            pPIDAbstraction(P, I, D, newSampleTime,  maxOut,  minOut,  maxChangePerSec,  minChangePerSec);
//        }
//        private void pPIDAbstraction(float P, float I, float D, float newSampleTime, float maxOut, float minOut, float maxChangePerSec, float minChangePerSec){
//			Kp = P;
//			Ki = I;
//			Kd = D;
//			Ts = newSampleTime;
//			ek1 = 0;
//			ek2 = 0;
//			uk1 = 0;
//			max = maxOut;
//			min = minOut;
//			if(maxChangePerSec != 0.0){
//			  maxMinChangeSet = true;
//			  maxChange = Ts * maxChangePerSec;
//			  minChange = Ts * minChangePerSec;
//			}
//			else{
//			  maxMinChangeSet = false;
//			}
//			update();
//			//timer = new System.Timers.Timer((double)Ts);
//			//timer.Elapsed += ControlFunction;
//		}


//		public IDisposable Run ()
//		{
//			//if (!timer.Enabled) {
//            if (timer==null) { 
//				done.Reset();
//            //timer.Start ();
//            timer = ThreadPoolTimer.CreatePeriodicTimer(this.ControlFunction, TimeSpan.FromMilliseconds(Ts*1000));
//            }
//		    return done;
//		}
		
//		public void Cancel ()
//		{
//			//if (timer.Enabled) 
//			{
//                //timer.Stop();
//                timer.Cancel();
//				Monitor.Enter(this);
//				done.Set();
//				Monitor.Exit(this);
//			}	
//		}
		
//		public void Dispose()
//		{
//			Cancel();
//		}


//		private void ControlFunction (ThreadPoolTimer source)
//		{
//			if (!Monitor.TryEnter(this))
//			{
//				//cancel has the lock
//				return;
//			}
//			currentError = CalculateError ();
//			currentOutput = CalculateOutput (currentError);
//			ApplyOutput (currentOutput);
//			if (StopLoop ()) 
//			{
//                //timer.Stop ();
//                timer.Cancel();
//				done.Set ();
//			} 
//			Monitor.Exit(this);
//		}

//		private void update ()
//		{
//			k1 = Kp;
//			if (Ki != 0.0f) 
//			{ 
//				k2 = Kp * (Ts / Ki);
//			} 
//			else 
//			{
//				k2 = 0.0f;
//			}
//			k3 = Kp*(Kd/Ts);
//		}

//		private float CalculateOutput (float ek)
//		{
//			uk = uk1 + k1 * (ek - ek1) + k2 * ek + k3 * (ek - 2 * ek1 + ek2);
//			if (uk > max) {
//				uk = max;
//			}
//			if (uk < min) {
//				uk = min;
//			}

//			if (maxMinChangeSet) {
//				if ((uk - uk1) > maxChange) {
//					uk = uk1 + maxChange;
//				}
//				if ((uk - uk1) < minChange) {
//					uk = uk1 + minChange;
//				}
//			}
//			uk1 = uk;
//			ek2 = ek1;
//			ek1 = ek;
//			return uk;
//		}
		
//	}
//}

