using System.Configuration;
using AutoMapper;
using FluentValidation;
using Grpc.Core;

using Librarymanagement;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Shared.Exceptions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace LibraryManagement.Api.Services;

public class GrpcBorrowingService: BorrowingService.BorrowingServiceBase
{
    private readonly IBorrowingService _borrowingService;
    private readonly IMapper _mapper;

    public GrpcBorrowingService(IBorrowingService borrowingService, IMapper mapper)
    {
        _borrowingService = borrowingService;
        _mapper = mapper;
    }

    public async override Task<BorrowingResponse> BorrowBook(BorrowBookRequest request, ServerCallContext context)
    {
        try
        {
            var bookBorrowCommand = _mapper.Map<BorrowBookCommand>(request);
            var newBorrowing = await _borrowingService.BorrowBookAsync(bookBorrowCommand);
            return _mapper.Map<BorrowingResponse>(newBorrowing);
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

    public async override Task<BorrowingResponse> ReturnBook(ReturnBookRequest request, ServerCallContext context)
    {
        try
        {
            var borrowingId = request.BorrowingId;
            var updatedBorrowing = await _borrowingService.ReturnBookAsync(borrowingId);
            return _mapper.Map<BorrowingResponse>(updatedBorrowing);
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
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

    public async override Task<BorrowingListResponse> GetUserBorrowings(UserBorrowingsRequest request, ServerCallContext context)
    {
        try
        {
            var userBorrowingsCommand = _mapper.Map<UserBorrowingsCommand>(request);

            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;

            var (totalCount, numberOfPages, searchResultDto) = await _borrowingService.GetUserBorrowingsAsync(
                userBorrowingsCommand, 
                pageNumber, 
                pageSize);

            var searchResult = _mapper.Map<IEnumerable<BorrowingResponse>>(searchResultDto);

            var response = new BorrowingListResponse
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                NumberOfPages = numberOfPages
            };
            response.Borrowings.AddRange(searchResult);
            return response;
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new RpcException(new Status(StatusCode.OutOfRange, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public async override Task<BorrowingListResponse> GetOverdueBooks(OverdueBooksRequest request, ServerCallContext context)
    {
        try
        {
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;

            var (totalCount, numberOfPages, searchResultDto) = await _borrowingService.GetOverdueBooksAsync(
                pageNumber, 
                pageSize);
            
            var searchResult = _mapper.Map<IEnumerable<BorrowingResponse>>(searchResultDto);

            var response = new BorrowingListResponse
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                NumberOfPages = numberOfPages
            };
            response.Borrowings.AddRange(searchResult);
            return response;
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new RpcException(new Status(StatusCode.OutOfRange, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}
