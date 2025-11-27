using MediatR;

using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Application.Authors.GetAuthors;

public record GetAuthors(
    SearchAuthorCommand Command,
    int PageSize,
    int PageNumber
) : IRequest<(int, int, IEnumerable<AuthorDto>)>;
