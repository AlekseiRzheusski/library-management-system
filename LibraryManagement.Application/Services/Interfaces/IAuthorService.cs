using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Application.Services.Interaces;

public interface IAuthorService
{
    public Task<AuthorDto> GetAuthorAsync(long authorId);
    public Task<(int totalCount, int numberOfPages, IEnumerable<AuthorDto>)> GetAuthorsAsync(
        SearchAuthorCommand command, 
        int pageSize, 
        int pageNumber);
    
    public Task<AuthorDto> CreateAuthorAsync(CreateAuthorCommand command);
    public Task<AuthorDto> UpdateAuthorAsync(UpdateAuthorCommand command, long authorId);
}
