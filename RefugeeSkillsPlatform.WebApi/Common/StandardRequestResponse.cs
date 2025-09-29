namespace RefugeeSkillsPlatform.WebApi.Common
{
    public class StandardRequestResponse<T>
    {
        public T? Data { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        // public List<T> Errors { get; set; } = new List<T>();
        public bool Success { get; set; }

    }
}
