using System.Data;
using AutoMapper;
using FluentValidation;
using Grpc.Core;

using Librarymanagement;
using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Shared.Exceptions;

namespace LibraryManagement.Api.Services;

public class GrpcBookService : BookService.BookServiceBase
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;
    public GrpcBookService(IBookService bookService, IMapper mapper)
    {
        _bookService = bookService;
        _mapper = mapper;
    }

    public override async Task<BookGetResponse> GetBook(BookGetRequest request, ServerCallContext context)
    {
        var book = await _bookService.GetBookAsync(request.BookId);
        if (book == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Book not found"));
        return new BookGetResponse 
        { 
            Book = _mapper.Map<BookResponse>(book) 
        };
    }

    public override async Task<BookResponse> CreateBook(CreateBookRequest request, ServerCallContext context)
    {
        try
        {
            var createBookCommand = _mapper.Map<CreateBookCommand>(request);
            var resultBookDto = await _bookService.CreateBookAsync(createBookCommand);
            return _mapper.Map<BookResponse>(resultBookDto);
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch
        {
            throw new RpcException(new Status(StatusCode.Internal, "Internal issue"));
        }
    }

    public override async Task<DeleteResponse> DeleteBook(BookDeleteRequest request, ServerCallContext context)
    {
        try
        {
            await _bookService.DeleteBookAsync(request.BookId);
            return new DeleteResponse {Message = $"{request.BookId} was successfully deleted."};
        }
        catch (IdNotFoundInDatabaseException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch
        {
            throw new RpcException(new Status(StatusCode.Internal, "Internal issue"));
        }
    }
}
