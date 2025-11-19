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
        try
        {
            var book = await _bookService.GetBookAsync(request.BookId);
            return new BookGetResponse
            {
                Book = _mapper.Map<BookResponse>(book)
            };
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
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
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<DeleteResponse> DeleteBook(BookDeleteRequest request, ServerCallContext context)
    {
        try
        {
            await _bookService.DeleteBookAsync(request.BookId);
            return new DeleteResponse { Message = $"{request.BookId} was successfully deleted." };
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<BookListResponse> GetBooks(BookPageRequest request, ServerCallContext context)
    {
        try
        {
            var searchBookCommand = _mapper.Map<SearchBookCommand>(request.SearchRequest);
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;

            var (totalCount, numberOfPages, searchResultDtos) = await _bookService.GetBooksAsync(searchBookCommand, pageSize, pageNumber);
            var searchResult = _mapper.Map<IEnumerable<BookResponse>>(searchResultDtos);


            var response = new BookListResponse
            {
                TotalCount = totalCount,
                NumberOfPages = numberOfPages,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            response.Books.AddRange(searchResult);

            return response;
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new RpcException(new Status(StatusCode.OutOfRange, ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<BookResponse> UpdateBook(UpdateBookRequest request, ServerCallContext context)
    {
        try
        {
            var updateBookCommand = _mapper.Map<UpdateBookCommand>(request);
            long bookId = request.BookId;

            var updatedBookDto = await _bookService.UpdateBookAsync(updateBookCommand, bookId);
            return _mapper.Map<BookResponse>(updatedBookDto);
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task GetBooksAllPages(
        BookAllPagesRequest request,
        IServerStreamWriter<BookListResponse> responseStream,
        ServerCallContext context)
    {
        try
        {
            int pageSize = request.PageSize;
            bool run = true;
            int pageNumber = 1;
            var searchBookCommand = _mapper.Map<SearchBookCommand>(request.SearchRequest);

            do
            {
                try
                {
                    var (totalCount, numberOfPages, searchResultDtos) = await _bookService.GetBooksAsync(searchBookCommand, pageSize, pageNumber);
                    var searchResult = _mapper.Map<IEnumerable<BookResponse>>(searchResultDtos);


                    var response = new BookListResponse
                    {
                        TotalCount = totalCount,
                        NumberOfPages = numberOfPages,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                    response.Books.AddRange(searchResult);

                    await responseStream.WriteAsync(response);
                    pageNumber++;

                    //imitation of hard work
                    await Task.Delay(1000);
                }
                catch (IndexOutOfRangeException)
                {
                    run = false;
                }

            } while (run);
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}
