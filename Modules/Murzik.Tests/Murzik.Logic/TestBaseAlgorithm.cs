using Moq;
using Murzik.Interfaces;
using NLog;

namespace Murzik.Tests.Murzik.Logic
{
    public abstract class TestBaseAlgorithm
    {
        protected Mock<ILogger> _logger;
        protected Mock<ITaskActions> _taskActions;

        public TestBaseAlgorithm()
        {
            _logger = new Mock<ILogger>();
            _taskActions = new Mock<ITaskActions>();
        }
    }
}
