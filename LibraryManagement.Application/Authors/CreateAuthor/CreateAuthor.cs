using MediatR;

using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Application.Authors.CreateAuthor;

public record CreateAuthor(CreateAuthorCommand Command) : IRequest<AuthorDto>;
