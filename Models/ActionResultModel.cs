namespace TADS_Web.Models
{
    public class ActionResultModel<T>
    {
        public ActionResultModel()
        {
            IsSuccess = true;
        }

        public bool IsSuccess { get; set; }
        public string Description { get; set; } = null!;
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
