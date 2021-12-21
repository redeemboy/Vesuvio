namespace Vesuvio.Domain.DTO
{

    public class DataTableRequest
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public DataTableOrder[] Order { get; set; }
            public DataTableSearch Search { get; set; }
        }

        //For Sorting
        public class DataTableOrder
        {
            public string Column { get; set; }
            public string Dir { get; set; }
        }

        public class DataTableSearch
        {
            public string Column { get; set; }
            public string Value { get; set; }
        }
    
}
