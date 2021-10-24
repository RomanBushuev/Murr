using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx.Synchronous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestFullSolutions.Core
{
    public class Algorithm
    {
        CancellationTokenSource _cancellationTokenSource;
        CancellationToken _cancellationToken;

        public void Run()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            try
            {
                //Будет вызван OperationCanceledException из метода Code
                var runAlgorithm = Task.Run(async () =>
                {
                    Console.WriteLine("Start task code");
                    await RunAlgorithm();
                    //никогда мы не дойдем до этого момента
                    Console.WriteLine("End task code");
                }, _cancellationToken);

                //Во втором методе можно вызывать любой Exception, ничего не будет
                Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(300);
                        Console.WriteLine("Heartbeat");
                        if(_cancellationToken.IsCancellationRequested)
                        {
                            Console.WriteLine("End heartbeat");
                            _cancellationToken.ThrowIfCancellationRequested();
                        }
                    }                    
                }, _cancellationToken);

                //Всегда будет первым
                Console.WriteLine("Wait task");
                Thread.Sleep(3000);
                _cancellationTokenSource.Cancel();

                runAlgorithm.WaitAndUnwrapException();
                Thread.Sleep(10000);
                //никогда мы не дойдем до этого момента
                Console.WriteLine("End task");
            }
            catch(OperationCanceledException)
            {
                Console.WriteLine("OperationCanceledException");
            }
            catch (Exception)
            {
                Console.WriteLine("Exception");
            }
        }

        /// <summary>
        /// Запускаем алгоритм
        /// </summary>
        /// <returns></returns>
        public async Task RunAlgorithm()
        {
            Console.WriteLine("Start code");
            while (true)
            {
                await Task.Delay(1000);
                Console.WriteLine("Running code");
                if (_cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("End job");
                    _cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

    }

    [TestClass]
    public class TestAlgorithm
    {
        [TestMethod]
        public void TestRun()
        {
            var test = new Algorithm();
            test.Run();
        }
    }


    public static class TestStatic
    {
        static int I = 1;
        static object obj = new object();
        static TestStatic()
        {
            Console.WriteLine("Initialized");
        }

        public static void Show()
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} : {Task.CurrentId} : {I++}");

            lock (obj)
            {
                Thread.Sleep(1000);
                Console.WriteLine();
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} : {Task.CurrentId} : {I++}");
            }
        }
    }

    [TestClass]
    public class TT
    {
        [TestMethod]
        public void T()
        {
            for (int i = 0; i < 4; ++i)
            {
                new Thread(() =>
                {
                    TestStatic.Show();
                }).Start();
            }

            Thread.Sleep(5000);

            for (int i = 0; i < 4; ++i)
            {
                Task.Run(() =>
                {
                    TestStatic.Show();
                });
            }

            Thread.Sleep(5000);

            
        }

    }

    public static class Spliter
    {
        public static List<List<T>> SplitToSublists<T>(List<T> source)
        {
            return source
                     .Select((x, i) => new { Index = i, Value = x })
                     .GroupBy(x => x.Index / 10000)
                     .Select(x => x.Select(v => v.Value).ToList())
                     .ToList();
        }

        public static IEnumerable<IEnumerable<T>> SplitToEnumerables<T>(IEnumerable<T> source)
        {
            return source
                     .Select((x, i) => new { Index = i, Value = x })
                     .GroupBy(x => x.Index / 10000)
                     .Select(x => x.Select(v => v.Value));
        }
    }

    public class Roman
    {
        public string Text { get; set; }

        public long Number { get; set; }
    }


    [TestClass]
    public class TestData
    {
        [TestMethod]
        public void Test1()
        {
            var data = Enumerable.Range(1, 2000000).Select(z => new Roman()
            {
                Number = z,
                Text = $"Text {z}"
            }).ToList();

            long values = 0;
            foreach (var part in Spliter.SplitToSublists(data))
            {
                foreach(var v in part)
                {
                    values += v.Number + v.Text.Length;
                }
            }
            Console.WriteLine(values);
        }

        [TestMethod]
        public void Test2()
        {
            var data = Enumerable.Range(1, 2000000).Select(z => new Roman()
            {
                Number = z,
                Text = $"Text {z}"
            });

            long values = 0;
            foreach (var part in Spliter.SplitToEnumerables(data))
            {
                foreach (var v in part)
                {
                    values += v.Number + v.Text.Length;
                }
            }
            Console.WriteLine(values);
        }
    }

}
