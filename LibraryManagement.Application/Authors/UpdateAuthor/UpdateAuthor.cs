using MediatR;

using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Application.Authors.UpdateAuthor;

public record UpdateAuthor(UpdateAuthorCommand Command, long authorId) : IRequest<AuthorDto>;
