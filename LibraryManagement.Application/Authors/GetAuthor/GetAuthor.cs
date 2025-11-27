using MediatR;

using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Application.Authors.GetAuthor;

public record GetAuthor(long AuthorId) : IRequest<AuthorDto>;
