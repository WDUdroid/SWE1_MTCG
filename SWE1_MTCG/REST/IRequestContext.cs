namespace SWE1_MTCG.REST
{
    public interface IRequestContext
    {
        string StatusCode { get; set; }
        string Payload { get; set; }
        string ContentType { get; set; }

        void RequestCoordinator();
    }
}
