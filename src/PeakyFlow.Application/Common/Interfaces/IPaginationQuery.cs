namespace PeakyFlow.Application.Common.Interfaces
{
    public interface IPaginationQuery
    {
        int PaginationCount { get; set; }

        int PaginationSkip { get; set; }
    }
}
