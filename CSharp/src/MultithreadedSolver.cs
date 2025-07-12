using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

//A = Cmin*Z_H + D*(MX*MR_H);


 class Program {

  const int N = 16;
  const int P = 8;
  const int H = N / P;

  static int[] C = new int[N];

  static int[] A = new int[N];
  static int[] MR_H = new int[N * N];
  static int[] Z_H = new int[N];
  static int[] MX = new int[N * N];
  static int[] D = new int[N];
  static int[] Cmin = new int[P];


  static int[] MXR_H = new int[N * N];
  static int[] MXRD_H = new int[N];

  static int[] CminZ_H = new int[N];
  static int sum1 = 0;
  static int sum2 = 0;
  static int sum3 = 0;
  static int sum4 = 0;
  static int sum5 = 0;
  static int sum6 = 0;
  static int sum7 = 0;
  static int sum8 = 0;
  static long minC;


  static EventWaitHandle E1 = new ManualResetEvent(false);
  static EventWaitHandle E2 = new ManualResetEvent(false);
  static EventWaitHandle E4 = new ManualResetEvent(false);


  static Semaphore S1 = new Semaphore(0, 6);
  static Semaphore S2 = new Semaphore(0, 14);
  static Semaphore S3 = new Semaphore(0, 6);
  static Semaphore S4 = new Semaphore(0, 6);
  static Semaphore S5 = new Semaphore(0, 6);
  static Semaphore S6 = new Semaphore(0, 6);
  static Semaphore S7 = new Semaphore(0, 6);
  static Semaphore S8 = new Semaphore(0, 6);


  static void Func1() {
   Console.WriteLine("T1 started.");

   for (int i = 0; i < N; i++) {
    C[i] = i + 1;
    Z_H[i] = i;
   }

   //Сигнал о вводе
   E1.Set();
   //Ждать завершения ввода в others
   E2.WaitOne();

   E4.WaitOne();


   Cmin[0] = C.Skip(0).Take(H).Min();

   S1.Release();
   S2.WaitOne();

   for (int i = 0; i < H; i++) {
    CminZ_H[i] = Convert.ToInt32(Interlocked.Read(ref minC)) * Z_H[i];
   }

   // MXR_H = 
   S1.Release();

   for (int i = 0; i < N; i++) {
    for (int j = 0; j < H; j++) {
     sum1 = 0;
     for (int z = 0; z < N; z++) {

      sum1 += MR_H[j * N + z] * MX[i * N + z];

     }

     MXR_H[i + j * N] = sum1;

    }
   }

   S1.Release();

   for (int i = 0; i < H; i++) {
    sum1 = 0;
    for (int j = 0; j < N; j++) {

     sum1 = sum1 + D[j] * MXR_H[i * N + j];

    }
    MXRD_H[i] = sum1;

   }
   S1.Release();

   for (int i = 0; i < H; i++) {
    A[i] = MXRD_H[i] + CminZ_H[i];
   }

   S2.Release();

   Console.WriteLine("T1 finished.");
  }

  static void Func2() {
   Console.WriteLine("T2 started.");

   for (int i = 0; i < N; i++) {
    for (int j = 0; j < N; j++) {
     MX[i * N + j] = i + j;
    }
   }
   //Сигнал others о вводе
   E2.Set();
   //Ждать завершения ввода в others
   E1.WaitOne();
   E4.WaitOne();

   Cmin[1] = C.Skip(H).Take(2 * H).Min();

   S1.WaitOne();
   S3.WaitOne();
   S4.WaitOne();
   S5.WaitOne();
   S6.WaitOne();
   S7.WaitOne();
   S8.WaitOne();

   int minimal = Cmin.Min();

   Interlocked.Add(ref minC, minimal);

   S2.Release();
   S2.Release();
   S2.Release();
   S2.Release();
   S2.Release();
   S2.Release();
   S2.Release();
   for (int i = H; i < 2 * H; i++) {
    CminZ_H[i] = Z_H[i] * minimal;
   }
   S1.WaitOne();
   S3.WaitOne();
   S4.WaitOne();
   S5.WaitOne();
   S6.WaitOne();
   S7.WaitOne();
   S8.WaitOne();

   // MXR_H = 

   for (int i = 0; i < N; i++) {

    for (int j = H; j < 2 * H; j++) {
     sum2 = 0;
     for (int z = 0; z < N; z++) {
      sum2 += MR_H[j * N + z] * MX[i * N + z];
     }
     MXR_H[i + j * N] = sum2;
    }
   }


   S1.WaitOne();
   S3.WaitOne();
   S4.WaitOne();
   S5.WaitOne();
   S6.WaitOne();
   S7.WaitOne();
   S8.WaitOne();


   for (int i = H; i < 2 * H; i++) {
    sum2 = 0;
    for (int j = 0; j < N; j++) {
     sum2 += D[j] * MXR_H[i * N + j];
    }
    MXRD_H[i] = sum2;
   }


   S1.WaitOne();
   S3.WaitOne();
   S4.WaitOne();
   S5.WaitOne();
   S6.WaitOne();
   S7.WaitOne();
   S8.WaitOne();

   for (int i = H; i < 2 * H; i++) {
    A[i] = MXRD_H[i] + CminZ_H[i];
   }
   S2.WaitOne();
   S2.WaitOne();
   S2.WaitOne();
   S2.WaitOne();
   S2.WaitOne();
   S2.WaitOne();
   S2.WaitOne();

   for (var i = 0; i < N; i++) {
    Console.Write("A: " + A[i].ToString().PadLeft(4));
    Console.WriteLine();
   }

   Console.WriteLine("T2 finished.");
  }

  static void Func3() {

   for (int i = 2 * H; i < 3 * H; i++)
    Cmin[2] = C.Skip(2 * H).Take(3 * H).Min();

   S3.Release();
   S2.WaitOne();

   for (int i = 2 * H; i < 3 * H; i++) {
    CminZ_H[i] = Z_H[i] * Convert.ToInt32(Interlocked.Read(ref minC));
   }
   // MXR_H = 
   S3.Release();
   for (int i = 0; i < N; i++) {

    for (int j = 2 * H; j < 3 * H; j++) {
     sum3 = 0;
     for (int z = 0; z < N; z++) {
      sum3 += MR_H[j * N + z] * MX[i * N + z];
     }
     MXR_H[i + j * N] = sum3;
    }
   }
   S3.Release();
   for (int i = 2 * H; i < 3 * H; i++) {
    sum3 = 0;
    for (int j = 0; j < N; j++) {
     sum3 += D[j] * MXR_H[i * N + j];
    }
    MXRD_H[i] = sum3;
   }
   S3.Release();
   for (int i = 2 * H; i < 3 * H; i++) {
    A[i] = MXRD_H[i] + CminZ_H[i];
   }
   S2.Release();
   Console.WriteLine("T3 finished.");
  }
  static void Func4() {
   Console.WriteLine("T4 started.");
   //Ввод МR, D
   for (int i = 0; i < N; i++) {
    D[i] = i;
    for (int j = 0; j < N; j++) {
     MR_H[i * N + j] = i + j;
    }
   }
   //Сигнал Т1,Т3 о вводе
   E4.Set();
   //Ждать завершения ввода в Т1,Т3
   E1.WaitOne();
   E2.WaitOne();
   //Счёт1
   for (int i = 3 * H; i < 4 * H; i++)
    Cmin[3] = C.Skip(3 * H).Take(4 * H).Min();

   S4.Release();
   S2.WaitOne();

   for (int i = 3 * H; i < 4 * H; i++) {
    CminZ_H[i] = Z_H[i] * Convert.ToInt32(Interlocked.Read(ref minC));
   }
   // MXR_H = 
   S4.Release();
   for (int i = 0; i < N; i++) {

    for (int j = 3 * H; j < 4 * H; j++) {
     sum4 = 0;
     for (int z = 0; z < N; z++) {
      sum4 += MR_H[j * N + z] * MX[i * N + z];
     }
     MXR_H[i + j * N] = sum4;
    }
   }
   S4.Release();
   for (int i = 3 * H; i < 4 * H; i++) {
    sum4 = 0;
    for (int j = 0; j < N; j++) {
     sum4 = sum4 + D[j] * MXR_H[i * N + j];
    }
    MXRD_H[i] = sum4;
   }
   S4.Release();
   for (int i = 3 * H; i < 4 * H; i++) {
    A[i] = MXRD_H[i] + CminZ_H[i];
   }
   S2.Release();
   Console.WriteLine("T4 finished.");
  }
  static void Func5() {
   Console.WriteLine("T5 started.");
   for (int i = 4 * H; i < 5 * H; i++)
    Cmin[4] = C.Skip(4 * H).Take(5 * H).Min();

   S5.Release();
   S2.WaitOne();

   for (int i = 4 * H; i < 5 * H; i++) {
    CminZ_H[i] = Z_H[i] * Convert.ToInt32(Interlocked.Read(ref minC));
   }
   // MXR_H = 
   S5.Release();
   for (int i = 0; i < N; i++) {

    for (int j = 4 * H; j < 5 * H; j++) {
     sum5 = 0;
     for (int z = 0; z < N; z++) {
      sum5 = sum5 + MR_H[j * N + z] * MX[i * N + z];
     }
     MXR_H[i + j * N] = sum5;
    }
   }
   S5.Release();
   for (int i = 4 * H; i < 5 * H; i++) {
    sum5 = 0;
    for (int j = 0; j < N; j++) {
     sum5 = sum5 + D[j] * MXR_H[i * N + j];
    }
    MXRD_H[i] = sum5;
   }
   S5.Release();
   for (int i = 4 * H; i < 5 * H; i++) {
    A[i] = MXRD_H[i] + CminZ_H[i];
   }
   S2.Release();
   Console.WriteLine("T5 finished.");
  }
  static void Func6() {
   Console.WriteLine("T6 started.");
   for (int i = 5 * H; i < 6 * H; i++)
    Cmin[5] = C.Skip(5 * H).Take(6 * H).Min();

   S6.Release();
   S2.WaitOne();

   for (int i = 5 * H; i < 6 * H; i++) {
    CminZ_H[i] = Z_H[i] * Convert.ToInt32(Interlocked.Read(ref minC));
   }
   // MXR_H = 
   S6.Release();
   for (int i = 0; i < N; i++) {

    for (int j = 5 * H; j < 6 * H; j++) {
     sum6 = 0;
     for (int z = 0; z < N; z++) {
      sum6 = sum6 + MR_H[j * N + z] * MX[i * N + z];
     }
     MXR_H[i + j * N] = sum6;
    }
   }
   S6.Release();

   for (int i = 5 * H; i < 6 * H; i++) {
    sum6 = 0;
    for (int j = 0; j < N; j++) {
     sum6 += D[j] * MXR_H[i * N + j];
    }
    MXRD_H[i] = sum6;
   }
   S6.Release();
   for (int i = 5 * H; i < 6 * H; i++) {
    A[i] = MXRD_H[i] + CminZ_H[i];
   }
   S2.Release();
   Console.WriteLine("T6 finished.");
  }
  static void Func7() {
   Console.WriteLine("T7 started.");
   for (int i = 6 * H; i < 7 * H; i++)
    Cmin[6] = C.Skip(6 * H).Take(7 * H).Min();

   S7.Release();
   S2.WaitOne();

   for (int i = 6 * H; i < 7 * H; i++) {
    CminZ_H[i] = Z_H[i] * Convert.ToInt32(Interlocked.Read(ref minC));
   }
   // MXR_H = 
   S7.Release();
   for (int i = 0; i < N; i++) {

    for (int j = 6 * H; j < 7 * H; j++) {
     sum7 = 0;
     for (int z = 0; z < N; z++) {
      sum7 = sum7 + MR_H[j * N + z] * MX[i * N + z];
     }
     MXR_H[i + j * N] = sum7;
    }
   }
   S7.Release();
   for (int i = 6 * H; i < 7 * H; i++) {
    sum7 = 0;
    for (int j = 0; j < N; j++) {
     sum7 += D[j] * MXR_H[i * N + j];
    }
    MXRD_H[i] = sum7;
   }
   S7.Release();
   for (int i = 6 * H; i < 7 * H; i++) {
    A[i] = MXRD_H[i] + CminZ_H[i];
   }
   S2.Release();
   Console.WriteLine("T7 finished.");
  }
  static void Func8() {
   Console.WriteLine("T8 started.");
   for (int i = 7 * H; i < 8 * H; i++)
    Cmin[7] = C.Skip(7 * H).Take(8 * H).Min();

   S8.Release();
   S2.WaitOne();

   for (int i = 7 * H; i < 8 * H; i++) {
    CminZ_H[i] = Z_H[i] * Convert.ToInt32(Interlocked.Read(ref minC));

   }

   S8.Release();

   for (int i = 0; i < N; i++) {
    for (int j = 7 * H; j < 8 * H; j++) {
     sum8 = 0;
     for (int z = 0; z < N; z++) {
      sum8 = sum8 + MR_H[j * N + z] * MX[i * N + z];
     }
     MXR_H[i + j * N] = sum8;
    }
   }
   S8.Release();
   for (int i = 7 * H; i < 8 * H; i++) {
    sum8 = 0;
    for (int j = 0; j < N; j++) {
     sum8 = sum8 + D[j] * MXR_H[i * N + j];
    }
    MXRD_H[i] = sum8;
   }
   S8.Release();

   for (int i = 7 * H; i < 8 * H; i++) {
    A[i] = MXRD_H[i] + CminZ_H[i];
   }
   S2.Release();
   Console.WriteLine("T8 finished.");
  }




  static void Main(string[] args) {
   var sw = new Stopwatch();
   sw.Start();
   Console.WriteLine("Main thread started.");
   Thread T1 = new Thread(Func1);
   Thread T2 = new Thread(Func2);
   Thread T3 = new Thread(Func3);
   Thread T4 = new Thread(Func4);
   Thread T5 = new Thread(Func5);
   Thread T6 = new Thread(Func6);
   Thread T7 = new Thread(Func7);
   Thread T8 = new Thread(Func8);
   T1.Start();
   T2.Start();
   T3.Start();
   T4.Start();
   T5.Start();
   T6.Start();
   T7.Start();
   T8.Start();
   T2.Join();

   Console.WriteLine("Main thread finished");
   // Console.Read();
   sw.Stop();
   Console.WriteLine("elapsed " + sw.ElapsedMilliseconds);
  }
 }