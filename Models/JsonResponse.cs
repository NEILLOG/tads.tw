namespace TADS_Web.Models
{
    public class JsonResponse<T>
    {
        public JsonResponse()
        {
            alert_plugin = "swal";
            alert_type = "success";
            IsSuccess = true;
        }

        public Boolean IsSuccess { get; set; }
        public String? Message { get; set; }
        public String? MessageDetail { get; set; }
        public T? Datas { get; set; }
        public string alert_plugin { get; set; }
        public string alert_type { get; set; }
        public string? redirect_url { get; set; }
    }
}
