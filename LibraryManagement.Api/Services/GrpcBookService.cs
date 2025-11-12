using AutoMapper;
using Grpc.Core;

using Librarymanagement;
using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Application.Services.Interaces;

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
}
