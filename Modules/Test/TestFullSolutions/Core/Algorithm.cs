using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx.Synchronous;
using System;
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
}
