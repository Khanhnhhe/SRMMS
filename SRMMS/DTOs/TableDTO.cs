namespace SRMMS.DTOs
{
    public class TableDTO
    {
        public int Table_Id { get; set; }
        public string Table_Name { get; set; }

        public int? StatusId { get; set; } = 1;
        public int? TableOfPeople { get; set; }



    }
}
