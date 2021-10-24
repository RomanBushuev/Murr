namespace Murzik.Entities.Enumerations
{
    public enum TaskStatuses
    {
        [MurrDb("UNDEFINED")]
        Undefined = 0,

        [MurrDb("CREATING")]
        Creating = 1,

        [MurrDb("CREATED")]
        Created = 2,

        [MurrDb("RUNNING")]
        Running = 3,

        [MurrDb("DONE")]
        Done = 4,

        [MurrDb("ERROR")]
        Error = 5,

        [MurrDb("STOPPING")]
        Stopping = 6,

        [MurrDb("STOP")]
        Stop = 7
    }
}
