namespace Murzik.Entities.Enumerations
{
    public enum ServiceStatuses
    {
        [MurrDb("UNDEFINED")]
        Undefined = 0,

        [MurrDb("RUNNING")]
        Running = 1,

        [MurrDb("STOPPING")]
        Stopping = 2,

        [MurrDb("FINISHED")]
        Finisehd = 3
    }
}
