using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;

namespace AkkaAdapterService
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private ActorSystem mySystem;
        private IActorRef actor;
        private IActorRef second;
        private Timer _timer;
        private long _interval = 10;

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mySystem = ActorSystem.Create("MySystem");
            actor = mySystem.ActorOf<GreetingActor>("greeter");
            
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await mySystem.Terminate();
        }

        private void DoWork(object state)
        {
            mySystem.ActorSelection("user/greeter").Tell(new Greet("bushuev"));
            actor.Tell(new Greet("roman"));
        }
    }

    /// <summary>
    /// The actor class
    /// </summary>
    public class GreetingActor : ReceiveActor
    {
        public GreetingActor()
        { 
            // Tell the actor to respond to the Greet message
            Receive<Greet>(greet => Console.WriteLine("Hello {0}", greet.Who));
        }
    }

    /// <summary>
    /// Immutable message type that actor will respond to
    /// </summary>
    public class Greet
    {
        public string Who { get; private set; }

        public Greet(string who)
        {
            Who = who;
        }
    }
}
