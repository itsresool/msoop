namespace Msoop.Web.Reddit.Serialization
{
    public class RedditResource<T>
    {
        public string Kind { get; set; }
        public T Data { get; set; }
    }
}
