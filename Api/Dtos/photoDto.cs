namespace Api.Dtos
{
    public class photoDto
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsMain { get; set; }
        
       public bool IsApproved { get; set; }
    }
}