namespace UserService.Data.Models
{
    public class ApiResponse<T>
    {
        public string ResMsg { get; set; }
        public int ResCode { get; set; }
        public bool ResFlag { get; set; }
        public T? Data { get; set; }
    }
}
