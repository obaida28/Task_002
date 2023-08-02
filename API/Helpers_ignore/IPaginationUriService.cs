namespace API.Helpers;
public interface IPaginationUriService
    {
        public Uri GetPageUri(PaginationQuery paginationQuery);
    }