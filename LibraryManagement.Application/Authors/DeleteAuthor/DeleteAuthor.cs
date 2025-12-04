using MediatR;

namespace LibraryManagement.Application.Authors.DeleteAuthor;

public record DeleteAuthor(long authorId) : IRequest;
